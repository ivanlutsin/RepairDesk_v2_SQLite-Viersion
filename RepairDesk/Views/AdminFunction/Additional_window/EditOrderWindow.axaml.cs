using Avalonia.Controls;
using Avalonia.Interactivity;
using RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

namespace RepairDesk.Views.AdminFunction.Additional_window;

public partial class EditOrderWindow : UserControl
{
    public EditOrderWindow()
    {
        InitializeComponent();
        
        // Подписываемся на события ComboBox после инициализации
        Loaded += (s, e) => SubscribeComboBoxEvents();
    }
    
    private void SubscribeComboBoxEvents()
    {
        if (DataContext is EditOrderViewModel vm)
        {
            var typeBox = this.FindControl<ComboBox>("TypeComboBox");
            var brandBox = this.FindControl<ComboBox>("BrandComboBox");
            var modelBox = this.FindControl<ComboBox>("ModelComboBox");
            
            if (typeBox != null)
                typeBox.SelectionChanged += (sender, args) => vm.SelectedDeviceType = typeBox.SelectedItem as string;
            
            if (brandBox != null)
                brandBox.SelectionChanged += (sender, args) => vm.SelectedBrand = brandBox.SelectedItem as string;
            
            if (modelBox != null)
                modelBox.SelectionChanged += (sender, args) => vm.SelectedModel = modelBox.SelectedItem as string;
        }
    }

    private void SaveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is EditOrderViewModel vm)
        {
            vm.Save();
        }
    }

    private void RefreshClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is EditOrderViewModel vm)
        {
            vm.Refresh();
        }
    }
}