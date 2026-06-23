using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class PrintsModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public PrintsModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    public DateOnly Today { get; private set; }
    public DateOnly WeekStart { get; private set; }
    public int TodayBookingCount { get; private set; }
    public int WeekBookingCount { get; private set; }

    public void OnGet()
    {
        Today = DateOnly.FromDateTime(DateTime.Today);
        WeekStart = GetMonday(Today);

        TodayBookingCount = _roomBoard.GetBookingsForDate(Today).Count;

        WeekBookingCount = Enumerable.Range(0, 5)
            .Select(offset => WeekStart.AddDays(offset))
            .Sum(day => _roomBoard.GetBookingsForDate(day).Count);
    }

    private static DateOnly GetMonday(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-diff);
    }
}
