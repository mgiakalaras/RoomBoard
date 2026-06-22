namespace RoomBoard.Web.Models;

public sealed class RoomStatus
{
    public Room Room { get; set; } = new();
    public BookingDetails? CurrentBooking { get; set; }
    public BookingDetails? NextBooking { get; set; }
    public IReadOnlyList<int> BusyPeriodNumbers { get; set; } = Array.Empty<int>();

    public bool IsBusy => CurrentBooking is not null;
    public bool HasNext => NextBooking is not null;
}
