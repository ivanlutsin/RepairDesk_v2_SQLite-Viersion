using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels;

public class StatusViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<Orders> Orders { get; set; } = new();

    private string _selectedStatus = "Новый";

    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            OnPropertyChanged();
            LoadByStatus(); // автообновление при смене
        }
    }

    public List<string> Statuses { get; } = new()
    {
        "Новый",
        "В работе",
        "Готов",
        "Выдан",
        "Отменён"
    };

    public StatusViewModel()
    {
        LoadByStatus();
    }

    public void LoadByStatus()
    {
        Orders.Clear();

        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
            SELECT * FROM Repairs
            WHERE RepairsStatus = $status
        """;

        cmd.Parameters.AddWithValue("$status", SelectedStatus);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Orders.Add(new Orders
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                ClientFullName = reader.GetString(reader.GetOrdinal("ClientFullName")),
                DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                Model = reader.GetString(reader.GetOrdinal("Model")),
                ProblemDescription = reader.GetString(reader.GetOrdinal("ProblemDescription")),
                RepairsStatus = reader.GetString(reader.GetOrdinal("RepairsStatus"))
            });
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}