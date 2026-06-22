using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RoomBoard.Web.Services;

public sealed class XlsxTable
{
    public string SheetName { get; init; } = string.Empty;
    public IReadOnlyList<string> Headers { get; init; } = Array.Empty<string>();
    public IReadOnlyList<IReadOnlyList<string>> Rows { get; init; } = Array.Empty<IReadOnlyList<string>>();
}

public static class SimpleXlsxReader
{
    private static readonly XNamespace MainNs = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace OfficeRelNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PackageRelNs = "http://schemas.openxmlformats.org/package/2006/relationships";

    public static XlsxTable ReadFirstTable(Stream stream, IReadOnlyList<string> preferredSheetNames)
    {
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

        var workbookEntry = archive.GetEntry("xl/workbook.xml")
            ?? throw new InvalidOperationException("Το αρχείο δεν μοιάζει με έγκυρο .xlsx αρχείο.");

        var relationshipEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
            ?? throw new InvalidOperationException("Δεν βρέθηκαν οι σχέσεις του workbook στο .xlsx αρχείο.");

        var sharedStrings = ReadSharedStrings(archive);

        XDocument workbookDoc;
        using (var workbookStream = workbookEntry.Open())
        {
            workbookDoc = XDocument.Load(workbookStream);
        }

        XDocument relsDoc;
        using (var relsStream = relationshipEntry.Open())
        {
            relsDoc = XDocument.Load(relsStream);
        }

        var sheets = workbookDoc.Root?
            .Element(MainNs + "sheets")?
            .Elements(MainNs + "sheet")
            .Select(sheet => new
            {
                Name = ((string?)sheet.Attribute("name") ?? string.Empty).Trim(),
                RelationId = ((string?)sheet.Attribute(OfficeRelNs + "id") ?? string.Empty).Trim()
            })
            .Where(sheet => !string.IsNullOrWhiteSpace(sheet.Name) && !string.IsNullOrWhiteSpace(sheet.RelationId))
            .ToList() ?? new();

        if (sheets.Count == 0)
        {
            throw new InvalidOperationException("Δεν βρέθηκε φύλλο εργασίας στο .xlsx αρχείο.");
        }

        var preferredNames = preferredSheetNames
            .Select(NormalizeName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var selectedSheet = sheets.FirstOrDefault(sheet => preferredNames.Contains(NormalizeName(sheet.Name)))
                            ?? sheets.First();

        var target = relsDoc.Root?
            .Elements(PackageRelNs + "Relationship")
            .FirstOrDefault(rel => string.Equals((string?)rel.Attribute("Id"), selectedSheet.RelationId, StringComparison.OrdinalIgnoreCase))
            ?.Attribute("Target")?.Value;

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new InvalidOperationException($"Δεν βρέθηκε το περιεχόμενο του φύλλου '{selectedSheet.Name}'.");
        }

        var worksheetPath = NormalizeWorksheetPath(target);
        var worksheetEntry = archive.GetEntry(worksheetPath)
            ?? throw new InvalidOperationException($"Δεν βρέθηκε το αρχείο φύλλου '{worksheetPath}' μέσα στο .xlsx.");

        XDocument sheetDoc;
        using (var worksheetStream = worksheetEntry.Open())
        {
            sheetDoc = XDocument.Load(worksheetStream);
        }

        var rows = new List<List<string>>();

        foreach (var row in sheetDoc.Descendants(MainNs + "sheetData").Elements(MainNs + "row"))
        {
            var valuesByColumn = new SortedDictionary<int, string>();

            foreach (var cell in row.Elements(MainNs + "c"))
            {
                var reference = ((string?)cell.Attribute("r") ?? string.Empty).Trim();
                var columnIndex = string.IsNullOrWhiteSpace(reference)
                    ? valuesByColumn.Count
                    : ColumnIndexFromCellReference(reference);

                valuesByColumn[columnIndex] = GetCellText(cell, sharedStrings);
            }

            if (valuesByColumn.Count == 0)
            {
                continue;
            }

            var maxColumn = valuesByColumn.Keys.Max();
            var values = Enumerable.Range(0, maxColumn + 1)
                .Select(index => valuesByColumn.TryGetValue(index, out var value) ? value.Trim() : string.Empty)
                .ToList();

            while (values.Count > 0 && string.IsNullOrWhiteSpace(values[^1]))
            {
                values.RemoveAt(values.Count - 1);
            }

            if (values.Any(value => !string.IsNullOrWhiteSpace(value)))
            {
                rows.Add(values);
            }
        }

        if (rows.Count == 0)
        {
            throw new InvalidOperationException($"Το φύλλο '{selectedSheet.Name}' δεν έχει δεδομένα.");
        }

        return new XlsxTable
        {
            SheetName = selectedSheet.Name,
            Headers = rows[0],
            Rows = rows.Skip(1).Cast<IReadOnlyList<string>>().ToList()
        };
    }

    private static IReadOnlyList<string> ReadSharedStrings(ZipArchive archive)
    {
        var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");
        if (sharedStringsEntry is null)
        {
            return Array.Empty<string>();
        }

        XDocument doc;
        using (var stream = sharedStringsEntry.Open())
        {
            doc = XDocument.Load(stream);
        }

        return doc.Root?
            .Elements(MainNs + "si")
            .Select(si => string.Concat(si.Descendants(MainNs + "t").Select(t => t.Value)))
            .ToList() ?? new List<string>();
    }

    private static string GetCellText(XElement cell, IReadOnlyList<string> sharedStrings)
    {
        var type = ((string?)cell.Attribute("t") ?? string.Empty).Trim();

        if (string.Equals(type, "s", StringComparison.OrdinalIgnoreCase))
        {
            var rawIndex = cell.Element(MainNs + "v")?.Value;
            return int.TryParse(rawIndex, out var index) && index >= 0 && index < sharedStrings.Count
                ? sharedStrings[index]
                : string.Empty;
        }

        if (string.Equals(type, "inlineStr", StringComparison.OrdinalIgnoreCase))
        {
            return string.Concat(cell.Descendants(MainNs + "t").Select(t => t.Value));
        }

        return cell.Element(MainNs + "v")?.Value ?? string.Empty;
    }

    private static int ColumnIndexFromCellReference(string reference)
    {
        var match = Regex.Match(reference, "^[A-Za-z]+");
        if (!match.Success)
        {
            return 0;
        }

        var index = 0;
        foreach (var ch in match.Value.ToUpperInvariant())
        {
            index = index * 26 + (ch - 'A' + 1);
        }

        return Math.Max(0, index - 1);
    }

    private static string NormalizeWorksheetPath(string target)
    {
        var clean = target.Replace("\\", "/").TrimStart('/');

        if (clean.StartsWith("xl/", StringComparison.OrdinalIgnoreCase))
        {
            return clean;
        }

        return "xl/" + clean;
    }

    private static string NormalizeName(string value)
    {
        return value.Trim().ToLowerInvariant().Replace(" ", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty);
    }
}
