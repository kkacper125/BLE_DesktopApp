#if WINDOWS

using BLE_DesktopApp.Services;

namespace BLE_DesktopApp;

public partial class MainPage : ContentPage
{
	private BleServer _bleServer;

	public MainPage()
	{
		InitializeComponent();
		_bleServer = new BleServer();
	}

	private async Task OnStartBleServerButtonClicked(object sender, EventArgs e)
	{
		if(sender is Button button)
		{
			button.Text = _bleServer.isRunning ? "Start BLE server" : "Stop BLE server";
		}

		if(_bleServer.isRunning)
		{
			await _bleServer.Stop();
		}
		else
		{
			await _bleServer.Start();
		}
	}
}

#endif