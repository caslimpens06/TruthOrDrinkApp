using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class ParticipantGamePage : ContentPage
{
	private Participant _participant;

	public ParticipantGamePage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
	}


	private async void LeaveGame(object sender, EventArgs e)
	{
		SupabaseService supabaseService = new SupabaseService();
		supabaseService.RemoveParticipant(_participant);
		Application.Current.MainPage = new WelcomePage();

	}

	private void OnTruthClicked(object sender, EventArgs e)
	{
		NextButton.IsVisible = true;
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;

		QuestionLabel.Text = "Je koos Truth!";
		QuestionLabel.TextColor = Colors.Green;
	}

	private void OnDrinkClicked(object sender, EventArgs e)
	{
		NextButton.IsVisible = true;
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;
		
		QuestionLabel.Text = "Je koos Drink!";
		QuestionLabel.TextColor = Colors.Red;
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton.IsVisible = true;
		DrinkButton.IsVisible = true;
		QuestionLabel.Text = "Hier komt de volgende vraag";
		QuestionLabel.TextColor = Colors.Black;
		NextButton.IsVisible = false;
	}
}