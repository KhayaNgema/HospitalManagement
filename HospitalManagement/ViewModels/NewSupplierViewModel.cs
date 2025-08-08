using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class NewSupplierViewModel
    {
        public string SupplierName { get; set; }

        public string DivisionDescription { get; set; }

        public string StreetLine1 { get; set; }

        public string StreetLine2 { get; set; }

        public string Suburb { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public Province Province { get; set; }

        public IFormFile SupplierBadges { get; set; }
    }
}
