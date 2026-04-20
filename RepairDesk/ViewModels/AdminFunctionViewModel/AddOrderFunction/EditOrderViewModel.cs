using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

public class EditOrderViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    private readonly DatabaseService _db = new();

    public Orders? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedStatus));
        }
    }
    
    
    public List<string> Status { get; } = new()
    {
        "Новый",
        "В работе",
        "Готов",
        "Выдан",
        "Отменён",
    };
    
    
    public string SelectedStatus
    {
        get => SelectedOrder?.RepairsStatus;
        set
        {
            if (SelectedOrder != null)
            {
                SelectedOrder.RepairsStatus = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Orders> Orders { get; set; } = new ();

    private Orders? _selectedOrder;


    public EditOrderViewModel()
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
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                ClientFullName = reader.GetString(reader.GetOrdinal("ClientFullName")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                Brand = reader.GetString(reader.GetOrdinal("Brand")),
                Model = reader.GetString(reader.GetOrdinal("Model")),
                ProblemDescription = reader.GetString(reader.GetOrdinal("ProblemDescription")),
                RepairsStatus = reader.GetString(reader.GetOrdinal("RepairsStatus")),
            });
        }
        Console.WriteLine(Orders.Count);
        Console.WriteLine("Status" + SelectedOrder?.RepairsStatus);
    }

    public void Save()
    {
        if (SelectedOrder == null)
            return;

        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
            @"
         UPDATE Repairs
         SET
             ClientFullName = $client,
             PhoneNumber = $phone,
             DeviceType = $device,
             Brand = $brand,
             Model = $model,
             ProblemDescription = $problem,
             RepairsStatus = $status
         WHERE ID = $id
         ";

        cmd.Parameters.AddWithValue("$client", SelectedOrder.ClientFullName);
        cmd.Parameters.AddWithValue("$phone", SelectedOrder.PhoneNumber);
        cmd.Parameters.AddWithValue("$device", SelectedOrder.DeviceType);
        cmd.Parameters.AddWithValue("$brand", SelectedOrder.Brand);
        cmd.Parameters.AddWithValue("$model", SelectedOrder.Model);
        cmd.Parameters.AddWithValue("$problem", SelectedOrder.ProblemDescription);
        cmd.Parameters.AddWithValue("$id", SelectedOrder.ID);
        cmd.Parameters.AddWithValue("$status", SelectedOrder.RepairsStatus);

        cmd.ExecuteNonQuery();
    }
}
     