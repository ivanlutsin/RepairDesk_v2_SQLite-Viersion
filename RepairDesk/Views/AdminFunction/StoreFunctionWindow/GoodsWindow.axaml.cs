using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;
using RepairDesk.ViewModels;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow;

public partial class GoodsWindow : UserControl
{
    public GoodsWindow()
    {
        InitializeComponent();
        DataContext = new GoodsViewModel();
    }

    private void SellProductCommand(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Products;
        
        if (product != null && product.Quantity > 0)
        {
            var vm = DataContext as GoodsViewModel;
            vm?.SellProductDirect(product);
        }
    }

    private void DeleteProductCommand(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Products;
        
        if (product != null)
        {
            var vm = DataContext as GoodsViewModel;
            vm?.DeleteProductDirect(product);
        }
    }
}