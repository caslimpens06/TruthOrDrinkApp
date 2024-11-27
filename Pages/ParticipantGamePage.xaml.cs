using TruthOrDrink.Model;

namespace TruthOrDrink;

public partial class ParticipantGamePage : ContentPage
{
	private Participant _participant;
	private List<Question> _questions;
	private int _currentQuestionIndex;
	private Game _game;
	private readonly SupabaseService _supabaseService;

	public ParticipantGamePage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
		_supabaseService = new SupabaseService();
	}
	
	// Called when the page is displayed
	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Initialize game and questions when the page appears
		await InitializePageAsync();
	}

	private async Task InitializePageAsync()
	{
		await InitializeGameAsync();
		await LoadQuestionsAsync();
		
	}

	private async Task InitializeGameAsync()
	{

		Participant participant = new Participant(_participant.SessionCode);
		Game game = await participant.GetGameBySessionId();

		if (game == null)
		{
			await DisplayAlert("Error", "Game not found. Please check the game code.", "OK");
			Application.Current.MainPage = new WelcomePage(); // Navigate back
			return;
		}

		if (GameNameLabel != null)
		{
			GameNameLabel.Text = game.Name;
		}
		_game = game;
	}

	private async Task LoadQuestionsAsync()
	{
		_questions = await _game.GetQuestionsAsync();

		if (_questions == null || _questions.Count == 0)
		{
			QuestionLabel.Text = "Het spel is afgelopen.";
			TruthButton.IsVisible = false;
			DrinkButton.IsVisible = false;
			NextButton.IsVisible = false;
			return;
		}

		_currentQuestionIndex = 0;
		SetNextQuestion();
	}

	private void SetNextQuestion()
	{
		if (_questions != null && _currentQuestionIndex < _questions.Count)
		{
			QuestionLabel.Text = _questions[_currentQuestionIndex].Text;
			QuestionLabel.TextColor = Colors.Black;

			TruthButton.IsVisible = true;
			DrinkButton.IsVisible = true;
			NextButton.IsVisible = false;
		}
		else
		{
			QuestionLabel.Text = "Het spel is afgelopen.";
			TruthButton.IsVisible = false;
			DrinkButton.IsVisible = false;
			NextButton.IsVisible = false;
		}
	}

	private async void LeaveGame(object sender, EventArgs e)
	{
		try
		{
			_supabaseService.RemoveParticipantAsync(_participant);
			Application.Current.MainPage = new WelcomePage();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Could not leave game: {ex.Message}", "OK");
		}
	}

	private async void OnTruthClicked(object sender, EventArgs e)
	{
		NextButton.IsVisible = true;
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;

		QuestionLabel.Text = "Je koos Truth!";
		QuestionLabel.TextColor = Colors.Green;

		await SaveAnswerAsync("Truth");
	}

	private async void OnDrinkClicked(object sender, EventArgs e)
	{
		NextButton.IsVisible = true;
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;

		QuestionLabel.Text = "Je koos Drink!";
		QuestionLabel.TextColor = Colors.Red;

		await SaveAnswerAsync("Drink");
	}

	private async Task SaveAnswerAsync(string response)
	{
		try
		{
			var question = _questions[_currentQuestionIndex];
			Answer answer = new Answer(question.Id, response, _participant.ParticipantId);

			await answer.SaveAnswerAsync();

			_currentQuestionIndex++;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Could not save answer: {ex.Message}", "OK");
		}
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton.IsVisible = true;
		DrinkButton.IsVisible = true;
		NextButton.IsVisible = false;

		SetNextQuestion();
	}
}
