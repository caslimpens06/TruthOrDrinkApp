using System.Collections.ObjectModel;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class HostJoinParticipantPage : ContentPage
{
	private readonly ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
	private readonly Session _session;

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

		// Blinking effect on label.
		await button.FadeTo(0, 200);
		await button.FadeTo(1, 200);

		await RefreshContent();
	}

	private async Task RefreshContent()
	{
		List<Participant> participants = await _session.GetParticipantsBySession();

		// Clear and re-populate the ObservableCollection with participants
		_participants.Clear();
		foreach (var participant in participants)
		{
			_participants.Add(participant);
		}
	}

	private async void StartButtonClicked(object sender, EventArgs e)
	{
		_session.StartGame();
		await Navigation.PushAsync(new HostControlsGamePage(_session));
	}

	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await Navigation.PopToRootAsync();

	}
}
