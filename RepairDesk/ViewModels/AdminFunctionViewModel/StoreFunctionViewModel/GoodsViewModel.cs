using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using RepairDesk.Models;
using RepairDesk.Services;
using YourApp.Services; // твой namespace

namespace RepairDesk.ViewModels;

public class GoodsViewModel : ReactiveObject
{
    private readonly DatabaseService _db;
    
    public ObservableCollection<Products> Products { get; set; }
    
    private Products? _selectedProduct;
    public Products? SelectedProduct
    {
        get => _selectedProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
    }
    
    private Products _newProduct = new Products();
    public Products NewProduct
    {
        get => _newProduct;
        set => this.RaiseAndSetIfChanged(ref _newProduct, value);
    }
    
    public ReactiveCommand<Unit, Unit> AddProductCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteProductCommand { get; }
    public ReactiveCommand<Unit, Unit> SellProductCommand { get; }
    
    public GoodsViewModel()
    {
        _db = new DatabaseService();
        Products = _db.LoadProducts();
        
        AddProductCommand = ReactiveCommand.Create(AddProduct);
        
        // Для DeleteProduct и SellProduct используем SelectedProduct
        DeleteProductCommand = ReactiveCommand.Create(DeleteProduct, 
            this.WhenAnyValue(x => x.SelectedProduct).Select(p => p != null));
        SellProductCommand = ReactiveCommand.Create(SellProduct,
            this.WhenAnyValue(x => x.SelectedProduct).Select(p => p != null && p.Quantity > 0));
    }
    
    private void AddProduct()
    {
        if (string.IsNullOrWhiteSpace(NewProduct.ProductName)) return;
        
        _db.AddProduct(NewProduct);
        Products.Add(NewProduct);
        NewProduct = new Products();
    }
    
    private void DeleteProduct()
    {
        if (SelectedProduct == null) return;
        
        _db.DeleteProduct(SelectedProduct.ID);
        Products.Remove(SelectedProduct);
        SelectedProduct = null;
    }
    
    private void SellProduct()
    {
        if (SelectedProduct == null || SelectedProduct.Quantity <= 0) return;
        
        _db.SellProduct(SelectedProduct);
        
        // Обновляем отображение
        var index = Products.IndexOf(SelectedProduct);
        if (index != -1)
        {
            Products[index] = SelectedProduct;
        }
    }
    
    public void SellProductDirect(Products product)
    {
        Console.WriteLine($"Продажа: {product?.ProductName}");
        if (product == null || product.Quantity <= 0) return;
    
        _db.SellProduct(product);
    
        // Обновляем в коллекции
        var index = Products.IndexOf(product);
        if (index != -1)
        {
            Products[index] = product;
        }
    }

    public void DeleteProductDirect(Products product)
    {
        Console.WriteLine($"Удаление: {product?.ProductName}");
        if (product == null) return;
    
        _db.DeleteProduct(product.ID);
        Products.Remove(product);
    
        if (SelectedProduct == product)
            SelectedProduct = null;
    }
}