using System.Collections.ObjectModel;
using System.ComponentModel;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.StoreFunctionViewModel.Add;

public class AssignPartViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<Orders> Orders { get; set; } = new();

    public Orders? SelectedOrder { get; set; }

    public RepairParts Part { get; }

    public AssignPartViewModel(RepairParts part)
    {
        Part = part;
        LoadOrders();
    }

    private void LoadOrders()
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            @"
SELECT ID, ClientFullName, DeviceType, Model, RepairsStatus
FROM Repairs
WHERE RepairsStatus = 'В работе'
";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Orders.Add(new Orders
            {
                ID = reader.GetInt32(0),
                ClientFullName = reader.GetString(1),
                DeviceType = reader.GetString(2),
                Model = reader.GetString(3),
                RepairsStatus = reader.GetString(4)
            });
        }
    }

    public void Assign()
    {
        if (SelectedOrder == null)
            return;

        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            @"
        UPDATE RepairParts
        SET RepairID = $repairId
        WHERE ID = $partId
        ";

        cmd.Parameters.AddWithValue("$repairId", SelectedOrder.ID);
        cmd.Parameters.AddWithValue("$partId", Part.ID);

        cmd.ExecuteNonQuery();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}