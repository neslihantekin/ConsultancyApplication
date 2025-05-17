using ConsultancyApplication.Core.Utilities.Converters;
using System.Text.Json.Serialization;

namespace ConsultancyApplication.Core.Entities
{
    // "GetOwnerConsumptions" servisinden dönebilecek saatlik tüketim verilerini temsil eder.
    public class Consumption
    {
        public int Id { get; set; } // EF Core için Primary Key

        // Owner (tesisat) bilgileri
        public long OwnerSerno { get; set; }
        public int OwnerType { get; set; }

        // Saatlik tüketim verisi
        [JsonPropertyName("Pd")]
        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? ProfileDate { get; set; }

        [JsonPropertyName("Cn")]
        public decimal? ActiveConsumption { get; set; }

        [JsonPropertyName("Gn")]
        public decimal? ActiveGeneration { get; set; }

        [JsonPropertyName("Ri")]
        public decimal? ReactiveInductive { get; set; }

        [JsonPropertyName("Rc")]
        public decimal? ReactiveCapacitive { get; set; }

        [JsonPropertyName("Rio")]
        public decimal? ReactiveInductiveOut { get; set; }

        [JsonPropertyName("Rco")]
        public decimal? ReactiveCapacitiveOut { get; set; }

        [JsonPropertyName("Addcn")]
        public decimal? AdditionalConsumption { get; set; }

        [JsonPropertyName("Addgn")]
        public decimal? AdditionalGeneration { get; set; }

        [JsonPropertyName("Rir")]
        public decimal? ReactiveInductiveRatio { get; set; }

        [JsonPropertyName("Rcr")]
        public decimal? ReactiveCapacitiveRatio { get; set; }

        [JsonPropertyName("Riro")]
        public decimal? ReactiveInductiveOutRatio { get; set; }

        [JsonPropertyName("Rcor")]
        public decimal? ReactiveCapacitiveOutRatio { get; set; }

        [JsonPropertyName("Ml")]
        public decimal? Multiplier { get; set; }

        [JsonPropertyName("St")]
        public int? Status { get; set; }
    }
}
