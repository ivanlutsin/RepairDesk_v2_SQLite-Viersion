using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels.AdminFunctionViewModel;
using RepairDesk.Models;

namespace RepairDesk.Views.AdminFunction;

public partial class Devices : UserControl
{
    public Devices()
    {
        InitializeComponent();
        DataContext = new DevicesViewModel();
    }

    private void DeleteDevice_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var device = button?.CommandParameter as RepairDesk.Models.DeviceItem;
        
        if (device == null) return;
        
        var vm = DataContext as DevicesViewModel;
        if (vm == null) return;
        
        // Удаляем через метод ViewModel
        vm.DeleteDeviceDirect(device);
    }
}