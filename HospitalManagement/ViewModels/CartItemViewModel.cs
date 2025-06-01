using System.ComponentModel.DataAnnotations;

public class CartItemViewModel
{
    [Required(ErrorMessage = "Menu item Id is required")]
    public int MenuItemId { get; set; }

    [Required(ErrorMessage = "Menu item name is required")]
    public string MenuItemName { get; set; }

    [Required(ErrorMessage = "Menu item price is required")]
    public decimal MenuItemPrice { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Subtotal is required")]

    public decimal Subtotal { get; set; }

    public bool Deleted { get; set; }
}
