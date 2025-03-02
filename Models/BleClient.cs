using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BLE_DesktopApp.Models;

public class BleClient : INotifyPropertyChanged 
{
    public required string DeviceId { get; set; }
    public required DateTime LastSeen { get; set; }
    public ObservableCollection<BleMessage> Messages {get; set;} = new ObservableCollection<BleMessage>();
    // public bool IsExpanded { get; set; } = true;
    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set 
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
