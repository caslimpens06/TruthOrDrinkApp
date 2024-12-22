using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TruthOrDrink.ViewModels;

public partial class SettingsPageViewModel : ObservableObject
{
	private int _playerNumber;
	private bool _playerNumberReadOnly = true;
	private string _toggleButtonText = "Bewerk";
	private string _exampleSetting = "Default Setting Value";

	public int PlayerNumber
	{
		get => _playerNumber;
		set => SetProperty(ref _playerNumber, value);
	}

	public bool PlayerNumberReadOnly
	{
		get => _playerNumberReadOnly;
		set => SetProperty(ref _playerNumberReadOnly, value);
	}

	public string ToggleButtonText
	{
		get => _toggleButtonText;
		set => SetProperty(ref _toggleButtonText, value);
	}

	public string ExampleSetting
	{
		get => _exampleSetting;
		set => SetProperty(ref _exampleSetting, value);
	}

	public ICommand RefreshDrinkDataCommand { get; }
	public ICommand OpenInstagramCommand { get; }
	public ICommand OpenFacebookCommand { get; }
	public ICommand OpenLinkedInCommand { get; }
	public ICommand TogglePlayerCommand { get; }

	public SettingsPageViewModel()
	{
		RefreshDrinkDataCommand = new Command(RefreshDrinkData);
		OpenInstagramCommand = new Command(async () => await OpenUrlAsync("https://www.instagram.com/cas_limpens"));
		OpenFacebookCommand = new Command(async () => await OpenUrlAsync("https://www.facebook.com/profile.php?id=100076414400998"));
		OpenLinkedInCommand = new Command(async () => await OpenUrlAsync("https://www.linkedin.com/in/cas-limpens-a091742b1/"));
		TogglePlayerCommand = new Command(TogglePlayerNumberReadOnly);
	}

	private void RefreshDrinkData()
	{
		// Voeg hier je logica toe om de drankgegevens te verversen.
	}

	private async Task OpenUrlAsync(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
		{
			await Launcher.OpenAsync(uriResult);
		}
	}

	private void TogglePlayerNumberReadOnly()
	{
		_playerNumberReadOnly = !_playerNumberReadOnly;
		_toggleButtonText = _playerNumberReadOnly ? "Bewerk" : "Klaar";
	}
}
