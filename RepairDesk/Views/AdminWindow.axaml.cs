using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairDesk.Models;
using RepairDesk.Views.AdminFunction;

namespace RepairDesk.Views;

public partial class AdminWindow : Window
{
    private AdminOrders _adminOrdersPage;
    private Store _storePage;
    private Engineers _engineersPage;
    private Devices _devicesPage;
    private Finance _financePage;
    private Notes_Phone _notesPhonePage;

    public AdminWindow()
    {
        InitializeComponent();
    }
    public AdminWindow(User user)
    {
        InitializeComponent();
        Console.WriteLine($"[AdminWindow] Opened for: {user.FullName}");
        SetDefaultContent();
    }
    
    private void SetDefaultContent()
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new TextBlock 
            { 
                Text = "Выберите раздел",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
    
    private void AdminOrderClick(object sender, RoutedEventArgs e)
    {
        if (_adminOrdersPage == null)
            _adminOrdersPage = new AdminOrders();

        MainContentArea.Content = _adminOrdersPage;
    }
    
    private void StoreClick(object sender, RoutedEventArgs e)
    {
        if (_storePage == null)
            _storePage = new Store();

        MainContentArea.Content = _storePage;
    }
    
    private void EngenierClick(object sender, RoutedEventArgs e)
    {
        if (_engineersPage == null)
            _engineersPage = new Engineers();

        MainContentArea.Content = _engineersPage;
    }
    
    private void DeviceClick(object sender, RoutedEventArgs e)
    {
        if (_devicesPage == null)
            _devicesPage = new Devices();

        MainContentArea.Content = _devicesPage;
    }
    
    private void FinanceClick(object sender, RoutedEventArgs e)
    {
        if (_financePage == null)
            _financePage = new Finance();

        MainContentArea.Content = _financePage;
    }

    private void Notes_Phone_Click(object? sender, RoutedEventArgs e)
    {
        if (_notesPhonePage == null)
            _notesPhonePage = new Notes_Phone();

        MainContentArea.Content = _notesPhonePage;
    }
}