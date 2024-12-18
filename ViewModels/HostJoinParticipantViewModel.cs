using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class HostJoinParticipantViewModel : ObservableObject
	{
		private readonly Session _session;
		private readonly ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();

		public HostJoinParticipantViewModel(Session session)
		{
			_session = session;
			RefreshCommand = new AsyncRelayCommand(RefreshContent);
			StartGameCommand = new AsyncRelayCommand(StartGame);
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
		}

		public ObservableCollection<Participant> Participants => _participants;

		public IAsyncRelayCommand RefreshCommand { get; }
		public IAsyncRelayCommand StartGameCommand { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		public async Task RefreshContent()
		{
			var participants = await _session.GetParticipantsBySession();
			_participants.Clear();
			foreach (var participant in participants)
			{
				_participants.Add(participant);
			}
		}

		public async Task StartGame()
		{
			_session.StartGame();
			// Navigate to HostControlsGamePage
			await App.Current.MainPage.Navigation.PushAsync(new HostControlsGamePage(_session));
		}

		public async Task LeaveGame()
		{
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}