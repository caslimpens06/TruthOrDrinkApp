using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using System.Windows.Input;

namespace TruthOrDrink.ViewModels
{
	public class GuestIdentifierViewModel : ObservableObject
	{
		private string _gender;
		private string _participantName;

		public GuestIdentifierViewModel()
		{
			ContinueCommand = new AsyncRelayCommand(Continue);
			ChooseMaleCommand = new RelayCommand(() => Gender = "Man");
			ChooseFemaleCommand = new RelayCommand(() => Gender = "Vrouw");
		}

		public string Gender
		{
			get => _gender;
			set => SetProperty(ref _gender, value);
		}

		public string ParticipantName
		{
			get => _participantName;
			set => SetProperty(ref _participantName, value);
		}

		public IAsyncRelayCommand ContinueCommand { get; }
		public ICommand ChooseMaleCommand { get; }
		public ICommand ChooseFemaleCommand { get; }

		private async Task Continue()
		{
			if (string.IsNullOrWhiteSpace(ParticipantName))
			{
				await Application.Current.MainPage.DisplayAlert("Warning", "Please enter your name before continuing.", "OK");
				return;
			}

			Participant participant = new Participant(ParticipantName, Gender);
			SupabaseService supabaseService = new SupabaseService();

			bool exists = await participant.AddParticipantIfNotExists();
			if (exists)
			{
				int ID = await participant.GetParticipantPrimarykey();
				Participant newParticipant = new Participant(ID, ParticipantName, Gender);
				await Application.Current.MainPage.Navigation.PushAsync(new GuestPage(newParticipant));
			}
			else
			{
				await Application.Current.MainPage.DisplayAlert("Warning", "Username already exists. Please choose a different username.", "OK");
			}
		}
	}
}