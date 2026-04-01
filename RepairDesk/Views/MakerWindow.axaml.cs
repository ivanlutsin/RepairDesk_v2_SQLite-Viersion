using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;

namespace RepairDesk.Views;

public partial class MakerWindow : Window
{
    public MakerWindow()
    {
        InitializeComponent();
    }
    public MakerWindow(User user)
    {
        InitializeComponent();
        Console.WriteLine($"[MakerWindow] Opened for: {user.FullName}");
    }
}