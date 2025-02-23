using System.Collections.ObjectModel;

namespace BLE_DesktopApp.Models;

public class BleClient
{
    public required string DeviceId { get; set; }
    public required DateTime LastSeen { get; set; }
    public ObservableCollection<BleMessage> Messages {get; set;} = new ObservableCollection<BleMessage>();
}
