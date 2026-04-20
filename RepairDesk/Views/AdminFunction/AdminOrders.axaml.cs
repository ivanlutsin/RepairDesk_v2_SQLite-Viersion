using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;
using RepairDesk.Models;
using RepairDesk.Services;
using RepairDesk.Views.AdminFunction.Additional_window;
using RepairDesk.Views.AdminFunction.Additional_window.add;
using YourApp.Services;

namespace RepairDesk.Views.AdminFunction;


public partial class AdminOrders : UserControl
{
    private AddOrderWindow _addOrderWindow;
    private EditOrderWindow _editOrderWindow;
    private DeleteOrderWindow _deleteOrderWindow;
    private StatusWorkWindow _statusWorkWindow;
    
    private void SetDefaultContent()
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new TextBlock 
            { 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
    public AdminOrders()
    {
        InitializeComponent(); 
        DataContext = this;
        SetDefaultContent();
        LoadOrders();
    }
    private readonly DatabaseService _db = new DatabaseService();
    public ObservableCollection<Orders> Orders { get; set; } = new();
    
    private void LoadOrders()
    {
        Orders.Clear();
        
        using var connection = _db.GetConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Repairs";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            Orders.Add(new Orders()
            {
                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                DeviceType = reader.GetString(reader.GetOrdinal("DeviceType")),
                Brand = reader.GetString(reader.GetOrdinal("Brand")),
                ClientFullName = reader.GetString(reader.GetOrdinal("ClientFullName")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                Model = reader.GetString(reader.GetOrdinal("Model")),

                SerialNumber = reader.IsDBNull(reader.GetOrdinal("SerialNumber")) 
                    ? "" 
                    : reader.GetString(reader.GetOrdinal("SerialNumber")),

                ProblemDescription = reader.IsDBNull(reader.GetOrdinal("ProblemDescription")) 
                    ? "" 
                    : reader.GetString(reader.GetOrdinal("ProblemDescription")),

                StartDate = reader.GetString(reader.GetOrdinal("StartDate")),

                EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) 
                    ? "" 
                    : reader.GetString(reader.GetOrdinal("EndDate")),

                RepairsStatus = reader.IsDBNull(reader.GetOrdinal("RepairsStatus")) 
                    ? "" 
                    : reader.GetString(reader.GetOrdinal("RepairsStatus")),

                Order_Number = reader.GetInt64(reader.GetOrdinal("Order_Number"))
            });
        }
    }
    public void ReloadOrders()
    {
        LoadOrders();
    }

    #region clicks_perechod
    private void AddOrderClick(object? sender, RoutedEventArgs e)
    {
        if (_addOrderWindow == null)
            _addOrderWindow = new AddOrderWindow(_db, this);
        
        MainContentArea.Content = _addOrderWindow;
    }

    private void DeleteOrderClick(object? sender, RoutedEventArgs e)
    {
        if (_deleteOrderWindow == null)
            _deleteOrderWindow = new DeleteOrderWindow();
        
        MainContentArea.Content = _deleteOrderWindow;
    }

    private void EditOrderClick(object? sender, RoutedEventArgs e)
    {
        if (_editOrderWindow == null)
            _editOrderWindow = new EditOrderWindow();
        
        MainContentArea.Content = _editOrderWindow;
    }
    
    private void StatusWorkClick(object? sender, RoutedEventArgs e)
    {
        if (_statusWorkWindow == null)
            _statusWorkWindow = new StatusWorkWindow();
        
        MainContentArea.Content = _statusWorkWindow;
    }
    
    
    #endregion


    
}

