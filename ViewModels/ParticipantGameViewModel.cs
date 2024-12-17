using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class ParticipantGameViewModel : ObservableObject
	{
		private readonly Participant _participant;
		private Game _game;
		private List<Question> _questions;
		private readonly List<Question> _answeredQuestions = new List<Question>();
		private Question _currentQuestion;

		public ParticipantGameViewModel(Participant participant)
		{
			_participant = participant;
			TruthCommand = new AsyncRelayCommand(OnTruthClicked);
			DrinkCommand = new AsyncRelayCommand(OnDrinkClicked);
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
			InitializeGameAsync();
		}

		public string CurrentQuestionText => _currentQuestion?.Text ?? "Waiting for question...";

		public bool IsTruthButtonVisible => _currentQuestion != null;
		public bool IsDrinkButtonVisible => _currentQuestion != null;

		public IAsyncRelayCommand TruthCommand { get; }
		public IAsyncRelayCommand DrinkCommand { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		private async Task InitializeGameAsync()
		{
			await LoadQuestionsAsync();
			await SetNextQuestion();
		}

		private async Task LoadQuestionsAsync()
		{
			_game = await _participant.GetGameBySessionId();
			_questions = await _game.GetQuestionsAsync();

			if (_questions == null || !_questions.Any())
			{
				_currentQuestion = null; // No questions available
				return;
			}
		}

		private async Task SetNextQuestion()
		{
			if (_questions != null && _answeredQuestions.Count < _questions.Count)
			{
				_currentQuestion = _questions.FirstOrDefault(q => !_answeredQuestions.Contains(q));
				// Notify property change for CurrentQuestionText
				OnPropertyChanged(nameof(CurrentQuestionText));
			}
			else
			{
				_currentQuestion = null; // No more questions
				OnPropertyChanged(nameof(CurrentQuestionText));
			}
		}

		private async Task OnTruthClicked()
		{
			await SaveAnswerAsync("Truth");
			await SetNextQuestion();
		}

		private async Task OnDrinkClicked()
		{
			await SaveAnswerAsync("Drink");
			await SetNextQuestion();
		}

		private async Task SaveAnswerAsync(string response)
		{
			if (_currentQuestion != null)
			{
				Answer answer = new Answer(_currentQuestion.QuestionId, response, _participant.ParticipantId);
				await answer.SaveAnswerAsync();
				_answeredQuestions.Add(_currentQuestion);
			}

			if (_answeredQuestions.Count == _questions.Count)
			{
				await _participant.SetAllQuestionsToAnswered();
				// Navigate to statistics page or handle end of game
				await App.Current.MainPage.Navigation.PushAsync(new GameStatisticsPage(_participant));
			}
		}

		private async Task LeaveGame()
		{
			await _participant.RemoveParticipantAsync();
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}