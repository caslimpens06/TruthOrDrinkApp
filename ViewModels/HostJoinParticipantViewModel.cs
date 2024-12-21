using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class HostJoinParticipantViewModel : ObservableObject
	{
		private Session _session;
		private readonly ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
		private bool _isOverlayVisible;

		public HostJoinParticipantViewModel(Session session)
		{
			_session = session;
			RefreshCommand = new AsyncRelayCommand(RefreshContent);
			StartGameCommand = new AsyncRelayCommand(StartGame);
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);

			RefreshContent().ConfigureAwait(false); // Refresh participants upon pageload
		}

		public ObservableCollection<Participant> Participants => _participants;

		public IAsyncRelayCommand RefreshCommand { get; }
		public IAsyncRelayCommand StartGameCommand { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		public bool IsOverlayVisible
		{
			get => _isOverlayVisible;
			set
			{
				_isOverlayVisible = value;
				OnPropertyChanged(nameof(IsOverlayVisible));
			}
		}

		public async Task RefreshContent()
		{
			List<Participant> participants = await _session.GetParticipantsBySession();

			_participants.Clear();
			foreach (var participant in participants)
			{
				_participants.Add(participant);
			}
		}

		public async Task StartGame()
		{
			IsOverlayVisible = true;
			await App.Current.MainPage.Navigation.PushAsync(new ChooseDrinksPage(_session));
			IsOverlayVisible = false;
		}

		public async Task LeaveGame()
		{
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}