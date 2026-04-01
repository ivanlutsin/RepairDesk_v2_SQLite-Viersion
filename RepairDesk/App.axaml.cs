using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RepairDesk.ViewModels;
using RepairDesk.Views;
using YourApp.Services;

namespace RepairDesk;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }
        
        try
        {
            var db = new DatabaseService();

            using var connection = db.GetConnection();
            connection.Open();

            File.WriteAllText("db_test.txt", "Found Succesfully!");
        }
        catch (Exception ex)
        {
            File.WriteAllText("db_test.txt", ex.Message);
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}