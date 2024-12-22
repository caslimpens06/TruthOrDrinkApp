using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using System.Collections.ObjectModel;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class AddOfflineParticipantsViewModel : ObservableObject
	{
		private readonly Participant _participant;
		private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();
		private Session _session;
		private bool _isOverlayVisible;

		public IAsyncRelayCommand ContinueCommand { get; }
		public IAsyncRelayCommand AddCommand { get; }
		public IRelayCommand ChooseMaleCommand { get; }
		public IRelayCommand ChooseFemaleCommand { get; }
		public IRelayCommand<Participant> RemoveParticipantCommand { get; }

		public AddOfflineParticipantsViewModel(Session session)
		{
			_session = session;
			_participant = new Participant("", "");
			ContinueCommand = new AsyncRelayCommand(Continue);
			AddCommand = new AsyncRelayCommand(AddParticipant);
			ChooseMaleCommand = new RelayCommand(() => SelectGender("Man"));
			ChooseFemaleCommand = new RelayCommand(() => SelectGender("Vrouw"));
			RemoveParticipantCommand = new RelayCommand<Participant>(RemoveParticipant);
		}

		public AddOfflineParticipantsViewModel() { }

		public string ParticipantName
		{
			get => _participant.Name;
			set
			{
				if (_participant.Name != value)
				{
					_participant.Name = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(Participants));
				}
			}
		}

		public string Gender
		{
			get => _participant.Gender;
			set
			{
				if (_participant.Gender != value)
				{
					_participant.Gender = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(IsMaleSelected));
					OnPropertyChanged(nameof(IsFemaleSelected));
				}
			}
		}

		public bool IsOverlayVisible
		{
			get => _isOverlayVisible;
			set
			{
				_isOverlayVisible = value;
				OnPropertyChanged(nameof(IsOverlayVisible));
			}
		}

		public bool IsMaleSelected => Gender == "Man";
		public bool IsFemaleSelected => Gender == "Vrouw";
		public string ParticipantsCountDisplay => $"Deelnemers ({_participants.Count})";

		public ObservableCollection<Participant> Participants
		{
			get => _participants;
			set => SetProperty(ref _participants, value);
		}

		private void SelectGender(string gender)
		{
			Gender = gender;
		}

		private async Task AddParticipant()
		{
			if (string.IsNullOrWhiteSpace(ParticipantName))
			{
				await App.Current.MainPage.DisplayAlert("Waarschuwing", "Vul je naam in voordat je verdergaat.", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(Gender))
			{
				await App.Current.MainPage.DisplayAlert("Waarschuwing", "Je moet een geslacht kiezen voordat je verdergaat.", "OK");
				return;
			}

			Participant newParticipant = new Participant(ParticipantName, Gender);

			bool exists = _participants.Any(p => p.Name == newParticipant.Name && p.Gender == newParticipant.Gender);

			if (!exists)
			{
				_participants.Add(newParticipant);
				OnPropertyChanged(nameof(ParticipantsCountDisplay));
				ParticipantName = string.Empty;
				Gender = string.Empty;

				OnPropertyChanged(nameof(Participants));
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Waarschuwing", "Deze deelnemer bestaat al.", "OK");
			}
		}

		private void RemoveParticipant(Participant participant)
		{
			if (_participants.Contains(participant))
			{
				_participants.Remove(participant);
				OnPropertyChanged(nameof(Participants));
				OnPropertyChanged(nameof(ParticipantsCountDisplay));
			}
		}

		private async Task Continue()
		{
			if (_participants.Count > 0)
			{
				IsOverlayVisible = true;
				await App.Current.MainPage.Navigation.PushAsync(new ChooseDrinksPage(_session, _participants.ToList())); //only session.gameid and _participants are passed
				IsOverlayVisible = false;
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Waarschuwing", "Je hebt nog geen deelnemers toegevoegd aan het spel.", "OK");
			}
		}
	}
}
