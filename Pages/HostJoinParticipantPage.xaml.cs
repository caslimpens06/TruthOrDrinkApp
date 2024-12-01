using System.Collections.ObjectModel;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class HostJoinParticipantPage : ContentPage
{
	private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
	private Session _session;

	// Property to bind to CollectionView
	public ObservableCollection<Participant> Users => _participants;

	public HostJoinParticipantPage(Session session)
	{
		InitializeComponent();
		_session = session;
		BindingContext = this;
		UserList.ItemsSource = _participants;
		RefreshContent();
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}


	private async void RefreshButtonClicked(object sender, EventArgs e)
	{
		var button = (Button)sender;

		// Flicker effect: makes the button briefly fade in and out
		await button.FadeTo(0, 200);
		await button.FadeTo(1, 200);

		// Refresh the participants list
		await RefreshContent();
	}

	private async Task RefreshContent()
	{
		SupabaseService supabaseService = new SupabaseService();

		// Fetch participants from the Supabase service
		List<Participant> participants = await _session.GetParticipantsBySession();

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
		_session.StartGame();
		Navigation.PushAsync(new HostControlsGamePage(_session));
	}

	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await Navigation.PopToRootAsync();

	}
}
