namespace RoomBoard.Web.Models;

public sealed class Booking
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int RoomId { get; set; }
    public int StartLessonPeriodId { get; set; }
    public int EndLessonPeriodId { get; set; }
    public int TeacherId { get; set; }
    public int? ClassGroupId { get; set; }
    public string SubjectOrPurpose { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
