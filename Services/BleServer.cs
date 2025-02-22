#if WINDOWS
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Radios;
using Windows.Storage.Streams;


namespace BLE_DesktopApp.Services;

public class BleServer
{
    private GattServiceProvider? _gattServiceProvider;
    private static readonly Guid _ServiceUuid = Guid.NewGuid();
    private static readonly Guid _CharacteristicUuid = Guid.NewGuid();
    private static GattLocalCharacteristic? _characteristic;

    public bool isRunning = false;

    public async Task<bool> Start()
    {

        var bluetoothAdapter = await BluetoothAdapter.GetDefaultAsync();
        if(bluetoothAdapter == null)
        {
            Console.WriteLine("Bluetooth is not available on this device.");
            isRunning = false;
            return false;
        }

        var radios = await Radio.GetRadiosAsync();
        var bluetoothRadio = radios.FirstOrDefault(r => r.Kind == RadioKind.Bluetooth);
        if(bluetoothRadio == null || bluetoothRadio.State != RadioState.On)
        {
            Console.WriteLine("Bluetooth is turned off. Please enable it.");
            isRunning = false;
            return false;
        }

        var result = await GattServiceProvider.CreateAsync(_ServiceUuid);
        if(result.Error != BluetoothError.Success)
        {
            Console.WriteLine($"Failed to start BLE server. Error: {result.Error}");
            isRunning = false;
            return false;
        }
        
        _gattServiceProvider = result.ServiceProvider;

        var characteristicResult = await _gattServiceProvider.Service.CreateCharacteristicAsync(
            _CharacteristicUuid,
            new GattLocalCharacteristicParameters
            {
                CharacteristicProperties =  GattCharacteristicProperties.Read | GattCharacteristicProperties.Write,
                ReadProtectionLevel = GattProtectionLevel.Plain,
                WriteProtectionLevel = GattProtectionLevel.Plain
            });
        
        if(characteristicResult.Error != BluetoothError.Success)
        {
            Console.WriteLine("Failed to create characteristic.");
            return false;
        }

        _characteristic = characteristicResult.Characteristic;


        _characteristic.ReadRequested += async (sender, args) =>
        {
            await WriteToClientAsync(args, "Helo From Gat Server");
        };

        _characteristic.WriteRequested += async (sender, args) =>
        {
            await ReadFromClientAsync(args);
        };


        _gattServiceProvider.StartAdvertising(new GattServiceProviderAdvertisingParameters
        {
            IsConnectable = true,
            IsDiscoverable = true
        });

        Console.WriteLine("Server started successfully");
        isRunning = true;
        return true;
    }

    public void Stop()
    {
        _gattServiceProvider?.StopAdvertising();
        Console.WriteLine("Server has been stopped");
        isRunning = false;
    }

    private async Task WriteToClientAsync(GattReadRequestedEventArgs args, string message)
    {
        var deferral = args.GetDeferral();
        try
        {
            var request = await args.GetRequestAsync(); 
            var writer = new DataWriter();
            writer.WriteString(message);
            request.RespondWithValue(writer.DetachBuffer());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in WriteToClientAsync: {ex.Message}");
        }
        finally
        {
            deferral.Complete();
        }
    }

    private async Task ReadFromClientAsync(GattWriteRequestedEventArgs args)
    {
        var deferral = args.GetDeferral();
        try
        {
            var request = await args.GetRequestAsync(); 
            var reader = DataReader.FromBuffer(request.Value);
            string message = reader.ReadString(reader.UnconsumedBufferLength);
            Console.WriteLine($"Received message: {message}");
            request.Respond();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ReadFromClientAsync: {ex.Message}");
        }
        finally
        {
            deferral.Complete();
        }
    }
}
#endif