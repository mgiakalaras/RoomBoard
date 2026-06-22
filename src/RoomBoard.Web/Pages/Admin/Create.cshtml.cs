using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class CreateModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public CreateModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public CreateBookingInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateOnly PreviewDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IReadOnlyList<Room> Rooms { get; private set; } = Array.Empty<Room>();
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public IReadOnlyList<Teacher> Teachers { get; private set; } = Array.Empty<Teacher>();
    public IReadOnlyList<ClassGroup> ClassGroups { get; private set; } = Array.Empty<ClassGroup>();
    public IReadOnlyList<BookingDetails> PreviewBookings { get; private set; } = Array.Empty<BookingDetails>();
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadLists();

        if (PreviewDate == default)
        {
            PreviewDate = DateOnly.FromDateTime(DateTime.Today);
        }

        Input.Date = PreviewDate;

        var firstPeriod = Periods.FirstOrDefault();
        if (firstPeriod is not null)
        {
            Input.StartLessonPeriodId = firstPeriod.Id;
            Input.EndLessonPeriodId = firstPeriod.Id;
        }

        LoadPreviewBookings(Input.Date);
    }

    public IActionResult OnPost()
    {
        LoadLists();
        LoadPreviewBookings(Input.Date);

        if (!ModelState.IsValid)
        {
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.CreateBooking(Input);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/Index", new { date = Input.Date.ToString("yyyy-MM-dd") });
        }

        LoadPreviewBookings(Input.Date);
        return Page();
    }

    private void LoadLists()
    {
        Rooms = _roomBoard.GetRooms();
        Periods = _roomBoard.GetLessonPeriods();
        Teachers = _roomBoard.GetTeachers();
        ClassGroups = _roomBoard.GetClassGroups();
    }

    private void LoadPreviewBookings(DateOnly date)
    {
        PreviewDate = date;
        PreviewBookings = _roomBoard.GetBookingsForDate(date);
    }
}
