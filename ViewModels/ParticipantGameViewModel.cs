using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels;

public partial class ParticipantGameViewModel : ObservableObject
{
	private readonly Participant _participant;
	private readonly INavigation _navigation;
	private List<Question> _questions;
	private List<Question> _answeredQuestions = new();
	private Question _currentQuestion;
	private Game _game;

	[ObservableProperty]
	private string gameName;

	[ObservableProperty]
	private string currentQuestionText;

	[ObservableProperty]
	private bool isQuestionAvailable;

	[ObservableProperty]
	private bool areButtonsEnabled;

	public IRelayCommand AnswerCommand { get; }
	public IRelayCommand LeaveGameCommand { get; }

	public ParticipantGameViewModel(Participant participant)
	{
		_participant = participant;
		_navigation = Application.Current.MainPage.Navigation;

		AnswerCommand = new RelayCommand<string>(async (response) => await HandleAnswerAsync(response));
		LeaveGameCommand = new RelayCommand(async () => await LeaveGameAsync());

		InitializePageAsync();
	}

	public ParticipantGameViewModel() { }

	private async Task InitializePageAsync()
	{
		await InitializeGameAsync();
		await LoadQuestionsAsync();
	}

	private async Task InitializeGameAsync()
	{
		Participant participant = new Participant(_participant.SessionCode);
		Game game = await participant.GetGameBySessionId();

		GameName = game.Name;
		_game = game;
	}

	private async Task LoadQuestionsAsync()
	{
		_questions = await _game.GetQuestionsAsync();

		if (_questions == null || _questions.Count == 0)
		{
			CurrentQuestionText = "Het spel is afgelopen.";
			IsQuestionAvailable = false;
			return;
		}

		await SetNextQuestionAsync();
	}

	private async Task SetNextQuestionAsync()
	{
		if (_questions != null && _answeredQuestions.Count == _questions.Count)
		{
			CurrentQuestionText = "Het spel is afgelopen. Wachten op antwoorden...";
			IsQuestionAvailable = false;
			return;
		}

		if (_questions != null && _questions.Count > 0)
		{
			AreButtonsEnabled = false;
			Question currentQuestion = await GetCurrentQuestionWithRetryAsync();
			_currentQuestion = currentQuestion;

			bool isQuestionAnswered = _answeredQuestions.Any(q => q.QuestionId == currentQuestion?.QuestionId);

			if (currentQuestion != null && !isQuestionAnswered)
			{
				_answeredQuestions.Add(currentQuestion);
				CurrentQuestionText = currentQuestion.Text;

				IsQuestionAvailable = true;
				AreButtonsEnabled = true;
			}
			else
			{
				CurrentQuestionText = "Het spel is afgelopen.";
				IsQuestionAvailable = false;
			}
		}
		else
		{
			CurrentQuestionText = "Het spel is afgelopen.";
			IsQuestionAvailable = false;
		}
	}

	private async Task<Question> GetCurrentQuestionWithRetryAsync()
	{
		Question currentQuestion = null;
		bool checking = true;
		CurrentQuestionText = "Wachten op vraag...";

		while (checking)
		{
			await MonitorGameClosureAsync();

			currentQuestion = await _participant.GetCurrentQuestionAsync();

			if (currentQuestion != null && !_answeredQuestions.Any(q => q.QuestionId == currentQuestion.QuestionId))
			{
				break;
			}

			await Task.Delay(1000);
		}

		return currentQuestion;
	}

	private async Task HandleAnswerAsync(string response)
	{
		AreButtonsEnabled = false;
		IsQuestionAvailable = false;
		CurrentQuestionText = "Wachten op volgende vraag...";

		await SaveAnswerAsync(response);
		await SetNextQuestionAsync();
	}

	private async Task SaveAnswerAsync(string response)
	{
		Answer answer = new(_currentQuestion.QuestionId, response, _participant.ParticipantId);
		await answer.SaveAnswerAsync();

		if (!_answeredQuestions.Contains(_currentQuestion))
		{
			_answeredQuestions.Add(_currentQuestion);
		}

		if (_answeredQuestions.Count == _questions.Count)
		{
			await _participant.SetAllQuestionsToAnswered();
			if (await CheckIfSessionDoneAsync())
			{
				await _navigation.PushAsync(new GameStatisticsParticipantPage(_participant));
			}
		}
	}

	private async Task<bool> CheckIfSessionDoneAsync()
	{
		bool done = false;
		CurrentQuestionText = "Het spel is afgelopen. Wachten op statistieken...";

		while (!done)
		{
			if (await _participant.CheckIfAllQuestionsAnswered())
			{
				done = true;
			}

			await Task.Delay(1000);
		}

		return done;
	}

	private async Task MonitorGameClosureAsync()
	{
		bool isGameClosed = await _participant.CheckIfGameClosed();

		if (isGameClosed)
		{
			await _navigation.PushAsync(new GameStatisticsParticipantPage(_participant));
		}
	}

	private async Task LeaveGameAsync()
	{
		await _participant.RemoveParticipantAsync();
		await _navigation.PopToRootAsync();
	}
}
