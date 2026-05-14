using Xunit;
using RepairDesk.Models;
using System.Linq;

using YourApp.Services;

namespace RepairDesk.Tests;

public class DatabaseTests
{
    private readonly DatabaseService _db = new();

    [Fact]
    public void ProductsTable_Load_ShouldReturnNotNull()
    {
        var products = _db.LoadProducts();
        Assert.NotNull(products);
    }

    [Fact]
    public void CartTable_Load_ShouldReturnNotNull()
    {
        var cart = _db.LoadCart();
        Assert.NotNull(cart);
    }

    [Fact]
    public void DevicesTable_Load_ShouldReturnNotNull()
    {
        var devices = _db.LoadDevices();
        Assert.NotNull(devices);
    }

    [Fact]
    public void PhoneBookTable_Load_ShouldReturnNotNull()
    {
        var phoneBook = _db.LoadPhoneBook();
        Assert.NotNull(phoneBook);
    }

    [Fact]
    public void AddProduct_AndDelete_ShouldWork()
    {
        var product = new Products
        {
            ProductName = "ТестУдалиМеня",
            Barcode = "123TEST",
            Price = 100,
            Quantity = 1,
            Characteristics = "Тестовый товар"
        };

        _db.AddProduct(product);
        var products = _db.LoadProducts();
        var added = products.FirstOrDefault(p => p.ProductName == "ТестУдалиМеня");

        Assert.NotNull(added);

        _db.DeleteProduct(added.ID);
        products = _db.LoadProducts();
        var deleted = products.FirstOrDefault(p => p.ProductName == "ТестУдалиМеня");

        Assert.Null(deleted);
    }
}