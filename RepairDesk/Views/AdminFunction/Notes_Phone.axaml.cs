using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;
using RepairDesk.ViewModels.AdminFunctionViewModel;

namespace RepairDesk.Views.AdminFunction;

public partial class Notes_Phone : UserControl
{
    public Notes_Phone()
    {
        InitializeComponent();
        DataContext = new Notes_PhoneViewModel();
    }

    private void SelectContact_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var contact = button?.CommandParameter as PhoneBookItem;
    }

    private void EditContact_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var contact = button?.CommandParameter as PhoneBookItem;
        
        if (contact != null)
        {
            var vm = DataContext as Notes_PhoneViewModel;
            vm.SelectedItem = contact;
            
            // Заполняем NewItem для редактирования
            vm.NewItem = new PhoneBookItem
            {
                ID = contact.ID,
                FullName = contact.FullName,
                PhoneNumber = contact.PhoneNumber,
                Comment = contact.Comment
            };
            
            // Можно открыть диалог редактирования или просто дать редактировать в форме
            Console.WriteLine($"Редактирование: {contact.FullName}");
        }
    }

    private void DeleteContact_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var contact = button?.CommandParameter as PhoneBookItem;
        
        if (contact != null)
        {
            var vm = DataContext as Notes_PhoneViewModel;
            vm.SelectedItem = contact;
            vm.DeleteCommand.Execute().Subscribe();
            
            Console.WriteLine($"Удалён контакт: {contact.FullName}");
        }
    }
}