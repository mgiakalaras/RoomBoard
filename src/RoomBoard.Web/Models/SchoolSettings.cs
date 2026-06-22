namespace RoomBoard.Web.Models;

public sealed class SchoolSettings
{
    public int Id { get; set; } = 1;
    public string SchoolName { get; set; } = "Πυθαγόρειο ΓΕΛ Σάμου";
    public string SchoolType { get; set; } = "Γενικό Λύκειο";
    public string SchoolYear { get; set; } = "2025-2026";
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? LogoPath { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
