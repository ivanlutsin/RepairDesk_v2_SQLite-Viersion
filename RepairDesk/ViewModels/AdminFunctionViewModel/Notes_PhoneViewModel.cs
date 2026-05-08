using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel;

public class Notes_PhoneViewModel: ReactiveObject
{
    private readonly DatabaseService _db;

    private ObservableCollection<PhoneBookItem> _phoneBook;

    public ObservableCollection<PhoneBookItem> PhoneBook
    {
        get => _phoneBook;
        set => this.RaiseAndSetIfChanged(ref _phoneBook, value);
    }

    private PhoneBookItem _selectedItem;

    public PhoneBookItem SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private PhoneBookItem _newItem;

    public PhoneBookItem NewItem
    {
        get => _newItem;
        set => this.RaiseAndSetIfChanged(ref _newItem, value);
    }

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    public Notes_PhoneViewModel()
    {
        _db = new DatabaseService();
        _phoneBook = new ObservableCollection<PhoneBookItem>();
        _newItem = new PhoneBookItem();

        LoadPhoneBook();

        AddCommand = ReactiveCommand.Create(Add);
        UpdateCommand = ReactiveCommand.Create(Update,
            this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null));
        DeleteCommand = ReactiveCommand.Create(Delete,
            this.WhenAnyValue(x => x.SelectedItem).Select(x => x != null));
        RefreshCommand = ReactiveCommand.Create(Refresh);
    }

    private void LoadPhoneBook()
    {
        PhoneBook = _db.LoadPhoneBook();
    }

    private void Add()
    {
        if (string.IsNullOrWhiteSpace(NewItem.FullName) ||
            string.IsNullOrWhiteSpace(NewItem.PhoneNumber))
        {
            Console.WriteLine("Заполните ФИО и телефон!");
            return;
        }

        _db.AddPhoneBookItem(NewItem);
        PhoneBook.Add(NewItem);
        NewItem = new PhoneBookItem();
    }

    private void Update()
    {
        if (SelectedItem == null) return;

        _db.UpdatePhoneBookItem(SelectedItem);
        Refresh();
    }

    private void Delete()
    {
        if (SelectedItem == null) return;

        _db.DeletePhoneBookItem(SelectedItem.ID);
        PhoneBook.Remove(SelectedItem);
        SelectedItem = null;
    }

    private void Refresh()
    {
        PhoneBook = _db.LoadPhoneBook();
        this.RaisePropertyChanged(nameof(PhoneBook));
    }
}