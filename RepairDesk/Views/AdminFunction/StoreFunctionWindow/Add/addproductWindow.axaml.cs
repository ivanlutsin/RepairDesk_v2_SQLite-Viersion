using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels.AdminFunctionViewModel.StoreFunctionViewModel;

namespace RepairDesk.Views.AdminFunction.StoreFunctionWindow.Add;

public partial class addproductWindow : Window
{
    public addproductWindow()
    {
        InitializeComponent();
        DataContext = new addproductViewModel();
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is addproductViewModel vm)
        {
            vm.Save();

            PartNameBox.Focus(); // 👈 возврат фокуса
        }
    }
}