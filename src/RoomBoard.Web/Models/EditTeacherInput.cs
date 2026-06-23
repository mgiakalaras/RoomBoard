using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class EditTeacherInput
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Συμπληρώστε ονοματεπώνυμο καθηγητή/τριας.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(40)]
    public string? Specialty { get; set; }
}
