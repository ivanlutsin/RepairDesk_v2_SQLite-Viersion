namespace RepairDesk.Models;

public class Products
{
    public int ID { get; set; }
    public string ProductName { get; set; } = "";
    public string Barcode { get; set; } = "";
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string Characteristics { get; set; } = "";
}