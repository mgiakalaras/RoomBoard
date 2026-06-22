namespace RoomBoard.Web.Models;

public sealed class BookingDetails
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public Room Room { get; set; } = new();
    public LessonPeriod StartPeriod { get; set; } = new();
    public LessonPeriod EndPeriod { get; set; } = new();
    public Teacher Teacher { get; set; } = new();
    public ClassGroup? ClassGroup { get; set; }
    public string SubjectOrPurpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Backwards-friendly alias for screens that mostly care about the start period.
    public LessonPeriod Period => StartPeriod;

    public string PeriodRangeLabel =>
        StartPeriod.Id == EndPeriod.Id
            ? StartPeriod.Label
            : $"{StartPeriod.Number}η - {EndPeriod.Number}η ώρα";

    public bool CoversPeriodNumber(int periodNumber)
    {
        return StartPeriod.Number <= periodNumber && EndPeriod.Number >= periodNumber;
    }

    public IEnumerable<int> CoveredPeriodNumbers()
    {
        for (var number = StartPeriod.Number; number <= EndPeriod.Number; number++)
        {
            yield return number;
        }
    }
}
