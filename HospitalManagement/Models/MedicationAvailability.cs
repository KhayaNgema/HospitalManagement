using System.ComponentModel.DataAnnotations;

public enum MedicationAvailability
{
    [Display(Name = "Available")]
    Available,

    [Display(Name = "Out of Stock")]
    OutOfStock
}

