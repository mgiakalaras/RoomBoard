using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class PeriodsModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public PeriodsModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public List<EditLessonPeriodInput> PeriodEdits { get; set; } = new();

    [BindProperty]
    public AddLessonPeriodInput NewPeriod { get; set; } = new();

    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }

    public void OnGet()
    {
        LoadPeriodEdits();
        PrepareNewPeriod();
    }

    public IActionResult OnPostSave()
    {
        // Remove validation entries from the add-period form; this post is only for the edit table.
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("NewPeriod", StringComparison.Ordinal)).ToList())
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            PrepareNewPeriod();
            ResultMessage = "Ελέγξτε τα πεδία του ωραρίου.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.UpdateLessonPeriods(PeriodEdits);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/Periods");
        }

        PrepareNewPeriod();
        return Page();
    }

    public IActionResult OnPostAdd()
    {
        // Remove validation entries from the edit table; this post is only for NewPeriod.
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("PeriodEdits", StringComparison.Ordinal)).ToList())
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            LoadPeriodEdits();
            ResultMessage = "Ελέγξτε τα πεδία της νέας ώρας.";
            ResultSuccess = false;
            return Page();
        }

        var result = _roomBoard.AddLessonPeriod(NewPeriod);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        if (result.Success)
        {
            return RedirectToPage("/Admin/Periods");
        }

        LoadPeriodEdits();
        return Page();
    }

    private void LoadPeriodEdits()
    {
        PeriodEdits = _roomBoard.GetLessonPeriods()
            .Select(p => new EditLessonPeriodInput
            {
                Id = p.Id,
                Number = p.Number,
                StartTime = p.StartTime,
                EndTime = p.EndTime
            })
            .ToList();
    }

    private void PrepareNewPeriod()
    {
        var periods = _roomBoard.GetLessonPeriods();
        var lastPeriod = periods.OrderBy(p => p.Number).LastOrDefault();

        NewPeriod = new AddLessonPeriodInput
        {
            Number = (lastPeriod?.Number ?? 0) + 1,
            StartTime = lastPeriod?.EndTime.AddMinutes(5) ?? new TimeOnly(8, 15),
            EndTime = lastPeriod?.EndTime.AddMinutes(50) ?? new TimeOnly(9, 0)
        };
    }
}
