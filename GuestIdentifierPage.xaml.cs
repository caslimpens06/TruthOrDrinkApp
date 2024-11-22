using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class GuestIdentifierPage : ContentPage
{
	public GuestIdentifierPage()
	{
		InitializeComponent();
	}
	private async void Continue_Clicked(object sender, EventArgs e)
	{
		string participantName = NameEntry.Text;

		if (string.IsNullOrWhiteSpace(participantName))
		{
			await DisplayAlert("Waarschuwing", "Vul je naam in voordat je doorgaat.", "OK");
			return;
		}
		Participant participant = new Participant(participantName);
		SupabaseService supabaseService = new SupabaseService();
		supabaseService.AddParticipantAsync(participant);
		int ID = await supabaseService.GetPrimaryKeyByName(participantName);
		await Navigation.PushModalAsync(new GuestPage(ID));
	}
}