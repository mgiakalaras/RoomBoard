namespace RoomBoard.Web.Models;

public sealed class LessonPeriod
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Label => $"{Number}η ώρα";
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
