using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class ClassGroupsModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public ClassGroupsModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public AddClassGroupInput NewClassGroup { get; set; } = new();

    [BindProperty]
    public EditClassGroupInput EditClassGroup { get; set; } = new();

    public IReadOnlyList<ClassGroup> ActiveClassGroups { get; private set; } = Array.Empty<ClassGroup>();
    public IReadOnlyList<ClassGroup> InactiveClassGroups { get; private set; } = Array.Empty<ClassGroup>();
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadClassGroups();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadClassGroups();
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.AddClassGroup(NewClassGroup);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/ClassGroups");
        }

        LoadClassGroups();
        return Page();
    }


    public IActionResult OnPostEdit()
    {
        if (!ModelState.IsValid)
        {
            LoadClassGroups();
            ResultMessage = "Ελέγξτε τα πεδία επεξεργασίας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.UpdateClassGroup(EditClassGroup);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadClassGroups();
        return Page();
    }

    public IActionResult OnPostDeactivate(int id)
    {
        var result = _roomBoard.DeactivateClassGroup(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadClassGroups();
        return Page();
    }

    public IActionResult OnPostReactivate(int id)
    {
        var result = _roomBoard.ReactivateClassGroup(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadClassGroups();
        return Page();
    }

    private void LoadClassGroups()
    {
        ActiveClassGroups = _roomBoard.GetClassGroups();
        InactiveClassGroups = _roomBoard.GetInactiveClassGroups();
    }
}
