using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; }

        public string? SupplierBadge { get; set; }

        public string SupplierName { get; set; }

        public string DivisionDescription { get; set; }

        public string Address { get; set; }


        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual UserBaseModel CreatedBy { get; set; }

        public string ModifiedById { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual UserBaseModel ModifiedBy { get; set; }

        public Supplier()
        {
            SupplierBadge = "UploadedFiles/supplier_placeholder.png";
        }
    }
}

