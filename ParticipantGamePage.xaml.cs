using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class ParticipantGamePage : ContentPage
{
	private Participant _participant;

	public ParticipantGamePage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
	}


	private async void LeaveGame(object sender, EventArgs e)
	{
		SupabaseService supabaseService = new SupabaseService();
		supabaseService.RemoveParticipant(_participant);
		Application.Current.MainPage = new WelcomePage();

	}
}