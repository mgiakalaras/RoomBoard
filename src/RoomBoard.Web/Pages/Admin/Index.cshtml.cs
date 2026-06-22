using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class IndexModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public IndexModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IReadOnlyList<Room> Rooms { get; private set; } = Array.Empty<Room>();
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public IReadOnlyList<BookingDetails> Bookings { get; private set; } = Array.Empty<BookingDetails>();

    [TempData]
    public string? ResultMessage { get; set; }

    [TempData]
    public bool ResultSuccess { get; set; }

    public void OnGet()
    {
        Rooms = _roomBoard.GetRooms();
        Periods = _roomBoard.GetLessonPeriods();
        Bookings = _roomBoard.GetBookingsForDate(Date);
    }

    public IActionResult OnPostCancel(int bookingId, string? cancellationReason)
    {
        var result = _roomBoard.CancelBooking(bookingId, cancellationReason);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        return RedirectToPage(new { Date = Date.ToString("yyyy-MM-dd") });
    }

    public BookingDetails? FindBooking(int roomId, int periodNumber)
    {
        return Bookings.FirstOrDefault(b => b.Room.Id == roomId && b.CoversPeriodNumber(periodNumber));
    }
}
