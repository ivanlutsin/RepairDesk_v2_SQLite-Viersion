using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Data.Sqlite;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel.AddOrderFunction;

public class EditOrderViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    private readonly DatabaseService _db = new();
    private List<DeviceItem> _allDevices = new();

    private Orders? _selectedOrder;
    public Orders? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            if (value != null)
            {
                SelectedDeviceType = value.DeviceType ?? string.Empty;
                SelectedBrand = value.Brand ?? string.Empty;
                SelectedModel = value.Model ?? string.Empty;
                SelectedStatus = string.IsNullOrEmpty(value.RepairsStatus) ? "Новый" : value.RepairsStatus;
                
                UpdateBrands();
                UpdateModels();
            }
            else
            {
                SelectedDeviceType = string.Empty;
                SelectedBrand = string.Empty;
                SelectedModel = string.Empty;
                SelectedStatus = "Новый";
            }
        }
    }
    
    public List<string> Status { get; } = new()
    {
        "Новый",
        "В работе",
        "Готов",
        "Выдан",
        "Отменён",
    };
    
    private string _selectedStatus = "Новый";
    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            _selectedStatus = value ?? "Новый";
            if (SelectedOrder != null)
                SelectedOrder.RepairsStatus = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Orders> Orders { get; set; } = new();
    
    private ObservableCollection<string> _deviceTypes = new();
    public ObservableCollection<string> DeviceTypes
    {
        get => _deviceTypes;
        set
        {
            _deviceTypes = value;
            OnPropertyChanged();
        }
    }
    
    private ObservableCollection<string> _brands = new();
    public ObservableCollection<string> Brands
    {
        get => _brands;
        set
        {
            _brands = value;
            OnPropertyChanged();
        }
    }
    
    private ObservableCollection<string> _models = new();
    public ObservableCollection<string> Models
    {
        get => _models;
        set
        {
            _models = value;
            OnPropertyChanged();
        }
    }
    
    private string _selectedDeviceType = string.Empty;
    public string SelectedDeviceType
    {
        get => _selectedDeviceType;
        set
        {
            _selectedDeviceType = value ?? string.Empty;
            if (SelectedOrder != null)
                SelectedOrder.DeviceType = value;
            OnPropertyChanged();
            UpdateBrands();
        }
    }
    
    private string _selectedBrand = string.Empty;
    public string SelectedBrand
    {
        get => _selectedBrand;
        set
        {
            _selectedBrand = value ?? string.Empty;
            if (SelectedOrder != null)
                SelectedOrder.Brand = value;
            OnPropertyChanged();
            UpdateModels();
        }
    }
    
    private string _selectedModel = string.Empty;
    public string SelectedModel
    {
        get => _selectedModel;
        set
        {
            _selectedModel = value ?? string.Empty;
            if (SelectedOrder != null)
                SelectedOrder.Model = value;
            OnPropertyChanged();
        }
    }

    public EditOrderViewModel()
    {
        LoadDevices();
        LoadOrders();
    }

    public void Refresh()
    {
        LoadOrders();
        ShowMessage("Данные обновлены успешно", "Успех");
    }
    
    public void LoadOrders()
    {
        try
        {
            Orders.Clear();

            using var connection = _db.GetConnection();
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Repairs";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Orders.Add(new Orders
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    ClientFullName = reader.GetString(reader.GetOrdinal("ClientFullName")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    DeviceType = reader.IsDBNull(reader.GetOrdinal("DeviceType")) ? null : reader.GetString(reader.GetOrdinal("DeviceType")),
                    Brand = reader.IsDBNull(reader.GetOrdinal("Brand")) ? null : reader.GetString(reader.GetOrdinal("Brand")),
                    Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                    ProblemDescription = reader.IsDBNull(reader.GetOrdinal("ProblemDescription")) ? null : reader.GetString(reader.GetOrdinal("ProblemDescription")),
                    RepairsStatus = reader.IsDBNull(reader.GetOrdinal("RepairsStatus")) ? "Новый" : reader.GetString(reader.GetOrdinal("RepairsStatus")),
                });
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка");
        }
    }
    
    private void LoadDevices()
    {
        try
        {
            _allDevices = _db.LoadDevices().ToList();
            
            DeviceTypes = new ObservableCollection<string>(
                _allDevices.Select(d => d.Device_Type).Distinct().OrderBy(t => t)
            );
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при загрузке устройств: {ex.Message}", "Ошибка");
        }
    }
    
    private void UpdateBrands()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedDeviceType))
            {
                Brands.Clear();
            }
            else
            {
                var brandsList = _allDevices
                    .Where(d => d.Device_Type == SelectedDeviceType)
                    .Select(d => d.Brand)
                    .Distinct()
                    .OrderBy(b => b)
                    .ToList();
                
                Brands.Clear();
                foreach (var brand in brandsList)
                    Brands.Add(brand);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при обновлении списка брендов: {ex.Message}", "Ошибка");
        }
    }
    
    private void UpdateModels()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedDeviceType) || string.IsNullOrEmpty(SelectedBrand))
            {
                Models.Clear();
            }
            else
            {
                var modelsList = _allDevices
                    .Where(d => d.Device_Type == SelectedDeviceType && d.Brand == SelectedBrand)
                    .Select(d => d.Model)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();
                
                Models.Clear();
                foreach (var model in modelsList)
                    Models.Add(model);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при обновлении списка моделей: {ex.Message}", "Ошибка");
        }
    }
    
    public void Save()
    {
        try
        {
            if (SelectedOrder == null)
            {
                ShowMessage("Не выбран заказ для сохранения", "Ошибка");
                return;
            }

            using var connection = _db.GetConnection();
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
                @"
             UPDATE Repairs
             SET
                 ClientFullName = $client,
                 PhoneNumber = $phone,
                 DeviceType = $device,
                 Brand = $brand,
                 Model = $model,
                 ProblemDescription = $problem,
                 RepairsStatus = $status
             WHERE ID = $id
             ";

            cmd.Parameters.AddWithValue("$client", SelectedOrder.ClientFullName ?? "");
            cmd.Parameters.AddWithValue("$phone", SelectedOrder.PhoneNumber ?? "");
            cmd.Parameters.AddWithValue("$device", SelectedDeviceType ?? "");
            cmd.Parameters.AddWithValue("$brand", SelectedBrand ?? "");
            cmd.Parameters.AddWithValue("$model", SelectedModel ?? "");
            cmd.Parameters.AddWithValue("$problem", SelectedOrder.ProblemDescription ?? "");
            cmd.Parameters.AddWithValue("$id", SelectedOrder.ID);
            cmd.Parameters.AddWithValue("$status", SelectedStatus ?? "Новый");

            cmd.ExecuteNonQuery();
            
            LoadOrders();
            ShowMessage("Изменения успешно сохранены", "Успех");
        }
        catch (SqliteException ex)
        {
            if (ex.Message.Contains("NOT NULL constraint failed"))
            {
                ShowMessage("Ошибка: Заполните все обязательные поля (Бренд и Модель)", "Ошибка");
            }
            else
            {
                ShowMessage($"Ошибка базы данных: {ex.Message}", "Ошибка");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Ошибка при сохранении: {ex.Message}", "Ошибка");
        }
    }
    
    // Метод для показа сообщений
    private async void ShowMessage(string message, string title)
    {
        try
        {
            // Получаем активное окно
            var currentWindow = GetActiveWindow();
            if (currentWindow != null)
            {
                await MessageBox.Show(currentWindow, message, title);
            }
            else
            {
                // Если окно не найдено, используем старый способ
                Console.WriteLine($"{title}: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing message: {ex.Message}");
        }
    }

// Добавьте метод для получения активного окна
    private Window? GetActiveWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Ищем активное окно среди всех открытых
            foreach (var window in desktop.Windows)
            {
                if (window.IsActive || window.IsFocused)
                {
                    return window;
                }
            }
            // Если активного нет, берем первое открытое
            return desktop.Windows.FirstOrDefault();
        }
        return null;
    }

}
public static class MessageBox
{
    public static async Task Show(Window parent, string message, string title)
    {
        try
        {
            var dialog = new Window
            {
                Title = title,
                Width = 350,
                Height = 180,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        new TextBlock 
                        { 
                            Text = message, 
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            Margin = new Thickness(0, 0, 0, 20)
                        },
                        new Button 
                        { 
                            Content = "OK", 
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Width = 100
                        }
                    }
                }
            };
            
            var button = ((StackPanel)dialog.Content).Children[1] as Button;
            button.Click += (s, e) => dialog.Close();
            
            await dialog.ShowDialog(parent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MessageBox error: {ex.Message}");
        }
    }
}