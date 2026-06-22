namespace RoomBoard.Web.Models;

public sealed class KioskSpotlightSettings
{
    public int Id { get; set; } = 1;
    public bool IsManualEnabled { get; set; }
    public string Label { get; set; } = "Image of the day";
    public string Title { get; set; } = "Μια εικόνα, μια σκέψη";
    public string Text { get; set; } = "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση.";
    public string? ImagePath { get; set; }
    public string? Credit { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
