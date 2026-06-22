using System.ComponentModel.DataAnnotations;

namespace RoomBoard.Web.Models;

public sealed class KioskSpotlightInput
{
    public bool IsManualEnabled { get; set; }

    [StringLength(60)]
    public string Label { get; set; } = "Image of the day";

    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(260)]
    public string Text { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Credit { get; set; }
}
