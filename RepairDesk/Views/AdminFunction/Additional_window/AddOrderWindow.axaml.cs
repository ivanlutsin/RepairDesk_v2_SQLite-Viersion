using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;
using YourApp.Services;

namespace RepairDesk.Views.AdminFunction.Additional_window;

public partial class AddOrderWindow : UserControl
{
    private readonly DatabaseService _db;
    private readonly AdminOrders _parent;

    public AddOrderWindow(DatabaseService db, AdminOrders parent)
    {
        InitializeComponent();
        _db = db;
        _parent = parent;
    }

    private void SaveClick(object sender, RoutedEventArgs e)
    {
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

        cmd.Parameters.AddWithValue("$device", DeviceBox.Text);
        cmd.Parameters.AddWithValue("$brand", BrandBox.Text);
        cmd.Parameters.AddWithValue("$client", ClientNameBox.Text);
        cmd.Parameters.AddWithValue("$model", ModelBox.Text);
        cmd.Parameters.AddWithValue("$serial", SerialBox.Text);
        

        cmd.Parameters.AddWithValue("$problem", ProblemBox.Text);
        cmd.Parameters.AddWithValue("$start", DateTime.Now.ToString("yyyy-MM-dd"));

        cmd.Parameters.AddWithValue("$end", string.IsNullOrWhiteSpace(EndDateBox.Text)
            ? ""
            : EndDateBox.Text);

        cmd.Parameters.AddWithValue("$status", "Новый");

        cmd.Parameters.AddWithValue("$order", DateTime.Now.Ticks);
        cmd.Parameters.AddWithValue("$phone", PhoneBox.Text);

        cmd.ExecuteNonQuery();

        var idCmd = connection.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid();";
        var orderId = (long)idCmd.ExecuteScalar();

        Console.WriteLine(_parent == null);
        _parent.ReloadOrders();

        var window = new OrderCreatedWindow(orderId);
        window.Show();
    }

    private void RefreshClick(object? sender, RoutedEventArgs e)
    {
        DeviceBox.Text = "";
        BrandBox.Text = "";
        ClientNameBox.Text = "";
        ModelBox.Text = "";
        SerialBox.Text = "";
        //PassportSeriesBox.Text = "";
        //PassportNumberBox.Text = "";
        EndDateBox.Text = "";
        ProblemBox.Text = ""; 
        PhoneBox.Text = "";
    }
}