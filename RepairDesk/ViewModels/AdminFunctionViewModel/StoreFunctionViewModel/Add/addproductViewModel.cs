using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.StoreFunctionViewModel;

public class addproductViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _db = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- ПОЛЯ ---

    private string _partName = "";
    public string PartName
    {
        get => _partName;
        set { _partName = value; OnPropertyChanged(); }
    }

    private string _barcode = "";
    public string Barcode
    {
        get => _barcode;
        set { _barcode = value; OnPropertyChanged(); }
    }

    private double _price;
    public double Price
    {
        get => _price;
        set { _price = value; OnPropertyChanged(); }
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    private string _manufacturer = "";
    public string Manufacturer
    {
        get => _manufacturer;
        set { _manufacturer = value; OnPropertyChanged(); }
    }

    private string _type = "";
    public string Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    private string _forDevice = "";
    public string ForDevice
    {
        get => _forDevice;
        set { _forDevice = value; OnPropertyChanged(); }
    }

    // --- СОХРАНЕНИЕ ---

    public void Save()
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"
        INSERT INTO RepairParts
        (PartName, Barcode, Price, Quantity, Manufacturer, Type, ForDevice)
        VALUES ($name, $barcode, $price, $qty, $man, $type, $device)
        ";

        cmd.Parameters.AddWithValue("$name", PartName);
        cmd.Parameters.AddWithValue("$barcode", Barcode);
        cmd.Parameters.AddWithValue("$price", Price);
        cmd.Parameters.AddWithValue("$qty", Quantity);
        cmd.Parameters.AddWithValue("$man", Manufacturer);
        cmd.Parameters.AddWithValue("$type", Type);
        cmd.Parameters.AddWithValue("$device", ForDevice);

        cmd.ExecuteNonQuery();
        
        ClearFields();
    }

    private void ClearFields()
    {
        PartName = "";
        Barcode = "";
        Price = 0;
        Quantity = 0;
        Manufacturer = "";
        Type = "";
        ForDevice = "";
    }
}