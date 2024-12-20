using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class ControlOfflineGameViewModel : ObservableObject
	{
		private readonly Session _session;
		private readonly List<Drink> _drinks;
		private readonly List<Participant> _participants;
		private List<Question> _questionList;
		private int _currentParticipantIndex = 0;
		private bool _waitingForNewQuestion = false;
		private List<Question> _answeredQuestions = new List<Question>() {};

		public ObservableCollection<Question> Questions { get; } = new();

		[ObservableProperty]
		private string currentParticipantName;
		[ObservableProperty]
		private bool participantShown = true;

		[ObservableProperty]
		private string currentQuestionText;

		[ObservableProperty]
		private bool isQuestionVisible;

		[ObservableProperty]
		private bool areButtonsEnabled = true;
		
		[ObservableProperty]
		private bool isQuestionListVisible = true;

		[ObservableProperty]
		private Color backgroundColor;

		public IRelayCommand StopGameCommand { get; }
		public IRelayCommand<string> AnswerCommand { get; }
		public IRelayCommand<Question> OnQuestionTappedCommand { get; }

		public ControlOfflineGameViewModel(Session session, List<Participant> participants, List<Drink> drinks)
		{
			_session = session;
			_participants = participants;
			_drinks = drinks;
			_waitingForNewQuestion = true;
			StopGameCommand = new RelayCommand(StopGame);
			AnswerCommand = new RelayCommand<string>(OnAnswer);
			OnQuestionTappedCommand = new RelayCommand<Question>(OnQuestionTapped);

			InitializeGame();
		}

		private async void InitializeGame()
		{
			await LoadQuestionsAsync();
		}

		private async Task LoadQuestionsAsync()
		{
			try
			{
				var questionList = await _session.GetLocalQuestions();
				if (questionList != null && questionList.Any())
				{
					foreach (var question in questionList)
					{
						Questions.Add(question);
					}
					_questionList = questionList;
					Console.WriteLine("Questions loaded successfully.");
				}
				else
				{
					Console.WriteLine("No questions found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading questions: {ex.Message}");
			}
		}

		private void OnQuestionTapped(Question question)
		{
			if (!_answeredQuestions.Contains(question))
			{
				// Mark the question as tapped
				question.IsTapped = true;

				// Hide the question list
				IsQuestionListVisible = false;

				if (_waitingForNewQuestion)
				{
					_waitingForNewQuestion = false;

					// Reset the participants' answered states
					foreach (var participant in _participants)
					{
						participant.HasAnswered = false;
					}
					_answeredQuestions.Add(question);

					// Enable buttons and show the question
					AreButtonsEnabled = true;
					IsQuestionVisible = true;
					CurrentQuestionText = question.Text;

					// Start the turn for the first participant
					_currentParticipantIndex = 0;
					ParticipantShown = true;
					CurrentParticipantName = $"It's {_participants[_currentParticipantIndex].Name}'s turn!";
				}
			}
		}




		private void OnAnswer(string answer)
		{
			if (!AreButtonsEnabled || _waitingForNewQuestion) return;

			var currentParticipant = _participants[_currentParticipantIndex];

			if (answer.Equals("Drink", StringComparison.OrdinalIgnoreCase))
			{
				currentParticipant.DrinkCount++;
			}
			else
			{
				currentParticipant.TruthCount++;
				currentParticipant.HasAnswered = true;
			}

			MoveToNextParticipantOrEndTurn();
		}

		private async void MoveToNextParticipantOrEndTurn()
		{
			if (_currentParticipantIndex + 1 < _participants.Count)
			{
				_currentParticipantIndex++;
				CurrentParticipantName = $"It's {_participants[_currentParticipantIndex].Name}'s turn!";
			}
			else
			{
				ParticipantShown = false;
				CurrentParticipantName = "";

				AreButtonsEnabled = false;
				IsQuestionVisible = false;
				_waitingForNewQuestion = true;
				IsQuestionListVisible = !IsQuestionListVisible;

				if (_answeredQuestions.Count == _questionList.Count)
				{
					await App.Current.MainPage.Navigation.PushAsync(new OfflineGameStatisticsPage(_participants));
				}
			}
		}


		private async void StopGame()
		{
			IsQuestionVisible = false;
			AreButtonsEnabled = false;
			await Application.Current.MainPage.Navigation.PushAsync(new OfflineGameStatisticsPage(_participants));
		}
	}
}
