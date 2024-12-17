using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;

namespace TruthOrDrink.ViewModels
{
	public class GameStatisticsViewModel : ObservableObject
	{
		private Participant _participant;
		private string _truthLabel;
		private string _topTruthCount;
		private string _drinkLabel;
		private string _topDrinkCount;
		private string _topTruthImage;
		private string _topDrinkImage;

		public GameStatisticsViewModel(Participant participant)
		{
			_participant = participant;
			ToMainMenuCommand = new AsyncRelayCommand(ToMainMenu);
			InitializeData();
		}

		public string TruthLabel
		{
			get => _truthLabel;
			set => SetProperty(ref _truthLabel, value);
		}

		public string TopTruthCount
		{
			get => _topTruthCount;
			set => SetProperty(ref _topTruthCount, value);
		}

		public string DrinkLabel
		{
			get => _drinkLabel;
			set => SetProperty(ref _drinkLabel, value);
		}

		public string TopDrinkCount
		{
			get => _topDrinkCount;
			set => SetProperty(ref _topDrinkCount, value);
		}

		public string TopTruthImage
		{
			get => _topTruthImage;
			set => SetProperty(ref _topTruthImage, value);
		}

		public string TopDrinkImage
		{
			get => _topDrinkImage;
			set => SetProperty(ref _topDrinkImage, value);
		}

		public IAsyncRelayCommand ToMainMenuCommand { get; }

		private async void InitializeData()
		{
			Participant truthParticipant = await GetParticipantWithMostTruthAnswers();
			Participant drinkParticipant = await GetParticipantWithMostDrinkAnswers();

			if (truthParticipant != null)
			{
				TruthLabel = truthParticipant.Name ?? "Geen Data";
				TopTruthCount = $"Meeste keren Truth: {truthParticipant.TruthOrDrinkCount}";
				TopTruthImage = truthParticipant.GenderImage;
			}
			else
			{
				TruthLabel = "Geen Data";
				TopTruthCount = "Geen Data";
				TopTruthImage = null;
			}

			if (drinkParticipant != null)
			{
				DrinkLabel = drinkParticipant.Name ?? "Geen Data";
				TopDrinkCount = $"Meeste keren Drink: {drinkParticipant.TruthOrDrinkCount}";
				TopDrinkImage = drinkParticipant.GenderImage;
			}
			else
			{
				DrinkLabel = "Geen Data";
				TopDrinkCount = "Geen Data";
				TopDrinkImage = null;
			}
		}

		private async Task<Participant> GetParticipantWithMostTruthAnswers()
		{
			return await _participant.GetMostTruths();
		}

		private async Task<Participant> GetParticipantWithMostDrinkAnswers()
		{
			return await _participant.GetMostDrinks();
		}

		private async Task ToMainMenu()
		{
			await _participant.RemoveParticipantAsync();
			await Application.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}