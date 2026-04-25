using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels
{
    public class SparePartsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // ================== СТАТУС ОБНОВЛЕНИЯ ==================

        private bool _isRefreshing;

        // ================== ДАННЫЕ ==================

        public ObservableCollection<RepairParts> Parts { get; set; } = new();

        public ObservableCollection<string> Types { get; set; } = new();
        public ObservableCollection<string> Devices { get; set; } = new();
        public ObservableCollection<string> Manufacturers { get; set; } = new();

        // ================== ФИЛЬТРЫ ==================

        private string _selectedType = "Все";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType == value)
                    return;

                _selectedType = value;
                OnPropertyChanged();

                if (!_isRefreshing)
                    LoadParts();
            }
        }

        private string _selectedDevice = "Все";
        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice == value)
                    return;

                _selectedDevice = value;
                OnPropertyChanged();

                if (!_isRefreshing)
                    LoadParts();
            }
        }

        private string _selectedManufacturer = "Все";
        public string SelectedManufacturer
        {
            get => _selectedManufacturer;
            set
            {
                if (_selectedManufacturer == value)
                    return;

                _selectedManufacturer = value;
                OnPropertyChanged();

                if (!_isRefreshing)
                    LoadParts();
            }
        }

        // ================== ВЫБРАННАЯ ДЕТАЛЬ ==================

        private RepairParts? _selectedPart;
        public RepairParts? SelectedPart
        {
            get => _selectedPart;
            set
            {
                _selectedPart = value;
                OnPropertyChanged();
            }
        }

        // ================== КОНСТРУКТОР ==================

        public SparePartsViewModel()
        {
            Refresh();
        }

        // ================== ЗАГРУЗКА ФИЛЬТРОВ ==================

        public void LoadFilters()
        {
            Types.Clear();
            Devices.Clear();
            Manufacturers.Clear();

            Types.Add("Все");
            Devices.Add("Все");
            Manufacturers.Add("Все");

            using var connection = _db.GetConnection();
            connection.Open();

            var cmdType = connection.CreateCommand();
            cmdType.CommandText = "SELECT DISTINCT Type FROM RepairParts WHERE Type IS NOT NULL AND Type != ''";

            using (var reader = cmdType.ExecuteReader())
            {
                while (reader.Read())
                    Types.Add(reader.GetString(0));
            }

            var cmdDevice = connection.CreateCommand();
            cmdDevice.CommandText = "SELECT DISTINCT ForDevice FROM RepairParts WHERE ForDevice IS NOT NULL AND ForDevice != ''";

            using (var reader = cmdDevice.ExecuteReader())
            {
                while (reader.Read())
                    Devices.Add(reader.GetString(0));
            }

            var cmdManufacturer = connection.CreateCommand();
            cmdManufacturer.CommandText = "SELECT DISTINCT Manufacturer FROM RepairParts WHERE Manufacturer IS NOT NULL AND Manufacturer != ''";

            using (var reader = cmdManufacturer.ExecuteReader())
            {
                while (reader.Read())
                    Manufacturers.Add(reader.GetString(0));
            }
        }

        // ================== ЗАГРУЗКА ДЕТАЛЕЙ ==================

        public void LoadParts()
        {
            Parts.Clear();

            using var connection = _db.GetConnection();
            connection.Open();

            var cmd = connection.CreateCommand();

            var conditions = new List<string>();

            if (SelectedType != "Все")
                conditions.Add("Type = $type");

            if (SelectedDevice != "Все")
                conditions.Add("ForDevice = $device");

            if (SelectedManufacturer != "Все")
                conditions.Add("Manufacturer = $manufacturer");

            var where = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : "";

            cmd.CommandText =
                "SELECT ID, RepairID, PartName, Barcode, Price, Quantity, Manufacturer, Type, ForDevice " +
                $"FROM RepairParts {where}";

            if (SelectedType != "Все")
                cmd.Parameters.AddWithValue("$type", SelectedType);

            if (SelectedDevice != "Все")
                cmd.Parameters.AddWithValue("$device", SelectedDevice);

            if (SelectedManufacturer != "Все")
                cmd.Parameters.AddWithValue("$manufacturer", SelectedManufacturer);

            using var reader = cmd.ExecuteReader();

            // индексы читаем 1 раз
            int id = reader.GetOrdinal("ID");
            int repairId = reader.GetOrdinal("RepairID");
            int partName = reader.GetOrdinal("PartName");
            int barcode = reader.GetOrdinal("Barcode");
            int price = reader.GetOrdinal("Price");
            int qty = reader.GetOrdinal("Quantity");
            int manufacturer = reader.GetOrdinal("Manufacturer");
            int type = reader.GetOrdinal("Type");
            int device = reader.GetOrdinal("ForDevice");

            while (reader.Read())
            {
                Parts.Add(new RepairParts()
                {
                    ID = reader.IsDBNull(id) ? 0 : reader.GetInt32(id),
                    RepairID = reader.IsDBNull(repairId) ? null : reader.GetInt32(repairId),
                    PartName = reader.IsDBNull(partName) ? "" : reader.GetString(partName),
                    Barcode = reader.IsDBNull(barcode) ? "" : reader.GetString(barcode),
                    Price = reader.IsDBNull(price) ? 0 : reader.GetDouble(price),
                    Quantity = reader.IsDBNull(qty) ? 0 : reader.GetInt32(qty),
                    Manufacturer = reader.IsDBNull(manufacturer) ? "" : reader.GetString(manufacturer),
                    Type = reader.IsDBNull(type) ? "" : reader.GetString(type),
                    ForDevice = reader.IsDBNull(device) ? "" : reader.GetString(device),
                });
            }
        }

        // ================== REFRESH ==================

        public void Refresh()
        {
            _isRefreshing = true;

            LoadFilters();
            LoadParts();
        }
        
        public void RefreshCommand()
        {
            LoadParts();
        }
        
        public void DeleteSelectedPart()
        {
            if (SelectedPart == null)
                return;

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM RepairParts WHERE ID = $id";
            cmd.Parameters.AddWithValue("$id", SelectedPart.ID);

            cmd.ExecuteNonQuery();

            // обновляем список
            LoadParts();
        }
        
        // ================== ПРИСВОЕНИЕ ==================

        public void AssignPart()
        {
            if (SelectedPart == null)
                return;
        }
    }
}