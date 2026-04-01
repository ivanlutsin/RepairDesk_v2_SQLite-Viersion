using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;

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
    }
}