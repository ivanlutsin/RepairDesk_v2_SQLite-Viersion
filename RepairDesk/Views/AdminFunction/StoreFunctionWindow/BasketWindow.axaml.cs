using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow;

public partial class BasketWindow : UserControl
{
    public BasketWindow()
    {
        InitializeComponent();
        DataContext = new BasketViewModel();
    }
    
    private void RemoveItem_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as BasketViewModel;
        vm?.RemoveItem();
    }
    
    private void ClearAll_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as BasketViewModel;
        vm?.ClearAll();
    }
}