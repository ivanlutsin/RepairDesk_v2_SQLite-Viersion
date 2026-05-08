namespace RepairDesk.Models;

public class PhoneBookItem
{
    public int ID { get; set; }
    public string FullName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Comment { get; set; } = "";
}