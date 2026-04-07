using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;

namespace RepairDesk.Views;

public partial class MinimumForm : Window
{
    private User _currentUser;
    private bool _shiftOpen = false;

    public MinimumForm()
    {
        InitializeComponent();
    }
    
    public MinimumForm(User user)
    {
        InitializeComponent();
        _currentUser = user;
        Console.WriteLine($"[AdminWindow] Opened for: {user.FullName}");
    }
}