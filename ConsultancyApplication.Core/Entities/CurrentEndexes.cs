using ConsultancyApplication.Core.Utilities.Converters;
using System.Text.Json.Serialization;

namespace ConsultancyApplication.Core.Entities
{
    // "GetCurrentEndexes" servisi ResultList'ini modelleyen sınıf.
    public class CurrentEndexes
    {
        public int Id { get; set; } // EF Core için Primary Key

        public long OwnerSerno { get; set; }  // İsteğe bağlı: Tesisatı saklamak isterseniz


        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? EndexDate { get; set; }
        public int EndexType { get; set; }
        public decimal T1Endex { get; set; }
        public decimal T2Endex { get; set; }
        public decimal T3Endex { get; set; }
        public decimal T4Endex { get; set; }
        public decimal ReactiveCapasitive { get; set; }
        public decimal ReactiveInductive { get; set; }
        public decimal MaxDemand { get; set; }

        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? DemandDate { get; set; }
        public decimal TSum { get; set; }
    }
}
