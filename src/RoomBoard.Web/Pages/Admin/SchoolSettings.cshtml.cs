using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class SchoolSettingsModel : PageModel
{
    private static readonly HashSet<string> AllowedLogoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".webp"
    };

    private readonly IRoomBoardService _roomBoard;
    private readonly IWebHostEnvironment _environment;

    public SchoolSettingsModel(IRoomBoardService roomBoard, IWebHostEnvironment environment)
    {
        _roomBoard = roomBoard;
        _environment = environment;
    }

    [BindProperty]
    public SchoolSettingsInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? LogoFile { get; set; }

    public string? CurrentLogoPath { get; private set; }
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadSettings();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CurrentLogoPath = _roomBoard.GetSchoolSettings().LogoPath;

        if (!ModelState.IsValid)
        {
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        string? logoPath = null;

        if (LogoFile is not null && LogoFile.Length > 0)
        {
            var extension = Path.GetExtension(LogoFile.FileName);
            if (!AllowedLogoExtensions.Contains(extension))
            {
                ResultMessage = "Το λογότυπο πρέπει να είναι PNG, JPG/JPEG ή WEBP.";
                ResultSuccess = false;
                return Page();
            }

            if (LogoFile.Length > 2 * 1024 * 1024)
            {
                ResultMessage = "Το αρχείο λογοτύπου πρέπει να είναι έως 2MB.";
                ResultSuccess = false;
                return Page();
            }

            var uploadsDirectory = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDirectory);

            var fileName = "school-logo" + extension.ToLowerInvariant();
            var destinationPath = Path.Combine(uploadsDirectory, fileName);

            await using var stream = System.IO.File.Create(destinationPath);
            await LogoFile.CopyToAsync(stream);

            logoPath = "/uploads/" + fileName;
        }

        var result = _roomBoard.SaveSchoolSettings(Input, logoPath);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadSettings();
        return Page();
    }

    private void LoadSettings()
    {
        var settings = _roomBoard.GetSchoolSettings();
        Input = new SchoolSettingsInput
        {
            SchoolName = settings.SchoolName,
            SchoolType = settings.SchoolType,
            SchoolYear = settings.SchoolYear,
            Address = settings.Address,
            Phone = settings.Phone,
            Email = settings.Email
        };
        CurrentLogoPath = settings.LogoPath;
    }
}
