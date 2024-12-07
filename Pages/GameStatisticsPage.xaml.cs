using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class GameStatisticsPage : ContentPage
{
	private Participant _participant;

	public GameStatisticsPage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
		BindingContext = this;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await InitializeData();
	}

	private async Task InitializeData()
	{
		Participant truthParticipant = await GetParticipantWithMostTruthAnswers();
		Participant drinkParticipant = await GetParticipantWithMostDrinkAnswers();

		// Update UI elements on the main thread
		MainThread.BeginInvokeOnMainThread(() =>
		{
			if (truthParticipant != null)
			{
				TruthLabel.Text = truthParticipant.Name ?? "Geen Data";
				TopTruthCount.Text = $"Meeste keren Truth: {truthParticipant.TruthOrDrinkCount}";
				TopTruthImage.Source = truthParticipant.GenderImage;
			}
			else
			{
				TruthLabel.Text = "Geen Data";
				TopTruthCount.Text = "Geen Data";
				TopTruthImage.Source = null;
			}

			if (drinkParticipant != null)
			{
				DrinkLabel.Text = drinkParticipant.Name ?? "Geen Data";
				TopDrinkCount.Text = $"Meeste keren Drink: {drinkParticipant.TruthOrDrinkCount}";
				TopDrinkImage.Source = drinkParticipant.GenderImage;
			}
			else
			{
				DrinkLabel.Text = "Geen Data";
				TopDrinkCount.Text = "Geen Data";
				TopDrinkImage.Source = null;
			}
		});
	}


	private async Task<Participant> GetParticipantWithMostTruthAnswers()
	{
		return await _participant.GetMostTruths();
	}

	private async Task<Participant> GetParticipantWithMostDrinkAnswers()
	{
		return await _participant.GetMostDrinks();
	}

	private async void ToMainMenuClicked(object sender, EventArgs e)
	{
		await _participant.SetGameToClose();
		await Navigation.PopToRootAsync();
	}
}