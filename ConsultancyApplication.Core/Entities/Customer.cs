using ConsultancyApplication.Core.Utilities.Converters;
using System.Text.Json.Serialization;

namespace ConsultancyApplication.Core.Entities
{
    // "GetCustomerPortalSubscriptions" servisinin ResultList'inden gelen verileri modelleyeceğiz.
    public class Customer
    {
        public int Id { get; set; } // EF Core için Primary Key
        public long SubscriptionSerno { get; set; }
        public string IdentifierValue { get; set; }
        public string IdentifierValueSec { get; set; }
        public int DefinitionType { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string MeterSerial { get; set; }
        public string MeterBrand { get; set; }
        public decimal Multiplier { get; set; }

        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? LastEndexDate { get; set; }

        [JsonConverter(typeof(ArilDateTimeConverter))]     
        public DateTime? LastProfileDate { get; set; }
        public decimal MinCapasitiveRate { get; set; }
        public decimal MinInductiveRate { get; set; }
        public decimal InstalledPower { get; set; }
        public decimal AccordPower { get; set; }
        public string GroupInfo { get; set; }
        public string Etso { get; set; }

        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? MeterPointOwnerAssignDate { get; set; }

        [JsonConverter(typeof(ArilDateTimeConverter))]
        public DateTime? SubscriberMultiplierChangeDate { get; set; }

        //public object CustomerFields { get; set; }  // tip bilinmiyorsa 'object' kullan
    }
}
