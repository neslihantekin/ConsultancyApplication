namespace ConsultancyApplication.Core.Entities
{
    public class ClientCredential
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PortalUsername { get; set; }
        public string PortalPassword { get; set; }
        public string AppPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
