using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink;

public partial class ParticipantGamePage : ContentPage
{
	private readonly Participant _participant;
	private List<Question> _questions;
	private readonly List<Question> _answeredquestions = new List<Question>();
	private Question _currentquestion;
	private Game _game;

	public ParticipantGamePage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
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
		GameNameLabel.Text = game.Name;
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
			return;
		}


		await SetNextQuestion();
	}

	private async Task SetNextQuestion()
	{
		if (_questions != null && _answeredquestions.Count == _questions.Count)
		{
			QuestionLabel.Text = "Het spel is afgelopen. Wachten op antwoorden...";
			return;
		}

		if (_questions != null && _questions.Count > 0)
		{
			TruthButton.IsVisible = false;
			DrinkButton.IsVisible = false;
			TruthButton.IsEnabled = false;
			DrinkButton.IsEnabled = false;

			Question currentQuestion = await GetCurrentQuestionWithRetryAsync();
			_currentquestion = currentQuestion;

			bool isQuestionAnswered = _answeredquestions.Any(q => q.QuestionId == currentQuestion?.QuestionId);

			if (currentQuestion != null && !isQuestionAnswered)
			{
				_answeredquestions.Add(currentQuestion);
				QuestionLabel.Text = currentQuestion.Text;
				QuestionLabel.TextColor = Colors.Black;

				TruthButton.IsVisible = true;
				DrinkButton.IsVisible = true;
				TruthButton.IsEnabled = true;
				DrinkButton.IsEnabled = true;
			}
			else
			{
				QuestionLabel.Text = "Het spel is afgelopen.";
				TruthButton.IsVisible = false;
				DrinkButton.IsVisible = false;
			}
		}
		else
		{
			QuestionLabel.Text = "Het spel is afgelopen.";
			TruthButton.IsVisible = false;
			DrinkButton.IsVisible = false;
		}
	}

	private async Task<bool> CheckIfSessionDone()
	{
		bool checking = true;
		bool done = false;
		QuestionLabel.Text = "Het spel is afgelopen. Wachten op statistieken...";

		while (checking)
		{
			if(await _participant.CheckIfAllQuestionsAnswered())
			{
				done = true;
				checking = false;
			}
			await Task.Delay(1000);
		}
		return done;
	}

	private async Task<Question> GetCurrentQuestionWithRetryAsync()
	{
		Question currentQuestion = null;
		bool checking = true;
		QuestionLabel.Text = "Wachten op vraag...";

		while (checking)
		{
			await MonitorGameClosureAsync();

			currentQuestion = await _participant.GetCurrentQuestionAsync();

			bool isQuestionAnswered = false;
			foreach (var answeredQuestion in _answeredquestions)
			{
				if (answeredQuestion.QuestionId == currentQuestion.QuestionId)
				{
					isQuestionAnswered = true;
					break;
				}
			}

			if (currentQuestion != null && !isQuestionAnswered)
			{
				break;
			}

			await Task.Delay(1000);
		}

		return currentQuestion;
	}


	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await _participant.RemoveParticipantAsync();
		await Navigation.PopToRootAsync();
	}

	private async void OnTruthClicked(object sender, EventArgs e)
	{
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;
		TruthButton.IsEnabled = false;
		DrinkButton.IsEnabled = false;
		QuestionLabel.Text = "Wachten op volgende vraag...";
		QuestionLabel.TextColor = Colors.Gray;

		await SaveAnswerAsync("Truth");

		await SetNextQuestion();
	}

	private async void OnDrinkClicked(object sender, EventArgs e)
	{
		TruthButton.IsVisible = false;
		DrinkButton.IsVisible = false;
		TruthButton.IsEnabled = false;
		DrinkButton.IsEnabled = false;
		QuestionLabel.Text = "Wachten op volgende vraag...";
		QuestionLabel.TextColor = Colors.Gray;

		await SaveAnswerAsync("Drink");

		await SetNextQuestion();
	}

	private async Task SaveAnswerAsync(string response)
	{
		Answer answer = new Answer(_currentquestion.QuestionId, response, _participant.ParticipantId);
		await answer.SaveAnswerAsync();

		if (!_answeredquestions.Contains(_currentquestion))
		{
			_answeredquestions.Add(_currentquestion);
		}

		if (_answeredquestions.Count == _questions.Count)
		{
			await _participant.SetAllQuestionsToAnswered();
			if (await CheckIfSessionDone())
			{
				await Navigation.PushAsync(new GameStatisticsPage(_participant));
			}
		}
	}
	private async Task MonitorGameClosureAsync()
	{
		bool isGameClosed = await _participant.CheckIfGameClosed();

		if (isGameClosed)
		{
			await Navigation.PushAsync(new GameStatisticsPage(_participant));
		}
	}
}
