using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;
using RepairDesk.ViewModels;
using YourApp.Services;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow;

public partial class GoodsWindow : UserControl
{
    public GoodsWindow()
    {
        InitializeComponent();
        DataContext = new GoodsViewModel();
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
    
    private void SellProduct_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var button = sender as Button;
        var product = button?.CommandParameter as Products;
    
        if (product != null && product.Quantity > 0)
        {
            Console.WriteLine("=== Button pressed ===");
            Console.WriteLine($"Goods: {product.ProductName}, ID={product.ID}, Price={product.Price}");
        
            // 1. Уменьшаем количество в Products
            var vm = DataContext as GoodsViewModel;
            vm?.SellProductDirect(product);
        
            // 2. Добавляем в корзину
            var db = new DatabaseService();
            db.AddToCart(product);
        
            // 3. Принудительно обновляем UI (тот самый магический способ)
            vm?.RefreshProducts();
        
            Console.WriteLine("=== The challenge is successful ===");
        }
    }
}