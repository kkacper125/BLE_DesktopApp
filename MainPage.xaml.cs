#if WINDOWS

using BLE_DesktopApp.Services;
using BLE_DesktopApp.Models;
using System.Resources;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BLE_DesktopApp;

public partial class MainPage : ContentPage
{
	public BleServer BleServer { get; }
	public ICommand ToggleExpandCommand {  get; } 
	public event EventHandler LanguageChanged;
	public string? this[string key] => resourceManager.GetString(key);

	private static readonly ResourceManager resourceManager = 
        new ResourceManager("BLE_DesktopApp.Resources.Localization.AppResources", typeof(MainPage).Assembly);
 	
	
	public MainPage()
	{
		InitializeComponent();
		BleServer = new BleServer();

		ToggleExpandCommand = new Command<BleClient>((client) =>
		{
			client.IsExpanded = !client.IsExpanded;
		});
		BindingContext = this;
		LanguageChanged += (s, e) => OnPropertyChanged("Item");
	}
	
	public void SetAppLanguage(string language){
		CultureInfo culture = new CultureInfo(language);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
	}
    
	private void OnChangeLanguageButtonClicked(object sender, TappedEventArgs e)
    {
		if(e.Parameter != null){
        	SetAppLanguage(e.Parameter.ToString());
		}
    }

	private async void OnStartBleServerButtonClicked(object sender, EventArgs e)
	{
		if (sender is Button button)
		{
			if(BleServer.isRunning)
			{
				button.SetBinding(Button.TextProperty, new Binding("[StartBleServer]")); 
				BleServer.Stop();
			}
			else
			{
				button.SetBinding(Button.TextProperty, new Binding("[StopBleServer]"));				
				await BleServer.Start();
			}
		}
	}
}

#endif