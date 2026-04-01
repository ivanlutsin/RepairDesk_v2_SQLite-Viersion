using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairDesk.Models;
using RepairDesk.Views.SellerFunction;

namespace RepairDesk.Views;

public partial class SellerWindow : Window
{
    private User _currentUser;
    private bool _shiftOpen = false;
    
    public SellerWindow()
    {
        InitializeComponent();
        SetDefaultContent();
    }
    
    public SellerWindow(User user)
    {
        InitializeComponent();
        _currentUser = user;
        Console.WriteLine($"[SellerWindow] Opened for: {user.FullName}");
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
    
    private void OnOrderClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new CreateNewOrder();
        }
    }
    
    private void OnCashClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Cash();
        }
    }
    
    private void OnWarrantyClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Warranty();
        }
    }
    
    private void OnSalaryClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Sallary_Calculation();
        }
    }
    
    private void OnAccountClick(object sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new TextBlock 
            { 
                Text = "Смена аккаунта",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
}