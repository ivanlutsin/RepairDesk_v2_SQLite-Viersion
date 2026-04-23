namespace RepairDesk.Models;

public class RepairParts
{
    public int ID { get; set; }
    public int? RepairID { get; set; }

    public string PartName { get; set; }
    public string Barcode { get; set; }

    public double Price { get; set; }
    public int Quantity { get; set; }

    public string Manufacturer { get; set; }
    public string Type { get; set; }
    public string ForDevice { get; set; }
}