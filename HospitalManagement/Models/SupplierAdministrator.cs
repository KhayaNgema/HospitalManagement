namespace HospitalManagement.Models
{
    public class SupplierAdministrator : UserBaseModel
    {
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
