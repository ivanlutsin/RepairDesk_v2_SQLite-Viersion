namespace RepairDesk.Models;

public class Orders
{
    public int ID { get; set; }

    public string DeviceType { get; set; }
    public string Brand { get; set; }

    public string ClientFullName { get; set; }
    public string PhoneNumber { get; set; }

    public string Model { get; set; }

    public string SerialNumber { get; set; }

    public string ProblemDescription { get; set; }

    public string StartDate { get; set; }
    public string EndDate { get; set; }

    public string RepairsStatus { get; set; }

    public long Order_Number { get; set; }
}