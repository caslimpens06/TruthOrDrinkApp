using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class OfflineGameStatisticsViewModel : ObservableObject
	{
		private Participant _participant;
		private List<Participant> _participants;
		private string _truthLabel;
		private string _topTruthCount;
		private string _drinkLabel;
		private string _topDrinkCount;
		private string _topTruthImage;
		private string _topDrinkImage;

		public OfflineGameStatisticsViewModel(List<Participant> participants)
		{
			_participants = participants;
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
				TopTruthCount = $"Meeste keren Truth: {truthParticipant.TruthCount}";
				TopTruthImage = truthParticipant.GenderImage;
			}
			else
			{
				TruthLabel = "Geen Data";
				TopTruthCount = "Geen Data";
				TopTruthImage = "blankimage.png";
			}

			if (drinkParticipant != null)
			{
				DrinkLabel = drinkParticipant.Name ?? "Geen Data";
				TopDrinkCount = $"Meeste keren Drink: {drinkParticipant.DrinkCount}";
				TopDrinkImage = drinkParticipant.GenderImage;
			}
			else
			{
				DrinkLabel = "Geen Data";
				TopDrinkCount = "Geen Data";
				TopDrinkImage = "blankimage.png";
			}
		}

		private async Task<Participant> GetParticipantWithMostTruthAnswers()
		{
			return _participants.OrderByDescending(p => p.TruthCount).FirstOrDefault();
		}

		private async Task<Participant> GetParticipantWithMostDrinkAnswers()
		{
			return _participants.OrderByDescending(p => p.DrinkCount).FirstOrDefault();
		}

		private async Task ToMainMenu()
		{
			await App.Current.MainPage.Navigation.PushAsync(new OfflineMode());
		}
	}
}

