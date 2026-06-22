using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoomBoard.Web.Models;
using RoomBoard.Web.Services;

namespace RoomBoard.Web.Pages.Admin;

public sealed class ImportModel : PageModel
{
    private readonly IRoomBoardService _roomBoard;

    public ImportModel(IRoomBoardService roomBoard)
    {
        _roomBoard = roomBoard;
    }

    [BindProperty]
    public IFormFile? ExcelFile { get; set; }

    [BindProperty]
    public string ImportType { get; set; } = "Teachers";

    public string? ResultMessage { get; private set; }
    public bool ResultSuccess { get; private set; }
    public ImportResult? Result { get; private set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (ExcelFile is null || ExcelFile.Length == 0)
        {
            ResultMessage = "Επιλέξτε αρχείο .xlsx για import.";
            ResultSuccess = false;
            return Page();
        }

        if (!Path.GetExtension(ExcelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ResultMessage = "Υποστηρίζονται μόνο αρχεία Excel .xlsx.";
            ResultSuccess = false;
            return Page();
        }

        try
        {
            using var stream = ExcelFile.OpenReadStream();

            var preferredSheetNames = ImportType switch
            {
                "Rooms" => new[] { "Rooms", "Room", "Αίθουσες", "Αιθουσες" },
                "ClassGroups" => new[] { "ClassGroups", "Classes", "Τμήματα", "Τμηματα", "Τμήμα", "Τμημα" },
                _ => new[] { "Teachers", "Teacher", "Καθηγητές", "Καθηγητες" }
            };

            var table = SimpleXlsxReader.ReadFirstTable(stream, preferredSheetNames);

            Result = ImportType switch
            {
                "Rooms" => ImportRooms(table),
                "ClassGroups" => ImportClassGroups(table),
                _ => ImportTeachers(table)
            };

            ResultSuccess = Result.FailedRows == 0;
            ResultMessage = ResultSuccess
                ? $"Το import ολοκληρώθηκε. Προστέθηκαν {Result.AddedRows} εγγραφές."
                : $"Το import ολοκληρώθηκε με {Result.FailedRows} σφάλματα.";
        }
        catch (Exception ex)
        {
            ResultMessage = "Το import απέτυχε: " + ex.Message;
            ResultSuccess = false;
        }

        return Page();
    }

    private ImportResult ImportTeachers(XlsxTable table)
    {
        var result = new ImportResult();
        var headers = table.Headers.Select(NormalizeHeader).ToList();

        var fullNameIndex = FindColumn(headers, "fullname", "teacher", "name", "ονοματεπωνυμο", "καθηγητης", "καθηγητρια", "εκπαιδευτικος", "ονομα");
        var specialtyIndex = FindColumn(headers, "specialty", "ειδικοτητα", "κλαδος");

        if (fullNameIndex < 0)
        {
            result.FailedRows++;
            result.Messages.Add("Δεν βρέθηκε στήλη ονοματεπωνύμου. Χρησιμοποιήστε FullName ή Ονοματεπώνυμο.");
            return result;
        }

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var rowNumber = i + 2;
            var row = table.Rows[i];
            var fullName = GetCell(row, fullNameIndex);

            if (string.IsNullOrWhiteSpace(fullName))
            {
                result.SkippedRows++;
                AddMessage(result, $"Γραμμή {rowNumber}: παραλείφθηκε γιατί δεν έχει ονοματεπώνυμο.");
                continue;
            }

            result.TotalRows++;

            var input = new AddTeacherInput
            {
                FullName = fullName,
                Specialty = specialtyIndex >= 0 ? GetCell(row, specialtyIndex) : null
            };

            var addResult = _roomBoard.AddTeacher(input);
            ApplyOperationResult(result, rowNumber, fullName, addResult);
        }

        return result;
    }

    private ImportResult ImportRooms(XlsxTable table)
    {
        var result = new ImportResult();
        var headers = table.Headers.Select(NormalizeHeader).ToList();

        var nameIndex = FindColumn(headers, "name", "room", "roomname", "αιθουσα", "ονομα", "ονομααιθουσας");
        var locationIndex = FindColumn(headers, "location", "τοποθεσια", "κτηριο", "κτιριο", "θεση", "περιγραφη");

        if (nameIndex < 0)
        {
            result.FailedRows++;
            result.Messages.Add("Δεν βρέθηκε στήλη αίθουσας. Χρησιμοποιήστε Name ή Αίθουσα.");
            return result;
        }

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var rowNumber = i + 2;
            var row = table.Rows[i];
            var roomName = GetCell(row, nameIndex);

            if (string.IsNullOrWhiteSpace(roomName))
            {
                result.SkippedRows++;
                AddMessage(result, $"Γραμμή {rowNumber}: παραλείφθηκε γιατί δεν έχει όνομα αίθουσας.");
                continue;
            }

            result.TotalRows++;

            var input = new AddRoomInput
            {
                Name = roomName,
                Location = locationIndex >= 0 ? GetCell(row, locationIndex) : null
            };

            var addResult = _roomBoard.AddRoom(input);
            ApplyOperationResult(result, rowNumber, roomName, addResult);
        }

        return result;
    }


    private ImportResult ImportClassGroups(XlsxTable table)
    {
        var result = new ImportResult();
        var headers = table.Headers.Select(NormalizeHeader).ToList();

        var nameIndex = FindColumn(headers, "name", "classgroup", "class", "τμημα", "τμηματα", "ονομα", "ονοματμηματος");

        if (nameIndex < 0)
        {
            result.FailedRows++;
            result.Messages.Add("Δεν βρέθηκε στήλη τμήματος. Χρησιμοποιήστε Name ή Τμήμα.");
            return result;
        }

        for (var i = 0; i < table.Rows.Count; i++)
        {
            var rowNumber = i + 2;
            var row = table.Rows[i];
            var classGroupName = GetCell(row, nameIndex);

            if (string.IsNullOrWhiteSpace(classGroupName))
            {
                result.SkippedRows++;
                AddMessage(result, $"Γραμμή {rowNumber}: παραλείφθηκε γιατί δεν έχει όνομα τμήματος.");
                continue;
            }

            result.TotalRows++;

            var input = new AddClassGroupInput
            {
                Name = classGroupName
            };

            var addResult = _roomBoard.AddClassGroup(input);
            ApplyOperationResult(result, rowNumber, classGroupName, addResult);
        }

        return result;
    }

    private static void ApplyOperationResult(ImportResult result, int rowNumber, string displayName, (bool Success, string Message) operation)
    {
        if (operation.Success)
        {
            result.AddedRows++;
            AddMessage(result, $"Γραμμή {rowNumber}: προστέθηκε '{displayName}'.");
            return;
        }

        if (operation.Message.Contains("Υπάρχει ήδη", StringComparison.CurrentCultureIgnoreCase))
        {
            result.SkippedRows++;
            AddMessage(result, $"Γραμμή {rowNumber}: παραλείφθηκε '{displayName}' — {operation.Message}");
            return;
        }

        result.FailedRows++;
        AddMessage(result, $"Γραμμή {rowNumber}: σφάλμα για '{displayName}' — {operation.Message}");
    }

    private static void AddMessage(ImportResult result, string message)
    {
        if (result.Messages.Count < 100)
        {
            result.Messages.Add(message);
        }
        else if (result.Messages.Count == 100)
        {
            result.Messages.Add("Εμφανίζονται μόνο τα πρώτα 100 μηνύματα.");
        }
    }

    private static string GetCell(IReadOnlyList<string> row, int index)
    {
        return index >= 0 && index < row.Count ? row[index].Trim() : string.Empty;
    }

    private static int FindColumn(IReadOnlyList<string> normalizedHeaders, params string[] aliases)
    {
        var normalizedAliases = aliases.Select(NormalizeHeader).ToHashSet(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < normalizedHeaders.Count; i++)
        {
            if (normalizedAliases.Contains(normalizedHeaders[i]))
            {
                return i;
            }
        }

        return -1;
    }

    private static string NormalizeHeader(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}

public sealed class ImportResult
{
    public int TotalRows { get; set; }
    public int AddedRows { get; set; }
    public int SkippedRows { get; set; }
    public int FailedRows { get; set; }
    public List<string> Messages { get; } = new();
}
