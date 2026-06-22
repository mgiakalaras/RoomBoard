using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class AddRoomInput
{
    [Required(ErrorMessage = "Συμπληρώστε όνομα αίθουσας.")]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Location { get; set; }
}
