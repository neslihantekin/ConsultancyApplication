using ConsultancyApplication.Core.Entities;
         
public class EndexViewModel
{
    public string Type { get; set; } // daily, monthly, yearly

    public List<string> Labels { get; set; } = new();
    public List<decimal> Data { get; set; } = new();

    public List<CurrentEndexes> DailyData { get; set; } = new();
    public List<EndOfMonthEndexes> MonthlyData { get; set; } = new();
    public List<Consumption> YearlyData { get; set; } = new();

    // Gauge göstergeler için oranlar
    public decimal ReactiveInductiveRatio { get; set; }
    public decimal ReactiveCapacitiveRatio { get; set; }

    // Güncel veriler (tablo için doğrudan erişim)
    public decimal TotalConsumption { get; set; }
    public decimal DayConsumption { get; set; }
    public decimal PuantConsumption { get; set; }
    public decimal NightConsumption { get; set; }
    public decimal ReactiveInductive { get; set; }
    public decimal ReactiveCapacitive { get; set; }
    public decimal DemandValue { get; set; }
    public DateTime DemandDate { get; set; }
    public DateTime LastReadDate { get; set; }
    public decimal CurrentReactiveInductiveRatio { get; set; }
    public decimal CurrentReactiveCapacitiveRatio { get; set; }
    public decimal InstalledPower { get; set; }

    // Fatura tüketimi (güncel aydaki geçen gün kadar)
    public decimal InvoiceTotalKwh { get; set; }
    public decimal InvoiceT1Kwh { get; set; }
    public decimal InvoiceT2Kwh { get; set; }
    public decimal InvoiceT3Kwh { get; set; }
    public decimal InvoiceReactiveInductive { get; set; }
    public decimal InvoiceReactiveCapacitive { get; set; }
    public decimal InvoiceDemandValue { get; set; }
    public DateTime InvoiceDemandDate { get; set; }
    public DateTime InvoiceStartDate { get; set; }
    public DateTime InvoiceEndDate { get; set; }

}
