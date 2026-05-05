using ReactiveUI;
using System.Collections.ObjectModel;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels;

public class BasketViewModel : ReactiveObject
{
    private readonly DatabaseService _db;
    
    public ObservableCollection<CartItem> CartItems { get; set; }
    
    private CartItem? _selectedItem;
    public CartItem? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }
    
    private double _totalAmount;
    public double TotalAmount
    {
        get => _totalAmount;
        set => this.RaiseAndSetIfChanged(ref _totalAmount, value);
    }
    
    public BasketViewModel()
    {
        _db = new DatabaseService();
        RefreshCart();
    }
    
    public void RefreshCart()
    {
        CartItems = _db.LoadCart();
        this.RaisePropertyChanged(nameof(CartItems));
        UpdateTotal();
    }
    
    private void UpdateTotal()
    {
        TotalAmount = 0;
        foreach (var item in CartItems)
        {
            TotalAmount += item.Total;
        }
    }
    
    // Методы для кнопок (их будем вызывать из .axaml.cs)
    public void RemoveItem()
    {
        if (SelectedItem == null) return;
        
        _db.RemoveFromCart(SelectedItem.ID);
        RefreshCart();
    }
    
    public void ClearAll()
    {
        _db.ClearCart();
        RefreshCart();
    }
}