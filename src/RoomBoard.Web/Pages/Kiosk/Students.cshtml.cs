
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Kiosk;

public sealed class StudentsModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public StudentsModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    public SchoolSettings SchoolSettings { get; private set; } = new();
    public DateOnly Today { get; private set; }
    public IReadOnlyList<LessonPeriod> Periods { get; private set; } = Array.Empty<LessonPeriod>();
    public IReadOnlyList<BookingDetails> CurrentPeriodBookings { get; private set; } = Array.Empty<BookingDetails>();
    public IReadOnlyList<BookingDetails> NextPeriodBookings { get; private set; } = Array.Empty<BookingDetails>();
    public LessonPeriod? CurrentPeriod { get; private set; }
    public LessonPeriod? NextPeriod { get; private set; }
    public KioskInfoItem InfoItem { get; private set; } = KioskInfoItem.Default;
    public string StatusText { get; private set; } = string.Empty;
    public string MainPeriodLabel { get; private set; } = string.Empty;
    public string PeriodBadgeText { get; private set; } = string.Empty;
    public string EmptyStateTitle { get; private set; } = string.Empty;
    public string EmptyStateMessage { get; private set; } = string.Empty;
    public string CarouselModeText { get; private set; } = string.Empty;
    public bool IsBeforeFirstPeriod { get; private set; }
    public bool IsAfterTeachingHours { get; private set; }
    public int AutoRefreshSeconds { get; } = 60;
    public int CarouselSeconds { get; } = 8;

    public void OnGet()
    {
        SchoolSettings = _roomBoard.GetSchoolSettings();
        Today = DateOnly.FromDateTime(DateTime.Today);
        var spotlightSettings = _roomBoard.GetKioskSpotlightSettings();
        InfoItem = spotlightSettings.IsManualEnabled
            ? KioskInfoItem.FromSettings(spotlightSettings)
            : KioskInfoProvider.GetForDate(Today);
        Periods = _roomBoard.GetLessonPeriods();

        var now = TimeOnly.FromDateTime(DateTime.Now);
        var firstPeriod = Periods.FirstOrDefault();
        var lastPeriod = Periods.LastOrDefault();

        IsBeforeFirstPeriod = firstPeriod is not null && now < firstPeriod.StartTime;
        IsAfterTeachingHours = lastPeriod is not null && now > lastPeriod.EndTime;

        CurrentPeriod = Periods.FirstOrDefault(p => now >= p.StartTime && now <= p.EndTime);

        if (CurrentPeriod is not null)
        {
            NextPeriod = Periods.FirstOrDefault(p => p.Number > CurrentPeriod.Number);
            StatusText = "Τρέχουσα διδακτική ώρα";
            MainPeriodLabel = $"{CurrentPeriod.Label} ώρα · {CurrentPeriod.StartTime:HH\\:mm} - {CurrentPeriod.EndTime:HH\\:mm}";
            PeriodBadgeText = $"Τώρα: {CurrentPeriod.Label}";
            CarouselModeText = "Οι αλλαγές αυτής της ώρας εμφανίζονται κυκλικά.";
            EmptyStateTitle = "Κανονική διδακτική ώρα";
            EmptyStateMessage = "Δεν υπάρχουν καταχωρισμένες μετακινήσεις τμημάτων για αυτή την ώρα.";
        }
        else
        {
            NextPeriod = Periods.FirstOrDefault(p => now < p.StartTime);

            if (NextPeriod is not null)
            {
                StatusText = IsBeforeFirstPeriod ? "Πριν την έναρξη" : "Διάλειμμα / μεταξύ ωρών";
                MainPeriodLabel = $"Επόμενη: {NextPeriod.Label} ώρα · {NextPeriod.StartTime:HH\\:mm} - {NextPeriod.EndTime:HH\\:mm}";
                PeriodBadgeText = $"Επόμενη: {NextPeriod.Label}";
                CarouselModeText = "Προβάλλονται οι αλλαγές της επόμενης διδακτικής ώρας.";
                EmptyStateTitle = IsBeforeFirstPeriod ? "Καλή αρχή" : "Δεν υπάρχουν αλλαγές";
                EmptyStateMessage = IsBeforeFirstPeriod
                    ? $"Το πρόγραμμα θα εμφανίσει τις αλλαγές της ημέρας με την έναρξη της {NextPeriod.Label} ώρας."
                    : "Δεν υπάρχουν καταχωρισμένες μετακινήσεις τμημάτων για την επόμενη ώρα.";
            }
            else
            {
                StatusText = "Εκτός διδακτικού ωραρίου";
                MainPeriodLabel = "Το διδακτικό ωράριο έχει ολοκληρωθεί";
                PeriodBadgeText = "Εκτός ωραρίου";
                CarouselModeText = "Η προβολή θα επανέλθει αυτόματα την επόμενη σχολική ημέρα.";
                EmptyStateTitle = "Το πρόγραμμα ολοκληρώθηκε";
                EmptyStateMessage = "Δεν υπάρχουν άλλες καταχωρισμένες μετακινήσεις τμημάτων για σήμερα.";
            }
        }

        var todayBookings = _roomBoard.GetBookingsForDate(Today);

        CurrentPeriodBookings = CurrentPeriod is null
            ? Array.Empty<BookingDetails>()
            : todayBookings
                .Where(b => b.ClassGroup is not null && b.CoversPeriodNumber(CurrentPeriod.Number))
                .OrderBy(b => b.ClassGroup!.Name)
                .ThenBy(b => b.Room.Name)
                .ToList();

        NextPeriodBookings = NextPeriod is null
            ? Array.Empty<BookingDetails>()
            : todayBookings
                .Where(b => b.ClassGroup is not null && b.CoversPeriodNumber(NextPeriod.Number))
                .OrderBy(b => b.ClassGroup!.Name)
                .ThenBy(b => b.Room.Name)
                .ToList();
    }
}

public sealed record KioskInfoItem(
    string Label,
    string Title,
    string Text,
    string ImagePath,
    string Credit)
{
    public static KioskInfoItem Default { get; } = new(
        "Image of the day",
        "Μια εικόνα, μια σκέψη",
        "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση.",
        "/img/kiosk/spotlight-cosmos.svg",
        "RoomBoard visual spotlight");

    public static KioskInfoItem FromSettings(KioskSpotlightSettings settings)
    {
        return new KioskInfoItem(
            string.IsNullOrWhiteSpace(settings.Label) ? "Μήνυμα σχολείου" : settings.Label,
            string.IsNullOrWhiteSpace(settings.Title) ? Default.Title : settings.Title,
            string.IsNullOrWhiteSpace(settings.Text) ? Default.Text : settings.Text,
            string.IsNullOrWhiteSpace(settings.ImagePath) ? Default.ImagePath : settings.ImagePath,
            string.IsNullOrWhiteSpace(settings.Credit) ? "Χειροκίνητο μήνυμα σχολικής μονάδας" : settings.Credit);
    }
}

public static class KioskInfoProvider
{
    private static readonly KioskInfoItem[] Items =
    {
        new("Image of the day", "Ο ουρανός ως εργαστήριο", "Κάθε παρατήρηση ξεκινά από κάτι απλό: κοιτάζω, συγκρίνω, ρωτώ.", "/img/kiosk/spotlight-cosmos.svg", "Τοπική εικόνα · ασφαλές fallback"),
        new("Γνώριζες ότι;", "Τα δεδομένα χρειάζονται κρίση", "Ένας αριθμός μόνος του δεν λέει πάντα την αλήθεια. Το πλαίσιο κάνει τη διαφορά.", "/img/kiosk/spotlight-data.svg", "RoomBoard did-you-know"),
        new("Σαν σήμερα", "Η γνώση ταξιδεύει", "Μια ιδέα μπορεί να ξεκινήσει σε μια τάξη και να φτάσει πολύ μακρύτερα από όσο φανταζόμαστε.", "/img/kiosk/spotlight-horizon.svg", "Τοπική εικόνα · χωρίς internet"),
        new("Παγκόσμια ημέρα", "Σεβασμός στον κοινό χώρο", "Οι κοινόχρηστες αίθουσες λειτουργούν καλύτερα όταν τις αφήνουμε έτοιμες για τον επόμενο.", "/img/kiosk/spotlight-school.svg", "Σχολική υπενθύμιση"),
        new("Γνώριζες ότι;", "Η τεχνολογία είναι εργαλείο", "Η αξία της φαίνεται όταν βοηθά ανθρώπους να μαθαίνουν, να δημιουργούν και να συνεργάζονται.", "/img/kiosk/spotlight-circuit.svg", "RoomBoard visual spotlight"),
        new("Μικρή σκέψη", "Έλεγξε την πηγή", "Πριν πιστέψεις μια πληροφορία στο διαδίκτυο, δες ποιος την έγραψε και πότε δημοσιεύτηκε.", "/img/kiosk/spotlight-library.svg", "Ψηφιακός γραμματισμός")
    };

    public static KioskInfoItem GetForDate(DateOnly date)
    {
        var index = Math.Abs(date.DayNumber) % Items.Length;
        return Items[index];
    }
}
