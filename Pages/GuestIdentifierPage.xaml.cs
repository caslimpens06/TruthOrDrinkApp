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
		string gender;
		if (ChooseGenderFemale.Opacity < ChooseGenderMale.Opacity) 
		{ 
			gender = "Man"; 
		}
		else
		{
			gender = "Vrouw";
		}
		string participantName = NameEntry.Text;

		if (string.IsNullOrWhiteSpace(participantName))
		{
			await DisplayAlert("Waarschuwing", "Vul je naam in voordat je doorgaat.", "OK");
			return;
		}
		Participant participant = new Participant(participantName, gender);
		SupabaseService supabaseService = new SupabaseService();

		bool exists = await supabaseService.AddParticipantIfNotExists(participant);
		if (exists)
		{
			// Maak de zoek primarykey method opnieuw
			int ID = await supabaseService.GetParticipantPrimarykey(participant.Name);
			await Navigation.PushAsync(new GuestPage(ID));	
		}
		else 
		{
			await DisplayAlert("Waarschuwing", "Gebruikersnaam bestaat al. Kies een andere gebruikersnaam.", "Ok");
			return;
		}
	}

	private void ManButton_Clicked(object sender, EventArgs e) 
	{
		ChooseGenderFemale.Opacity = 0.2;
		ChooseGenderMale.Opacity = 1.0;
	}

	private void WomanButton_Clicked(object sender, EventArgs e) 
	{
		ChooseGenderMale.Opacity = 0.2;
		ChooseGenderFemale.Opacity = 1.0;
	}
}