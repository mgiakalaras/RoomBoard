using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class SchoolSettingsInput
{
    [Required(ErrorMessage = "Συμπληρώστε όνομα σχολικής μονάδας.")]
    [StringLength(160)]
    public string SchoolName { get; set; } = string.Empty;

    [StringLength(100)]
    public string SchoolType { get; set; } = string.Empty;

    [StringLength(30)]
    public string SchoolYear { get; set; } = string.Empty;

    [StringLength(180)]
    public string? Address { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }
}
