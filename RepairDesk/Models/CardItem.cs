namespace RepairDesk.Models;

public class CartItem
{
    public int ID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double Total => Quantity * Price;
}