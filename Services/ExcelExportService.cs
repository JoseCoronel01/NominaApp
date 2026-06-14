using System.Text;
using ClosedXML.Excel;
using Microsoft.JSInterop;

namespace NominaApp.Services;

public class ExcelExportService
{
    private readonly IJSRuntime _js;

    public ExcelExportService(IJSRuntime js)
    {
        _js = js;
    }

    public Task<byte[]> ExportAsync<T>(
        IEnumerable<T> data,
        string sheetName,
        string[]? headers = null,
        CancellationToken ct = default)
    {
        var list = data?.ToList() ?? new List<T>();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Sheet1" : sheetName);

        var props = typeof(T).GetProperties().Where(p => p.CanRead).ToArray();

        // headers
        for (int c = 0; c < props.Length; c++)
        {
            var header = headers != null && c < headers.Length && !string.IsNullOrWhiteSpace(headers[c])
                ? headers[c]
                : props[c].Name;

            worksheet.Cell(1, c + 1).Value = header;
        }

        // rows
        for (int r = 0; r < list.Count; r++)
        {
            for (int c = 0; c < props.Length; c++)
            {
                var value = props[c].GetValue(list[r]);
                worksheet.Cell(r + 2, c + 1).Value = value?.ToString() ?? string.Empty;
            }
        }

        worksheet.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return Task.FromResult(ms.ToArray());
    }

    public Task DownloadAsync(string fileName, byte[] bytes, string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CancellationToken ct = default)
    {
        var base64 = Convert.ToBase64String(bytes);

        return _js.InvokeVoidAsync("nominaDownloadFile", fileName, base64, contentType).AsTask();
    }
}
