using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;
using RepairDesk.ViewModels.AdminFunctionViewModel.StoreFunctionViewModel.Add;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow.Add;

public partial class AssignPartWindow : Window
{
    private readonly RepairParts _part;

    public AssignPartWindow(RepairParts part)
    {
        InitializeComponent();

        _part = part;

        DataContext = new AssignPartViewModel(part);
    }
    
    private void AssignClick(object? sender, RoutedEventArgs e)
    { 
        if (DataContext is AssignPartViewModel vm)
        {
            vm.Assign();
            Close();
        }
    }
}