using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class TeachersModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public TeachersModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public AddTeacherInput NewTeacher { get; set; } = new();

    [BindProperty]
    public EditTeacherInput EditTeacher { get; set; } = new();

    public IReadOnlyList<Teacher> ActiveTeachers { get; private set; } = Array.Empty<Teacher>();
    public IReadOnlyList<Teacher> InactiveTeachers { get; private set; } = Array.Empty<Teacher>();
    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadTeachers();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadTeachers();
            ResultMessage = "Ελέγξτε τα πεδία της φόρμας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.AddTeacher(NewTeacher);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/Teachers");
        }

        LoadTeachers();
        return Page();
    }


    public IActionResult OnPostEdit()
    {
        if (!ModelState.IsValid)
        {
            LoadTeachers();
            ResultMessage = "Ελέγξτε τα πεδία επεξεργασίας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.UpdateTeacher(EditTeacher);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadTeachers();
        return Page();
    }

    public IActionResult OnPostDeactivate(int id)
    {
        var result = _roomBoard.DeactivateTeacher(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadTeachers();
        return Page();
    }

    public IActionResult OnPostReactivate(int id)
    {
        var result = _roomBoard.ReactivateTeacher(id);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        LoadTeachers();
        return Page();
    }

    private void LoadTeachers()
    {
        ActiveTeachers = _roomBoard.GetTeachers();
        InactiveTeachers = _roomBoard.GetInactiveTeachers();
    }
}
