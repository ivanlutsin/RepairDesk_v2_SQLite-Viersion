using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairDesk.Models;
using YourApp.Services;
using Microsoft.Data.Sqlite;
using RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

namespace RepairDesk.Views.AdminFunction.Additional_window;

public partial class AddOrderWindow : UserControl
{
    private readonly DatabaseService _db;
    private readonly AdminOrders _parent;
    
    // Данные для ComboBox
    private List<DeviceItem> _allDevices;
    
    private ObservableCollection<string> _deviceTypes;
    public ObservableCollection<string> DeviceTypes
    {
        get => _deviceTypes;
        set => _deviceTypes = value;
    }
    
    private ObservableCollection<string> _brands;
    public ObservableCollection<string> Brands
    {
        get => _brands;
        set => _brands = value;
    }
    
    private ObservableCollection<string> _models;
    public ObservableCollection<string> Models
    {
        get => _models;
        set => _models = value;
    }
    
    private string _selectedDeviceType;
    public string SelectedDeviceType
    {
        get => _selectedDeviceType;
        set
        {
            _selectedDeviceType = value;
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
            UpdateModels();
        }
    }
    
    private string _selectedModel;
    public string SelectedModel
    {
        get => _selectedModel;
        set => _selectedModel = value;
    }

    public AddOrderWindow(DatabaseService db, AdminOrders parent)
    {
        InitializeComponent();
        _db = db;
        _parent = parent;
        
        DataContext = this;
        
        LoadDevices();
        UpdateDeviceTypes();
        
        // Подписываемся на события ComboBox
        TypeComboBox.SelectionChanged += (s, e) => SelectedDeviceType = TypeComboBox.SelectedItem as string;
        BrandComboBox.SelectionChanged += (s, e) => SelectedBrand = BrandComboBox.SelectedItem as string;
        ModelComboBox.SelectionChanged += (s, e) => SelectedModel = ModelComboBox.SelectedItem as string;
    }
    
    private void LoadDevices()
    {
        _allDevices = new List<DeviceItem>();
        using var connection = _db.GetConnection();
        connection.Open();
        
        var query = "SELECT ID, Device_Type, Brand, Model FROM Devices";
        using var cmd = new SqliteCommand(query, connection);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            _allDevices.Add(new DeviceItem
            {
                ID = reader.GetInt32(0),
                Device_Type = reader.GetString(1),
                Brand = reader.GetString(2),
                Model = reader.GetString(3)
            });
        }
    }
    
    private void UpdateDeviceTypes()
    {
        DeviceTypes = new ObservableCollection<string>(
            _allDevices.Select(d => d.Device_Type).Distinct().OrderBy(t => t)
        );
        TypeComboBox.ItemsSource = DeviceTypes;
    }
    
    private void UpdateBrands()
    {
        if (string.IsNullOrEmpty(SelectedDeviceType))
        {
            Brands = new ObservableCollection<string>();
        }
        else
        {
            Brands = new ObservableCollection<string>(
                _allDevices
                    .Where(d => d.Device_Type == SelectedDeviceType)
                    .Select(d => d.Brand)
                    .Distinct()
                    .OrderBy(b => b)
            );
        }
        BrandComboBox.ItemsSource = Brands;
        SelectedBrand = null;
        Models = new ObservableCollection<string>();
        ModelComboBox.ItemsSource = Models;
    }
    
    private void UpdateModels()
    {
        if (string.IsNullOrEmpty(SelectedDeviceType) || string.IsNullOrEmpty(SelectedBrand))
        {
            Models = new ObservableCollection<string>();
        }
        else
        {
            Models = new ObservableCollection<string>(
                _allDevices
                    .Where(d => d.Device_Type == SelectedDeviceType && d.Brand == SelectedBrand)
                    .Select(d => d.Model)
                    .Distinct()
                    .OrderBy(m => m)
            );
        }
        ModelComboBox.ItemsSource = Models;
        SelectedModel = null;
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(SelectedDeviceType) ||
            string.IsNullOrEmpty(SelectedBrand) ||
            string.IsNullOrEmpty(SelectedModel))
        {
            Console.WriteLine("Заполните все поля устройства!");
            return;
        }
        
        using var connection = _db.GetConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText =
            @"
INSERT INTO Repairs
(DeviceType, Brand, ClientFullName, Model, SerialNumber,
 ProblemDescription, StartDate, EndDate,
 RepairsStatus, Order_Number, PhoneNumber)
VALUES
($device, $brand, $client, $model, $serial,
 $problem, $start, $end,
 $status, $order, $phone)
";

        cmd.Parameters.AddWithValue("$device", SelectedDeviceType);
        cmd.Parameters.AddWithValue("$brand", SelectedBrand);
        cmd.Parameters.AddWithValue("$client", ClientNameBox.Text);
        cmd.Parameters.AddWithValue("$model", SelectedModel);
        cmd.Parameters.AddWithValue("$serial", SerialBox.Text);
        cmd.Parameters.AddWithValue("$problem", ProblemBox.Text);
        cmd.Parameters.AddWithValue("$start", DateTime.Now.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$end", string.IsNullOrWhiteSpace(EndDateBox.Text) ? "" : EndDateBox.Text);
        cmd.Parameters.AddWithValue("$status", "Новый");
        cmd.Parameters.AddWithValue("$order", DateTime.Now.Ticks);
        cmd.Parameters.AddWithValue("$phone", PhoneBox.Text);

        cmd.ExecuteNonQuery();

        var idCmd = connection.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid();";
        var orderId = (long)idCmd.ExecuteScalar();

        _parent.ReloadOrders();

        var window = new OrderCreatedWindow(orderId);
        window.Show();
    }

    private void RefreshClick(object? sender, RoutedEventArgs e)
    {
        TypeComboBox.SelectedItem = null;
        BrandComboBox.SelectedItem = null;
        ModelComboBox.SelectedItem = null;
        ClientNameBox.Text = "";
        SerialBox.Text = "";
        EndDateBox.Text = "";
        ProblemBox.Text = ""; 
        PhoneBox.Text = "";
    }
}