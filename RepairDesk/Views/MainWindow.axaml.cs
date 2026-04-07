using Avalonia.Controls;
using RepairDesk.ViewModels;
using RepairDesk.Models;
using System;
using Avalonia.Interactivity;

namespace RepairDesk.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Подписываемся на событие после инициализации
        this.Opened += OnWindowOpened;
    }

    private void OnWindowOpened(object? sender, EventArgs e)
    {
        this.Opened -= OnWindowOpened; // Отписываемся чтобы не сработало повторно

        var vm = DataContext as MainWindowViewModel;
        if (vm == null) return;

        vm.LoginSuccessful += OnLoginSuccessful;
    }

    private void OnLoginSuccessful(object? sender, User user)
    {
        try
        {
            Console.WriteLine($"[Window] Opening window for role: {user.Role}");

            // Отписываемся от события
            if (DataContext is MainWindowViewModel vm)
                vm.LoginSuccessful -= OnLoginSuccessful;

            // Создаём окно в зависимости от роли
            Window newWindow = user.Role switch
            {
                "Admin" => new AdminWindow(user),
                "Master" => new MakerWindow(user),
                "Seller" => new SellerWindow(user),
                "Form" => new MinimumForm(user),
                _ => new SellerWindow(user)
            };

            Console.WriteLine($"[Window] Created: {newWindow.GetType().Name}");

            // Показываем новое окно
            newWindow.Show();

            Console.WriteLine($"[Window] Closing login window");
            
            // Закрываем окно входа
            this.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Window ERROR] {ex.Message}");
            Console.WriteLine($"[Window ERROR] Stack: {ex.StackTrace}");
        }
    }
}