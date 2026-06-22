using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class AddClassGroupInput
{
    [Required(ErrorMessage = "Συμπληρώστε όνομα τμήματος.")]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;
}
