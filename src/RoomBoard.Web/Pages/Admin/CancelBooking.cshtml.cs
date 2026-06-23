using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class CancelBookingModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public CancelBookingModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty(SupportsGet = true)]
    public int BookingId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? ReturnDate { get; set; }

    [BindProperty]
    public string? CancellationReason { get; set; }

    public BookingDetails? Booking { get; private set; }
    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        Booking = _roomBoard.GetBookingById(BookingId);
        if (Booking is null)
        {
            ErrorMessage = "Η κράτηση δεν βρέθηκε.";
        }
        else if (Booking.IsCancelled)
        {
            ErrorMessage = "Η κράτηση είναι ήδη ακυρωμένη.";
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        Booking = _roomBoard.GetBookingById(BookingId);
        if (Booking is null)
        {
            ErrorMessage = "Η κράτηση δεν βρέθηκε.";
            return Page();
        }

        if (Booking.IsCancelled)
        {
            ErrorMessage = "Η κράτηση είναι ήδη ακυρωμένη.";
            return Page();
        }

        var result = _roomBoard.CancelBooking(BookingId, CancellationReason);
        if (!result.Success)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        var returnDate = ReturnDate ?? Booking.Date;
        return RedirectToPage("/Admin/Index", new { Date = returnDate.ToString("yyyy-MM-dd") });
    }
}
