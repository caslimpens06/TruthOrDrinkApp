using System.Collections.ObjectModel;
using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class HostControlGamePage : ContentPage
{
	private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
	private Game _game;
	public HostControlGamePage(Game game)
	{
		InitializeComponent();
		_game = game;
		UserList.ItemsSource = _participants;
		RefreshContent();
	}
	private async void RefreshButtonClicked(object sender, EventArgs e)
	{
		var button = (Button)sender;

		// Flicker effect: maakt het klikken visueel door de transparantie kort te veranderen.
		await button.FadeTo(0, 200);
		await button.FadeTo(1, 200);

		// Refresh de deelnemerslijst
		await RefreshContent();
	}

	private async Task RefreshContent()
	{
			SupabaseService supabaseService = new SupabaseService();

			// Fetch participants from the Supabase service
			List<Participant> participants = await supabaseService.GetParticipantsByGame(_game);

			// Clear and re-populate the ObservableCollection
			_participants.Clear();
			foreach (var participant in participants)
			{
				Console.WriteLine($"Participant: {participant.Name}, Gender: {participant.Gender}");
				_participants.Add(participant);
			}
	}

	private void StartButtonClicked(object sender, EventArgs e) 
	{
		DisplayAlert("Klik", "Start Game", "Ok");
	}
}