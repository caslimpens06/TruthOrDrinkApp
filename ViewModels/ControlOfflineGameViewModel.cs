using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class ControlOfflineGameViewModel : ObservableObject
	{
		private bool _questionsAreClickable = true;
		private readonly List<int> _tappedQuestionIds = new();
		private readonly List<Participant> _participants;
		private int _currentPlayerIndex = 0;
		private List<Question> _questions;
		private Session _session;

		public ControlOfflineGameViewModel(Session session, List<Participant> participants)
		{
			_session = session;
			LoadLocalQuestions();
			_participants = participants;

			OnQuestionTappedCommand = new AsyncRelayCommand<Question>(OnQuestionTapped);
			StopGameCommand = new AsyncRelayCommand(StopGame);
		}

		public ObservableCollection<Question> Questions { get; } = new ObservableCollection<Question>();
		public IAsyncRelayCommand<Question> OnQuestionTappedCommand { get; }
		public IAsyncRelayCommand StopGameCommand { get; }

		private async void LoadLocalQuestions()
		{
			_questions = await _session.GetLocalQuestions();

			foreach (var question in _questions)
			{
				Questions.Add(question);
			}
		}

		private async Task OnQuestionTapped(Question tappedQuestion)
		{
			if (!_questionsAreClickable || tappedQuestion == null || tappedQuestion.IsTapped)
			{
				await Application.Current.MainPage.DisplayAlert("Waarschuwing", "Deze vraag is al gespeeld. Je kan hem niet nog een keer spelen.", "OK");
				return;
			}

			_questionsAreClickable = false;

			tappedQuestion.IsTapped = true;
			tappedQuestion.IsEnabled = false;

			_tappedQuestionIds.Add(tappedQuestion.QuestionId);

			await SetCurrentQuestion(tappedQuestion);

			if (await CheckIfPlayerAnswered())
			{
				_questionsAreClickable = true;
			}

			_currentPlayerIndex = (_currentPlayerIndex + 1) % _participants.Count;

			if (Questions.All(q => q.IsTapped))
			{
				await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsParticipantPage(_participants));
			}
		}

		private async Task StopGame()
		{
			await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsParticipantPage(_participants));
		}

		private async Task<bool> CheckIfPlayerAnswered()
		{
			// Logic to check if the current player has answered
			// In offline mode, just simulate answering and wait for some time (e.g., 2 seconds)
			await Task.Delay(2000);
			return true;
		}


		private async Task SetCurrentQuestion(Question tappedQuestion)
		{
			//await tappedQuestion.SetCurrentQuestion(new Session());  // Here we use a new Session object as a placeholder
		}
	}
}
