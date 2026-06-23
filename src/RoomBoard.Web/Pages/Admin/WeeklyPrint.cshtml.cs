using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class WeeklyPrintModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public WeeklyPrintModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty(SupportsGet = true)]
    public DateOnly WeekStart { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [BindProperty(SupportsGet = true)]
    public int[] ExtraRoomIds { get; set; } = Array.Empty<int>();

    public SchoolSettings SchoolSettings { get; private set; } = new();
    public IReadOnlyList<DateOnly> WeekDays { get; private set; } = Array.Empty<DateOnly>();
    public IReadOnlyList<Room> Rooms { get; private set; } = Array.Empty<Room>();
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public Dictionary<DateOnly, IReadOnlyList<BookingDetails>> BookingsByDate { get; private set; } = new();

    public void OnGet()
    {
        if (WeekStart == default)
        {
            WeekStart = DateOnly.FromDateTime(DateTime.Today);
        }

        WeekStart = GetMonday(WeekStart);
        WeekDays = Enumerable.Range(0, 5)
            .Select(offset => WeekStart.AddDays(offset))
            .ToList();

        SchoolSettings = _roomBoard.GetSchoolSettings();
        Rooms = _roomBoard.GetRooms();
        Periods = _roomBoard.GetLessonPeriods();

        BookingsByDate = WeekDays.ToDictionary(
            day => day,
            day => _roomBoard.GetBookingsForDate(day));
    }

    public string WeekRangeText =>
        WeekDays.Count == 0
            ? string.Empty
            : $"{WeekDays.First():dd/MM/yyyy} - {WeekDays.Last():dd/MM/yyyy}";

    private static DateOnly GetMonday(DateOnly date)
    {
        var diff = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-diff);
    }
}
