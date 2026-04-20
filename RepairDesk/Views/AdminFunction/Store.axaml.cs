using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Views.AdminFunction.StoreFunctionWindow;
using YourApp.Services;

namespace RepairDesk.Views.AdminFunction;

public partial class Store : UserControl
{
    
    private BasketWindow _basketWindow;
    private GoodsWindow _goodsWindow;
    private SparePartsWindow _sparePartsWindow;
    
    public Store()
    {
        DataContext = this;
        SetDefaultContent();
        InitializeComponent();
    }
    private readonly DatabaseService _db = new DatabaseService();

    private void SetDefaultContent()
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new TextBlock 
            { 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
    
    private void SparePartsClick(object? sender, RoutedEventArgs e)
    {
        if (_sparePartsWindow == null)
            _sparePartsWindow = new SparePartsWindow();
        
        MainContentArea.Content = _sparePartsWindow;
    }

    private void GoodsClick(object? sender, RoutedEventArgs e)
    {
        if (_goodsWindow == null)
            _goodsWindow = new GoodsWindow();
        
        MainContentArea.Content = _goodsWindow;
    }

    private void BasketClick(object? sender, RoutedEventArgs e)
    {
        if (_basketWindow == null)
            _basketWindow = new BasketWindow();
        
        MainContentArea.Content = _basketWindow;
    }
}