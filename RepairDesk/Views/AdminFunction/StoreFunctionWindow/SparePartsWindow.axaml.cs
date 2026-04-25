using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels;
using RepairDesk.Views.AdminFunction.StoreFunctionWindow.Add;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow;

public partial class SparePartsWindow : UserControl
{
    public SparePartsWindow()
    {
        InitializeComponent();
        DataContext = new SparePartsViewModel();
    }

    private void AssignClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as SparePartsViewModel;
        if (vm?.SelectedPart == null)
            return;

        var window = new AssignPartWindow(vm.SelectedPart);
        window.Show();
    }
    

    private void DeleteClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SparePartsViewModel vm)
            vm.DeleteSelectedPart();
    }

    private void AddProductClick(object? sender, RoutedEventArgs e)
    {
        var window = new addproductWindow();
        window.Show();
    }

    private void RefreshList(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SparePartsViewModel vm)
            vm.LoadParts();
    }
}