using System.Runtime.CompilerServices;

namespace TruthOrDrink;

public partial class GuestPage : ContentPage
{
	public GuestPage()
	{
		InitializeComponent();
	}

	private async void Connect_Clicked(object sender, EventArgs e)
	{
		string gameCode = GameCodeEntry.Text;
		if (string.IsNullOrWhiteSpace(gameCode))
		{
			await DisplayAlert("Ongeldige gamecode", "Voer de gamecode in die op het hostapparaat staat.", "OK");
			return;
		}
		else 
		{
			int gamecode = Int32.Parse(gameCode);
			// Connect to game
			await DisplayAlert("Gamecode", $"De ingevoerde gamecode is {gameCode}", "OK");
		}
	}

	private void QR_Clicked(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new QRScanner());
	}
}