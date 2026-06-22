using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages;

public sealed class IndexModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;
    private readonly IConfiguration _configuration;

    public IndexModel(IRoomBoardService roomBoard, IConfiguration configuration)
    {
        _roomBoard = roomBoard;
        _configuration = configuration;
    }

    public string SchoolName { get; private set; } = string.Empty;
    public string? SchoolLogoPath { get; private set; }
    public DateOnly Today { get; private set; }
    public int DisplayPeriodNumber { get; private set; }
    public string DisplayPeriodText { get; private set; } = string.Empty;
    public string NextPeriodText { get; private set; } = string.Empty;
    public string AvailabilityPeriodText { get; private set; } = string.Empty;
    public string AvailabilityEmptyText { get; private set; } = string.Empty;
    public string NextMovementsEmptyText { get; private set; } = string.Empty;
    public bool IsCurrentTeachingPeriod { get; private set; }
    public bool HasNextTeachingPeriod { get; private set; }
    public bool IsAfterTeachingHours { get; private set; }
    public int AutoRefreshSeconds { get; private set; }
    public LessonPeriod? CurrentPeriod { get; private set; }
    public LessonPeriod? NextPeriod { get; private set; }
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public IReadOnlyList<RoomStatus> RoomStatuses { get; private set; } = Array.Empty<RoomStatus>();
    public IReadOnlyList<BookingDetails> Bookings { get; private set; } = Array.Empty<BookingDetails>();
    public IReadOnlyList<BookingDetails> NextPeriodMovements { get; private set; } = Array.Empty<BookingDetails>();

    public void OnGet()
    {
        var schoolSettings = _roomBoard.GetSchoolSettings();
        SchoolName = schoolSettings.SchoolName;
        SchoolLogoPath = schoolSettings.LogoPath;
        AutoRefreshSeconds = _configuration.GetValue("RoomBoard:AutoRefreshSeconds", 60);
        Today = DateOnly.FromDateTime(DateTime.Today);
        Periods = _roomBoard.GetLessonPeriods();
        Bookings = _roomBoard.GetBookingsForDate(Today);

        var now = DateTime.Now;
        var nowTime = TimeOnly.FromDateTime(now);

        CurrentPeriod = _roomBoard.GetCurrentPeriod(now);
        NextPeriod = CurrentPeriod is not null
            ? Periods.FirstOrDefault(p => p.Number > CurrentPeriod.Number)
            : Periods.FirstOrDefault(p => nowTime < p.StartTime);

        var firstPeriod = Periods.FirstOrDefault();
        var lastPeriod = Periods.LastOrDefault();

        IsCurrentTeachingPeriod = CurrentPeriod is not null;
        HasNextTeachingPeriod = NextPeriod is not null;
        IsAfterTeachingHours = lastPeriod is not null && nowTime > lastPeriod.EndTime;

        if (CurrentPeriod is not null)
        {
            DisplayPeriodNumber = CurrentPeriod.Number;
            DisplayPeriodText = $"{CurrentPeriod.Label} ώρα · {CurrentPeriod.StartTime:HH\\:mm} - {CurrentPeriod.EndTime:HH\\:mm}";
            AvailabilityPeriodText = DisplayPeriodText;
            AvailabilityEmptyText = "Δεν υπάρχουν διαθέσιμες αίθουσες αυτή τη διδακτική ώρα.";
        }
        else if (NextPeriod is not null)
        {
            DisplayPeriodNumber = NextPeriod.Number;
            DisplayPeriodText = nowTime < firstPeriod?.StartTime
                ? $"Πριν την έναρξη · επόμενη {NextPeriod.Label} ώρα"
                : $"Διάλειμμα / μεταξύ ωρών · επόμενη {NextPeriod.Label} ώρα";
            AvailabilityPeriodText = "Εκτός διδακτικής ώρας";
            AvailabilityEmptyText = "Η διαθεσιμότητα εμφανίζεται μόνο όταν βρισκόμαστε μέσα σε διδακτική ώρα.";
        }
        else
        {
            DisplayPeriodNumber = lastPeriod?.Number ?? firstPeriod?.Number ?? 1;
            DisplayPeriodText = "Εκτός σχολικού ωραρίου";
            AvailabilityPeriodText = "Εκτός σχολικού ωραρίου";
            AvailabilityEmptyText = "Το σχολικό διδακτικό ωράριο έχει ολοκληρωθεί για σήμερα.";
        }

        NextPeriodText = NextPeriod is null
            ? "Δεν υπάρχει επόμενη διδακτική ώρα σήμερα"
            : $"{NextPeriod.Label} ώρα · {NextPeriod.StartTime:HH\\:mm} - {NextPeriod.EndTime:HH\\:mm}";

        NextMovementsEmptyText = NextPeriod is null
            ? "Δεν υπάρχει επόμενη διδακτική ώρα σήμερα."
            : "Δεν υπάρχουν καταχωρισμένες μετακινήσεις τμημάτων για την επόμενη ώρα.";

        RoomStatuses = _roomBoard.GetRoomStatuses(Today, DisplayPeriodNumber);
        NextPeriodMovements = GetBookingsForPeriod(NextPeriod)
            .Where(b => b.ClassGroup is not null)
            .OrderBy(b => b.ClassGroup!.Name)
            .ThenBy(b => b.Room.DisplayOrder)
            .ToList();
    }

    private IReadOnlyList<BookingDetails> GetBookingsForPeriod(LessonPeriod? period)
    {
        if (period is null)
        {
            return Array.Empty<BookingDetails>();
        }

        return Bookings
            .Where(b => b.CoversPeriodNumber(period.Number))
            .ToList();
    }
}
