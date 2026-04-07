using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RepairDesk.Models;
using RepairDesk.Views.MakerFunction.Diagnostic_and_repair.checklist;
using RepairDesk.Views.MakerFunction.Diagnostic_and_repair.Create_a_report;
using RepairDesk.Views.MakerFunction.Diagnostic_and_repair.Spares;
using RepairDesk.Views.MakerFunction.Diagnostic_and_repair.Typical_problems;
using RepairDesk.Views.MakerFunction.My_Aplication.Application;
using RepairDesk.Views.MakerFunction.My_Aplication.Repair_history;
using RepairDesk.Views.MakerFunction.My_Aplication.Salary;
using RepairDesk.Views.MakerFunction.My_Aplication.Start_of_work_end;
using RepairDesk.Views.MakerFunction.Quick_access.Notes;
using RepairDesk.Views.MakerFunction.Quick_access.Technical_documentation;
using RepairDesk.Views.MakerFunction.Working_with_spare_parts.Creating_requests_for_the_order_of_spare_parts;
using RepairDesk.Views.MakerFunction.Working_with_spare_parts.Inspection_at_the_spare_parts_warehouse;

namespace RepairDesk.Views;

public partial class MakerWindow : Window
{
    public MakerWindow()
    {
        InitializeComponent();
        SetDefaultContent();
    }
    public MakerWindow(User user)
    {
        InitializeComponent();
        Console.WriteLine($"[MakerWindow] Opened for: {user.FullName}");
        SetDefaultContent();
    }


    private void SetDefaultContent()
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new TextBlock 
            { 
                Text = "Выберите раздел",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
        }
    }
    private void OrderMaker_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new AplicationWindow();
        }
    }

    private void History_repair_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new RepaerHistoryWindow();
        }
    }

    private void Salary_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new SalaryWindow();
        }
    }

    private void Start_of_work_end_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Stat_of_work_endWindow();
        }
    }

    private void Tech_dock_click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new Tecnical_docWindow();
        }
    }

    private void Notes_click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null)
        {
            MainContentArea.Content = new NotesWindow();
        }
    }

    private void Spare_Parts_click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new Inspection_at_the_spare_parts_warehouse();
    }

    private void Create_a_new_order_click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new Creating_requests_for_the_order_of_spare_parts();
    }

    private void CheckList_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new CheckListWindow();
    }

    private void Base_tipical_problems_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new Typical_problemsWindow();
    }

    private void Create_Report_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new Create_a_report();
    }

    private void Repair_Click(object? sender, RoutedEventArgs e)
    {
        if (MainContentArea != null) MainContentArea.Content = new SparesWindow();
    }
}