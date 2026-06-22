using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class AddLessonPeriodInput
{
    [Range(1, 20, ErrorMessage = "Ο αριθμός ώρας πρέπει να είναι από 1 έως 20.")]
    public int Number { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }
}
