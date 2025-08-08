namespace HospitalManagement.Models
{
    public class SupplierDriver : UserBaseModel
    {
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public string DriverLicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }

        public bool IsAvailable { get; set; }
    }
}
