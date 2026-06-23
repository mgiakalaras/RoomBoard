using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class RoomsModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public RoomsModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public AddRoomInput NewRoom { get; set; } = new();

    [BindProperty]
    public EditRoomInput EditRoom { get; set; } = new();

    public IReadOnlyList<Room> ActiveRooms { get; private set; } = Array.Empty<Room>();
    public IReadOnlyList<Room> InactiveRooms { get; private set; } = Array.Empty<Room>();
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadRooms();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadRooms();
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.AddRoom(NewRoom);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/Rooms");
        }

        LoadRooms();
        return Page();
    }


    public IActionResult OnPostEdit()
    {
        if (!ModelState.IsValid)
        {
            LoadRooms();
            ResultMessage = "Ελέγξτε τα πεδία επεξεργασίας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.UpdateRoom(EditRoom);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadRooms();
        return Page();
    }

    public IActionResult OnPostDeactivate(int id)
    {
        var result = _roomBoard.DeactivateRoom(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadRooms();
        return Page();
    }

    public IActionResult OnPostReactivate(int id)
    {
        var result = _roomBoard.ReactivateRoom(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadRooms();
        return Page();
    }

    private void LoadRooms()
    {
        ActiveRooms = _roomBoard.GetRooms();
        InactiveRooms = _roomBoard.GetInactiveRooms();
    }
}
