using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    private List<DeviceItem> _allDevices = new();

    private Orders? _selectedOrder;
    public Orders? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            if (value != null)
            {
                // При выборе заказа подставляем старые данные
                SelectedDeviceType = value.DeviceType;
                SelectedBrand = value.Brand;
                SelectedModel = value.Model;
                SelectedStatus = value.RepairsStatus;
            }
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
    
    private string _selectedStatus;
    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value;
            if (SelectedOrder != null)
                SelectedOrder.RepairsStatus = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Orders> Orders { get; set; } = new();
    
    // Для ComboBox устройств
    private ObservableCollection<string> _deviceTypes = new();
    public ObservableCollection<string> DeviceTypes
    {
        get => _deviceTypes;
        set
        {
            _deviceTypes = value;
            OnPropertyChanged();
        }
    }
    
    private ObservableCollection<string> _brands = new();
    public ObservableCollection<string> Brands
    {
        get => _brands;
        set
        {
            _brands = value;
            OnPropertyChanged();
        }
    }
    
    private ObservableCollection<string> _models = new();
    public ObservableCollection<string> Models
    {
        get => _models;
        set
        {
            _models = value;
            OnPropertyChanged();
        }
    }
    
    private string _selectedDeviceType;
    public string SelectedDeviceType
    {
        get => _selectedDeviceType;
        set
        {
            _selectedDeviceType = value;
            if (SelectedOrder != null)
                SelectedOrder.DeviceType = value;
            OnPropertyChanged();
            UpdateBrands();
        }
    }
    
    private string _selectedBrand;
    public string SelectedBrand
    {
        get => _selectedBrand;
        set
        {
            _selectedBrand = value;
            if (SelectedOrder != null)
                SelectedOrder.Brand = value;
            OnPropertyChanged();
            UpdateModels();
        }
    }
    
    private string _selectedModel;
    public string SelectedModel
    {
        get => _selectedModel;
        set
        {
            _selectedModel = value;
            if (SelectedOrder != null)
                SelectedOrder.Model = value;
            OnPropertyChanged();
        }
    }

    public EditOrderViewModel()
    {
        LoadDevices();
        LoadOrders();
    }

    public void Refresh()
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
    }
    
    private void LoadDevices()
    {
        _allDevices = _db.LoadDevices().ToList();
        
        DeviceTypes = new ObservableCollection<string>(
            _allDevices.Select(d => d.Device_Type).Distinct().OrderBy(t => t)
        );
    }
    
    private void UpdateBrands()
    {
        if (string.IsNullOrEmpty(SelectedDeviceType))
        {
            Brands.Clear();
        }
        else
        {
            var brandsList = _allDevices
                .Where(d => d.Device_Type == SelectedDeviceType)
                .Select(d => d.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToList();
            
            Brands.Clear();
            foreach (var brand in brandsList)
                Brands.Add(brand);
        }
    }
    
    private void UpdateModels()
    {
        if (string.IsNullOrEmpty(SelectedDeviceType) || string.IsNullOrEmpty(SelectedBrand))
        {
            Models.Clear();
        }
        else
        {
            var modelsList = _allDevices
                .Where(d => d.Device_Type == SelectedDeviceType && d.Brand == SelectedBrand)
                .Select(d => d.Model)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
            
            Models.Clear();
            foreach (var model in modelsList)
                Models.Add(model);
        }
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
        cmd.Parameters.AddWithValue("$device", SelectedDeviceType);
        cmd.Parameters.AddWithValue("$brand", SelectedBrand);
        cmd.Parameters.AddWithValue("$model", SelectedModel);
        cmd.Parameters.AddWithValue("$problem", SelectedOrder.ProblemDescription);
        cmd.Parameters.AddWithValue("$id", SelectedOrder.ID);
        cmd.Parameters.AddWithValue("$status", SelectedStatus);

        cmd.ExecuteNonQuery();
    }
}