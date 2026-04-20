using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

public partial class OrderCreatedWindow : Window
{
    private readonly long _orderId;

    public OrderCreatedWindow(long orderId)
    {
        InitializeComponent();

        _orderId = orderId;

        OrderText.Text = $"Заказ № {_orderId} создан";
    }

    private void CloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PrintAddOrderClck(object? sender, RoutedEventArgs e)
    {
        // заглушка
    }
}