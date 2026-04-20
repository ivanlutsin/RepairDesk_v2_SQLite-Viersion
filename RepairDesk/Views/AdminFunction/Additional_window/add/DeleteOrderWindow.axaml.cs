using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels;

namespace RepairDesk.Views.AdminFunction.Additional_window.add;

public partial class DeleteOrderWindow : UserControl
{
    public DeleteOrderWindow()
    {
        InitializeComponent();
    }

    private void DeleteClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DeleteOrderViewModel vm)
        {
            vm.DeleteSelected();
        }
    }

    private void UpdateBTN(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DeleteOrderViewModel vm)
            vm.Refresh();
    }
}