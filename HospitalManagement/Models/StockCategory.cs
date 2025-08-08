using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class StockCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockCategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
