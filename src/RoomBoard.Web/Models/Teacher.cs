namespace RoomBoard.Web.Models;

public sealed class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public bool IsActive { get; set; } = true;
}
