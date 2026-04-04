using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairDesk.Models;
using RepairDesk.Views.AdminFunction;

namespace RepairDesk.Views;

public partial class AdminWindow : Window
{
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
        if (MainContentArea != null)
        {
            MainContentArea.Content = new AdminOrders();
        }
    }
    
    private void StoreClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Store();
        }
    }
    
    private void EngenierClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Engineers();
        }
    }
    
    private void DeviceClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Devices();
        }
    }
    
    private void FinanceClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Finance();
        }
    }
}