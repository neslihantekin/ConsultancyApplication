using ClosedXML.Excel;
using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Web.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace ConsultancyApplication.Web.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConsumptionService _consumptionService;
    private readonly ICurrentEndexesService _currentEndexesService;
    private readonly IEndOfMonthEndexesService _endOfMonthEndexesService;
    private readonly UserSession _userSession;

    public HomeController(ILogger<HomeController> logger, IConsumptionService consumptionService, ICurrentEndexesService currentEndexesService, IEndOfMonthEndexesService endOfMonthEndexesService, UserSession userSession)
    {
        _logger = logger;
        _consumptionService = consumptionService;
        _currentEndexesService = currentEndexesService;
        _endOfMonthEndexesService = endOfMonthEndexesService;
        _userSession = userSession;
    }
    public async Task<IActionResult> Index()
    {
        EndexViewModel viewModel = new EndexViewModel();

        await FillCurrentEndexesAsync(viewModel); // ⬅️ ViewModel'e günlük veriler buradan ekleniyor     
        await FillInvoiceConsumptionAsync(viewModel);

        ViewBag.Title = _userSession.Title;
        viewModel.InstalledPower = _userSession.InstalledPower;
        return View(viewModel);
    }
    public async Task<IActionResult> GetEndexes(string type = "monthly", string? startDate = null, string? endDate = null)
    {
        var ownerSerno = _userSession.OwnerSerno;
        var definitionType = _userSession.DefinitionType;
        var endexDirection = 0; //Tüketim

        startDate ??= "20240101000000";
        endDate ??= "20241231235959";

        // ISO format kontrolü
        if (startDate.Contains("T"))
            startDate = DateTime.ParseExact(startDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture).ToString("yyyyMMddHHmmss");

        if (endDate.Contains("T"))
            endDate = DateTime.ParseExact(endDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture).ToString("yyyyMMddHHmmss");

        // Tarihleri DateTime'e çeviriyoruz
        DateTime parsedStartDate = DateTime.ParseExact(startDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        DateTime parsedEndDate = DateTime.ParseExact(endDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        var viewModel = new EndexViewModel { Type = type };

        await FillCurrentEndexesAsync(viewModel);
        await FillInvoiceConsumptionAsync(viewModel);

        switch (type.ToLower())
        {
            case "hourly":
                var hourlyStart = parsedStartDate.AddHours(-1);
                var hourlyRaw = (await _currentEndexesService.GetCurrentEndexesAsync(hourlyStart.ToString("yyyyMMddHHmmss"), endDate, endexDirection))?.OrderBy(x => x.EndexDate).ToList();

                var filteredHourly = hourlyRaw.Where(x => x.EndexDate >= parsedStartDate).ToList();
                viewModel.Labels = filteredHourly.Select(x => x.EndexDate?.ToString("HH:mm")).ToList();

                var hourlyConsumption = new List<decimal>();
                for (int i = 1; i < hourlyRaw.Count; i++)
                {
                    var diff = hourlyRaw[i].TSum - hourlyRaw[i - 1].TSum;
                    if (hourlyRaw[i].EndexDate >= parsedStartDate)
                        hourlyConsumption.Add(Math.Max(0, diff));
                }
                viewModel.Data = hourlyConsumption;

                var validHourly = filteredHourly.Where(x => x.TSum != 0 && x.ReactiveCapasitive != 0 && x.ReactiveInductive != 0 && (x.TSum - x.ReactiveCapasitive) != 0 && (x.TSum - x.ReactiveInductive) != 0).ToList();

                viewModel.ReactiveInductiveRatio = validHourly.Any() ? validHourly.Average(x => (x.ReactiveInductive / (x.TSum - x.ReactiveCapasitive)) * 100) : 0;
                viewModel.ReactiveCapacitiveRatio = validHourly.Any() ? validHourly.Average(x => (x.ReactiveCapasitive / (x.TSum - x.ReactiveInductive)) * 100) : 0;

                break;

            case "monthly":
                var monthlyStart = parsedStartDate.AddMonths(-1);
                var monthlyRaw = (await _endOfMonthEndexesService.GetEndOfMonthEndexesAsync(monthlyStart.ToString("yyyyMMddHHmmss"), endDate, endexDirection))?.OrderBy(x => x.EndexDate).ToList();

                var filteredMonthly = monthlyRaw.Where(x => x.EndexDate >= parsedStartDate).ToList();
                viewModel.Labels = filteredMonthly.Select(x => $"{x.EndexMonth}/{x.EndexYear}").ToList();

                var monthlyConsumption = new List<decimal>();
                for (int i = 1; i < monthlyRaw.Count; i++)
                {
                    var diff = monthlyRaw[i].TSum - monthlyRaw[i - 1].TSum;
                    if (monthlyRaw[i].EndexDate >= parsedStartDate)
                        monthlyConsumption.Add(Math.Max(0, diff));
                }
                viewModel.Data = monthlyConsumption;

                var validMonthly = filteredMonthly.Where(x => (x.TSum != 0 && x.ReactiveCapasitive != 0 && x.ReactiveInductive != 0)
                && (x.TSum - x.ReactiveCapasitive) != 0 && (x.TSum - x.ReactiveInductive) != 0).ToList();

                viewModel.ReactiveInductiveRatio = validMonthly.Any() ? validMonthly.Average(x => (x.ReactiveInductive / (x.TSum - x.ReactiveCapasitive)) * 100) : 0;
                viewModel.ReactiveCapacitiveRatio = validMonthly.Any() ? validMonthly.Average(x => (x.ReactiveCapasitive / (x.TSum - x.ReactiveInductive)) * 100) : 0;

                break;

            case "yearly":
                var yearlyStart = parsedStartDate.AddYears(-1);
                var yearlyRaw = (await _endOfMonthEndexesService.GetEndOfMonthEndexesAsync(yearlyStart.ToString("yyyyMMddHHmmss"), endDate, endexDirection))?.OrderBy(x => x.EndexDate).ToList();

                // Yıllık farkları hesaplamak için aynı şekilde fark alacağız
                var yearlyDiffs = new List<(int Year, decimal Consumption)>();
                for (int i = 1; i < yearlyRaw.Count; i++)
                {
                    var current = yearlyRaw[i];
                    var previous = yearlyRaw[i - 1];

                    if (current.EndexDate >= parsedStartDate)
                    {
                        var diff = Math.Max(0, current.TSum - previous.TSum);
                        yearlyDiffs.Add((current.EndexDate.Value.Year, diff));
                    }
                }

                // Aynı yıla ait farkları gruplayarak topluyoruz
                var groupedYearly = yearlyDiffs.GroupBy(x => x.Year).Select(g => new { Year = g.Key.ToString(), Total = g.Sum(x => x.Consumption) }).OrderBy(g => g.Year).ToList();

                // ViewModel'e aktar
                viewModel.Labels = groupedYearly.Select(x => x.Year).ToList();
                viewModel.Data = groupedYearly.Select(x => x.Total).ToList();

                // Reaktif oranlar aynı kalabilir (varsa)
                var filteredYearly = yearlyRaw.Where(x => x.EndexDate >= parsedStartDate).ToList();
                var validYearly = filteredYearly.Where(x => (x.TSum != 0 && x.ReactiveCapasitive != 0 && x.ReactiveInductive != 0) && (x.TSum - x.ReactiveCapasitive) != 0 && (x.TSum - x.ReactiveInductive) != 0).ToList();

                viewModel.ReactiveInductiveRatio = validYearly.Any() ? validYearly.Average(x => (x.ReactiveInductive / (x.TSum - x.ReactiveCapasitive)) * 100) : 0;
                viewModel.ReactiveCapacitiveRatio = validYearly.Any() ? validYearly.Average(x => (x.ReactiveCapasitive / (x.TSum - x.ReactiveInductive)) * 100) : 0;

                break;
        }

        ViewBag.Title = _userSession.Title;
        viewModel.InstalledPower = _userSession.InstalledPower;
        viewModel.Type = type;

        TempData["ExportData"] = JsonSerializer.Serialize(viewModel);

        return View("Index", viewModel);
    }
    private async Task FillCurrentEndexesAsync(EndexViewModel viewModel)
    {
        var startDate = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss");
        var endDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        var endexDirection = 0; //Tüketim          

        var dailyData = await _currentEndexesService.GetCurrentEndexesAsync(startDate, endDate, endexDirection);

        if (dailyData != null && dailyData.Any())
        {
            var latest = dailyData.OrderByDescending(x => x.EndexDate).FirstOrDefault();

            viewModel.TotalConsumption = latest?.TSum ?? 0;
            viewModel.LastReadDate = latest?.EndexDate ?? DateTime.MinValue;
            viewModel.DayConsumption = latest?.T1Endex ?? 0;
            viewModel.PuantConsumption = latest?.T2Endex ?? 0;
            viewModel.NightConsumption = latest?.T3Endex ?? 0;
            viewModel.DemandValue = latest?.MaxDemand ?? 0;
            viewModel.DemandDate = latest?.DemandDate ?? DateTime.MinValue;
            viewModel.ReactiveInductive = latest?.ReactiveInductive ?? 0;
            viewModel.ReactiveCapacitive = latest?.ReactiveCapasitive ?? 0;

            // latest tarihli verilerden valid kayıtları al (aynı gün)
            var validYearly = dailyData
                .Where(x => x.EndexDate.Value.Date == latest?.EndexDate.Value.Date &&
                    x.TSum != 0 &&
                    x.ReactiveCapasitive != 0 &&
                    x.ReactiveInductive != 0 &&
                    (x.TSum - x.ReactiveCapasitive) != 0 &&
                    (x.TSum - x.ReactiveInductive) != 0
                ).ToList();

            if (validYearly.Any())
            {
                viewModel.CurrentReactiveInductiveRatio = validYearly.Average(x => (x.ReactiveInductive / (x.TSum - x.ReactiveCapasitive)) * 100);
                viewModel.CurrentReactiveCapacitiveRatio = validYearly.Average(x => (x.ReactiveCapasitive / (x.TSum - x.ReactiveInductive)) * 100);
            }
            else
            {
                viewModel.CurrentReactiveInductiveRatio = 0;
                viewModel.CurrentReactiveCapacitiveRatio = 0;
            }
        }
    }
    private async Task FillInvoiceConsumptionAsync(EndexViewModel viewModel)
    {
        var now = DateTime.Now;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var previousMonthEnd = currentMonthStart.AddDays(-1); // önceki ayın son günü

        int endexDirection = 0;

        // 🔹 Önceki ayın son verisi (fatura başlangıcı)
        var prevMonthData = await _currentEndexesService.GetCurrentEndexesAsync(previousMonthStart.ToString("yyyyMMddHHmmss"), previousMonthEnd.ToString("yyyyMMdd") + "235959", endexDirection);

        var start = prevMonthData?.OrderByDescending(x => x.EndexDate).FirstOrDefault();

        // 🔸 Bu ayın son verisi (fatura bitişi)
        var currentMonthData = await _currentEndexesService.GetCurrentEndexesAsync(currentMonthStart.ToString("yyyyMMddHHmmss"), now.ToString("yyyyMMddHHmmss"), endexDirection);

        var end = currentMonthData?.OrderByDescending(x => x.EndexDate).FirstOrDefault();

        if (start != null && end != null)
        {
            viewModel.InvoiceStartDate = start.EndexDate ?? previousMonthEnd;
            viewModel.InvoiceEndDate = end.EndexDate ?? now;

            viewModel.InvoiceTotalKwh = end.TSum - start.TSum;
            viewModel.InvoiceT1Kwh = end.T1Endex - start.T1Endex;
            viewModel.InvoiceT2Kwh = end.T2Endex - start.T2Endex;
            viewModel.InvoiceT3Kwh = end.T3Endex - start.T3Endex;
            viewModel.InvoiceReactiveInductive = end.ReactiveInductive - start.ReactiveInductive;
            viewModel.InvoiceReactiveCapacitive = end.ReactiveCapasitive - start.ReactiveCapasitive;
            viewModel.InvoiceDemandValue = end.MaxDemand;
            viewModel.InvoiceDemandDate = end.DemandDate ?? DateTime.MinValue;
        }
        else
        {
            viewModel.InvoiceStartDate = previousMonthEnd;
            viewModel.InvoiceEndDate = now;

            viewModel.InvoiceTotalKwh = 0;
            viewModel.InvoiceT1Kwh = 0;
            viewModel.InvoiceT2Kwh = 0;
            viewModel.InvoiceT3Kwh = 0;
            viewModel.InvoiceReactiveInductive = 0;
            viewModel.InvoiceReactiveCapacitive = 0;
            viewModel.InvoiceDemandValue = 0;
            viewModel.InvoiceDemandDate = DateTime.MinValue;
        }
    }

    /* 
    public IActionResult ExportToExcel()
    {     
        if (!TempData.ContainsKey("ExportData"))
            return RedirectToAction("Index");

        var jsonData = TempData["ExportData"] as string;
        var viewModel = JsonSerializer.Deserialize<EndexViewModel>(jsonData);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tüketim Detayları");

        // Başlıklar
        string[] headers = new[]
        {
        "Abone", "Ünvan", "Endeks Tarihi", "T Toplam", "T1", "T2", "T3",
        "Demand", "Demand Tarihi", "RI", "RC",
        "Akım 1", "Akım 2", "Akım 3", "Gerilim 1", "Gerilim 2", "Gerilim 3"
    };

        for (int i = 0; i < headers.Length; i++)
            worksheet.Cell(1, i + 1).Value = headers[i];

        // Veri satırları
        for (int i = 0; i < viewModel.DailyData.Count; i++)
        {
            var item = viewModel.DailyData[i];
            int row = i + 2;

            worksheet.Cell(row, 1).Value = _userSession.Username;
            worksheet.Cell(row, 2).Value = _userSession.Title;
            worksheet.Cell(row, 3).Value = item.EndexDate?.ToString("dd.MM.yyyy HH:mm");
            worksheet.Cell(row, 4).Value = item.TSum;
            worksheet.Cell(row, 5).Value = item.T1Endex;
            worksheet.Cell(row, 6).Value = item.T2Endex;
            worksheet.Cell(row, 7).Value = item.T3Endex;
            worksheet.Cell(row, 8).Value = item.MaxDemand;
            worksheet.Cell(row, 9).Value = item.DemandDate?.ToString("dd.MM.yyyy HH:mm");
            worksheet.Cell(row, 10).Value = item.ReactiveInductive;
            worksheet.Cell(row, 11).Value = item.ReactiveCapasitive;
            //worksheet.Cell(row, 12).Value = item.ReactiveInductiveExported;   // varsa
            //worksheet.Cell(row, 13).Value = item.ReactiveCapasitiveExported;  // varsa
            //worksheet.Cell(row, 14).Value = item.TSumExported;                // varsa
            //worksheet.Cell(row, 15).Value = item.T1EndexExported;             // varsa
            //worksheet.Cell(row, 16).Value = item.T2EndexExported;             // varsa
            //worksheet.Cell(row, 17).Value = item.T3EndexExported;             // varsa
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TuketimDetaylari.xlsx");
         
    }    */
    public IActionResult ExportToExcel()
    {
        if (!TempData.ContainsKey("ExportData"))
            return RedirectToAction("Index");

        var jsonData = TempData["ExportData"] as string;
        var viewModel = JsonSerializer.Deserialize<EndexViewModel>(jsonData);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tüketim Verileri");

        worksheet.Cell(1, 1).Value = "Tarih";
        worksheet.Cell(1, 2).Value = "Tüketim (kWh)";

        for (int i = 0; i < viewModel.Labels.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = viewModel.Labels[i];
            worksheet.Cell(i + 2, 2).Value = viewModel.Data[i];
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TuketimVerileri.xlsx");

    }
    public IActionResult ExportToPdf()
    {
        if (!TempData.ContainsKey("ExportData"))
            return RedirectToAction("Index");

        var jsonData = TempData["ExportData"] as string;
        var viewModel = JsonSerializer.Deserialize<EndexViewModel>(jsonData);

        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 12);

        gfx.DrawString("Tüketim Verileri", font, XBrushes.Black, new XPoint(30, 30));

        for (int i = 0; i < viewModel.Labels.Count; i++)
        {
            gfx.DrawString($"{viewModel.Labels[i]} - {viewModel.Data[i]} kWh", font, XBrushes.Black, new XPoint(30, 60 + (i * 20)));
        }

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return File(stream.ToArray(), "application/pdf", "TuketimVerileri.pdf");
    }


    public IActionResult Privacy()
    {
        return View(new EndexViewModel());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
