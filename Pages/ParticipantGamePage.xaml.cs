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
			await DisplayAlert("Error", "Game niet gevonden.", "OK");
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
		await SetNextQuestion();
	}

	private async Task SetNextQuestion()
	{
		// Check if there are questions left to display
		if (_questions != null && _currentQuestionIndex < _questions.Count)
		{
			// Disable answer buttons until the question is ready
			TruthButton.IsEnabled = false;
			DrinkButton.IsEnabled = false;

			// Call GetCurrentQuestionWithRetryAsync to keep checking every 3 seconds until we get a valid question
			var currentQuestion = await GetCurrentQuestionWithRetryAsync();

			// Check if a valid question was found
			if (currentQuestion != null)
			{
				// Display the question text
				QuestionLabel.Text = currentQuestion.Text;
				QuestionLabel.TextColor = Colors.Black;

				// Enable answer buttons after the question is displayed
				TruthButton.IsEnabled = true;
				DrinkButton.IsEnabled = true;
				NextButton.IsVisible = false;
			}
			else
			{
				// If no valid question is found after retries, display end game message
				QuestionLabel.Text = "Het spel is afgelopen.";
				TruthButton.IsVisible = false;
				DrinkButton.IsVisible = false;
				NextButton.IsVisible = false;
			}
		}
		else
		{
			// End the game if no questions remain
			QuestionLabel.Text = "Het spel is afgelopen.";
			TruthButton.IsVisible = false;
			DrinkButton.IsVisible = false;
			NextButton.IsVisible = false;
		}
	}

	private async Task<Question> GetCurrentQuestionWithRetryAsync()
	{
		Question currentQuestion = null;

		// Keep checking for a valid question every 3 seconds until one is found
		while (currentQuestion == null)
		{
			// Call the method to fetch the current question
			currentQuestion = await _participant.GetCurrentQuestionAsync();

			// If a valid question is found, break the loop
			if (currentQuestion != null)
			{
				break;
			}

			// Wait for 3 seconds before trying again
			await Task.Delay(3000);
		}

		// Return the found question (or null if it wasn't found)
		return currentQuestion;
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
			Answer answer = new Answer(question.QuestionId, response, _participant.ParticipantId);

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
