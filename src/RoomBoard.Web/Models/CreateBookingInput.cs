using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class CreateBookingInput
{
    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Range(1, int.MaxValue, ErrorMessage = "Επιλέξτε αίθουσα.")]
    public int RoomId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Επιλέξτε ώρα έναρξης.")]
    public int StartLessonPeriodId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Επιλέξτε ώρα λήξης.")]
    public int EndLessonPeriodId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Επιλέξτε καθηγητή/τρια.")]
    public int TeacherId { get; set; }

    public int? ClassGroupId { get; set; }

    [Required(ErrorMessage = "Συμπληρώστε μάθημα ή χρήση.")]
    [StringLength(100)]
    public string SubjectOrPurpose { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Notes { get; set; }
}
