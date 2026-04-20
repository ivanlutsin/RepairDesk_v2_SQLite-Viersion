using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

namespace RepairDesk.Views.AdminFunction.Additional_window;

public partial class EditOrderWindow : UserControl
{
    public EditOrderWindow()
    {
        InitializeComponent();
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is EditOrderViewModel vm)
        {
            vm.Save();
        }
    }
}