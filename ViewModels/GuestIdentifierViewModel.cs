using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels;

public partial class GuestIdentifierViewModel : ObservableObject
{
	private readonly Participant _participant;

	public GuestIdentifierViewModel()
	{
		_participant = new Participant("","");
		ContinueCommand = new AsyncRelayCommand(Continue);
		ChooseMaleCommand = new RelayCommand(() => SelectGender("Man"));
		ChooseFemaleCommand = new RelayCommand(() => SelectGender("Vrouw"));
		_participant.Gender = "";
		_participant.Name = "";
	}

	public string ParticipantName
	{
		get => _participant.Name;
		set
		{
			if (_participant.Name != value)
			{
				_participant.Name = value;
				OnPropertyChanged(nameof(ParticipantName));
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
				OnPropertyChanged(nameof(Gender));
				OnPropertyChanged(nameof(IsMaleSelected));
				OnPropertyChanged(nameof(IsFemaleSelected));
			}
		}
	}


	public bool IsMaleSelected => Gender == "Man";
	public bool IsFemaleSelected => Gender == "Vrouw";

	public IAsyncRelayCommand ContinueCommand { get; }
	public IRelayCommand ChooseMaleCommand { get; }
	public IRelayCommand ChooseFemaleCommand { get; }

	private void SelectGender(string gender)
	{
		Gender = gender;
	}

	private async Task Continue()
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

		_participant.Name = ParticipantName;
		_participant.Gender = Gender;

		bool exists = await _participant.AddParticipantIfNotExists();
		if (exists)
		{
			int participantId = await _participant.GetParticipantPrimarykey();
			_participant.ParticipantId = participantId;

			await App.Current.MainPage.Navigation.PushAsync(new GuestPage(_participant));
		}
		else
		{
			await App.Current.MainPage.DisplayAlert("Waarschuwing", "Gebruiker bestaat al. Vul een andere gebruikersnaam in.", "OK");
		}
	}

}
