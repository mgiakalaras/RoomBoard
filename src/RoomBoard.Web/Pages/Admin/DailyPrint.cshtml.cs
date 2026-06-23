using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class DailyPrintModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public DailyPrintModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public SchoolSettings SchoolSettings { get; private set; } = new();
    public IReadOnlyList<Room> Rooms { get; private set; } = Array.Empty<Room>();
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public IReadOnlyList<BookingDetails> Bookings { get; private set; } = Array.Empty<BookingDetails>();

    public void OnGet()
    {
        if (Date == default)
        {
            Date = DateOnly.FromDateTime(DateTime.Today);
        }

        SchoolSettings = _roomBoard.GetSchoolSettings();
        Rooms = _roomBoard.GetRooms();
        Periods = _roomBoard.GetLessonPeriods();
        Bookings = _roomBoard.GetBookingsForDate(Date);
    }
}
