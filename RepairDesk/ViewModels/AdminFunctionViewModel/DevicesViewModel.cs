using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using RepairDesk.Models;
using YourApp.Services;

namespace RepairDesk.ViewModels.AdminFunctionViewModel;

public class DevicesViewModel: ReactiveObject
{
    private readonly DatabaseService _db;
    
    public ObservableCollection<DeviceItem> Devices { get; set; }
    
    private DeviceItem? _selectedDevice;
    public DeviceItem? SelectedDevice
    {
        get => _selectedDevice;
        set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
    }
    
    // Новое устройство
    private DeviceItem _newDevice = new DeviceItem();
    public DeviceItem NewDevice
    {
        get => _newDevice;
        set => this.RaiseAndSetIfChanged(ref _newDevice, value);
    }
    
    // Команды
    public ReactiveCommand<Unit, Unit> AddDeviceCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteDeviceCommand { get; }
    
    public DevicesViewModel()
    {
        _db = new DatabaseService();
        Devices = _db.LoadDevices();
        
        AddDeviceCommand = ReactiveCommand.Create(AddDevice);
        DeleteDeviceCommand = ReactiveCommand.Create(DeleteDevice, 
            this.WhenAnyValue(x => x.SelectedDevice).Select(d => d != null));
    }
    
    private void AddDevice()
    {
        if (string.IsNullOrWhiteSpace(NewDevice.Device_Type)) return;
        if (string.IsNullOrWhiteSpace(NewDevice.Brand)) return;
        if (string.IsNullOrWhiteSpace(NewDevice.Model)) return;
        
        _db.AddDevice(NewDevice);
        Devices.Add(NewDevice);
        NewDevice = new DeviceItem();
    }
    
    private void DeleteDevice()
    {
        if (SelectedDevice == null) return;
        
        _db.DeleteDevice(SelectedDevice.ID);
        Devices.Remove(SelectedDevice);
        SelectedDevice = null;
    }

    public void DeleteDeviceDirect(DeviceItem? device)
    {
        if (device == null) return;
    
        _db.DeleteDevice(device.ID);
        Devices.Remove(device);
    
        if (SelectedDevice == device)
            SelectedDevice = null;
    }
}