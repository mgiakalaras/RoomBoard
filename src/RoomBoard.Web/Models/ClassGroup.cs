namespace RoomBoard.Web.Models;

public sealed class ClassGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
