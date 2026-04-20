using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RepairDesk.Models;
using YourApp.Services;
using Microsoft.Data.Sqlite;

namespace RepairDesk.ViewModels;

public class DeleteOrderViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<Orders> Orders { get; set; } = new();

    private Orders? _selectedOrder;
    public Orders? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
        }
    }

    public DeleteOrderViewModel()
    {
        LoadOrders();
    }

    public void LoadOrders()
    {
        Orders.Clear();

        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Repairs";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Orders.Add(new Orders
            {
                ID = reader.GetInt32(0),
                DeviceType = reader.GetString(1),
                Brand = reader.GetString(2),
                ClientFullName = reader.GetString(3),
                PhoneNumber = reader.GetString(4),
                Model = reader.GetString(5),
                SerialNumber = reader.GetString(6),
                ProblemDescription = reader.GetString(7),
                StartDate = reader.GetString(8),
                EndDate = reader.GetString(9),
                RepairsStatus = reader.GetString(10),
                Order_Number  = reader.GetHashCode()
                
            });
        }
        Console.WriteLine("Orders loaded" + " " + Orders.Count);
    }

    public void DeleteSelected()
    {
        if (SelectedOrder == null)
            return;

        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Repairs WHERE ID = $id";
        cmd.Parameters.AddWithValue("$id", SelectedOrder.ID);

        cmd.ExecuteNonQuery();

        LoadOrders();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Refresh()
    {
        LoadOrders();
    }
}