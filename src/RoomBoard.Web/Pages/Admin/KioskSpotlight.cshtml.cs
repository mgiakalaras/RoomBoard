using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class KioskSpotlightModel : PageModel
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp", ".svg"
    };

    private readonly IRoomBoardService _roomBoard;
    private readonly IWebHostEnvironment _environment;

    public KioskSpotlightModel(IRoomBoardService roomBoard, IWebHostEnvironment environment)
    {
        _roomBoard = roomBoard;
        _environment = environment;
    }

    [BindProperty]
    public KioskSpotlightInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? SpotlightImageFile { get; set; }

    public KioskSpotlightSettings CurrentSettings { get; private set; } = new();
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadSettings();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CurrentSettings = _roomBoard.GetKioskSpotlightSettings();

        if (!ModelState.IsValid)
        {
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        string? imagePath = null;

        if (SpotlightImageFile is not null && SpotlightImageFile.Length > 0)
        {
            var extension = Path.GetExtension(SpotlightImageFile.FileName);
            if (!AllowedImageExtensions.Contains(extension))
            {
                ResultMessage = "Η εικόνα πρέπει να είναι PNG, JPG/JPEG, WEBP ή SVG.";
                ResultSuccess = false;
                return Page();
            }

            if (SpotlightImageFile.Length > 4 * 1024 * 1024)
            {
                ResultMessage = "Η εικόνα πρέπει να είναι έως 4MB.";
                ResultSuccess = false;
                return Page();
            }

            var uploadsDirectory = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDirectory);

            var fileName = "kiosk-spotlight" + extension.ToLowerInvariant();
            var destinationPath = Path.Combine(uploadsDirectory, fileName);

            await using var stream = System.IO.File.Create(destinationPath);
            await SpotlightImageFile.CopyToAsync(stream);

            imagePath = "/uploads/" + fileName;
        }

        var result = _roomBoard.SaveKioskSpotlightSettings(Input, imagePath);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadSettings();
        return Page();
    }

    private void LoadSettings()
    {
        CurrentSettings = _roomBoard.GetKioskSpotlightSettings();
        Input = new KioskSpotlightInput
        {
            IsManualEnabled = CurrentSettings.IsManualEnabled,
            Label = CurrentSettings.Label,
            Title = CurrentSettings.Title,
            Text = CurrentSettings.Text,
            Credit = CurrentSettings.Credit
        };
    }
}
