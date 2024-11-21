namespace TruthOrDrink;

public partial class GuestIdentifierPage : ContentPage
{
	public GuestIdentifierPage()
	{
		InitializeComponent();
	}
	private async void Continue_Clicked(object sender, EventArgs e)
	{
		string userName = NameEntry.Text;

		if (string.IsNullOrWhiteSpace(userName))
		{

			await DisplayAlert("Waarschuwing", "Vul je naam in voordat je doorgaat.", "OK");
			return;
		}
		await Navigation.PushModalAsync(new GuestPage());
	}
}