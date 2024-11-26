using System.Runtime.CompilerServices;
using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class GuestPage : ContentPage
{
	private int _participantid;
	public GuestPage(int participantid)
	{
		InitializeComponent();
		_participantid = participantid;
	}

	private async void Connect_Clicked(object sender, EventArgs e)
	{
		string gameCode = GameCodeEntry.Text;
		if (string.IsNullOrWhiteSpace(gameCode) || gameCode.Length != 6)
		{
			await DisplayAlert("Ongeldige gamecode", "Voer de gamecode in die op het hostapparaat staat. Dit is een 6-cijferige code.", "OK");
			return;
		}
		else 
		{
			int parsedGameCode = Int32.Parse(gameCode);
			SupabaseService supabaseService = new SupabaseService();

			if (await supabaseService.CheckIfGameExistsAsync(parsedGameCode))
			{
				Participant participant = new Participant(_participantid, parsedGameCode);
				await supabaseService.JoinParticipantToGame(participant);
				await Navigation.PushModalAsync(new ParticipantGamePage(participant));
			}
			else 
			{
				await DisplayAlert("Ongeldige gamecode", "Verifiëer of de host al een spel heeft gemaakt.", "OK");
			}
		}
	}

	private void QR_Clicked(object sender, EventArgs e)
	{
		Navigation.PushModalAsync(new QRScanner(_participantid));
	}
}