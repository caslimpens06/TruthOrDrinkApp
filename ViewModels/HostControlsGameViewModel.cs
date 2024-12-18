using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class HostControlsGameViewModel : ObservableObject
	{
		private readonly ObservableCollection<Question> _questions = new();
		private readonly Game _game;
		private readonly Session _session;
		private readonly Participant _participant;
		private bool _questionsAreClickable = true;
		private readonly List<int> _tappedQuestions = new List<int>();

		public HostControlsGameViewModel(Session session)
		{
			_session = session;
			_game = new Game(session.GameId);
			LoadQuestionsAsync();
			_participant = new Participant(_session.SessionCode);
			StopGameCommand = new AsyncRelayCommand(StopGame);
		}

		public ObservableCollection<Question> Questions => _questions;

		public IAsyncRelayCommand StopGameCommand { get; }

		private async Task LoadQuestionsAsync()
		{
			_questions.Clear();
			var questionsList = await _game.GetQuestionsAsync();

			foreach (Question question in questionsList)
			{
				_questions.Add(question);
			}
		}

		public async Task OnQuestionTapped(Question tappedQuestion)
		{
			if (_questionsAreClickable)
			{
				_questionsAreClickable = false;

				if (_tappedQuestions.Contains(tappedQuestion.QuestionId))
				{
					_questionsAreClickable = true;
					await Application.Current.MainPage.DisplayAlert("Warning", "This question has already been played. You cannot play it again.", "OK");
				}
				else
				{
					_tappedQuestions.Add(tappedQuestion.QuestionId);
					tappedQuestion.IsTapped = true; // Mark the question as tapped

					await SetCurrentQuestion(tappedQuestion);

					if (await CheckIfEveryParticipantAnswered(tappedQuestion))
					{
						_questionsAreClickable = true;
					}

					if (_tappedQuestions.Count == _questions.Count)
					{
						if (await CheckIfSessionDone())
						{
							await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsPage(_participant));
						}
					}
				}
			}
		}

		private async Task SetCurrentQuestion(Question tappedQuestion)
		{
			await tappedQuestion.SetCurrentQuestion(_session);
		}

		private async Task<bool> CheckIfSessionDone()
		{
			bool checking = true;
			bool done = false;

			while (checking)
			{
				if (await _participant.CheckIfAllQuestionsAnswered())
				{
					done = true;
					checking = false;
				}
				await Task.Delay(500);
			}

			return done;
		}

		private async Task<bool> CheckIfEveryParticipantAnswered(Question tappedQuestion)
		{
			bool checking = true;
			bool done = false;

			while (checking)
			{
				if (await tappedQuestion.CheckIfAnswerHasBeenGiven(_session))
				{
					done = true;
					checking = false;
				}
				await Task.Delay(500);
			}

			return done;
		}

		private async Task StopGame()
		{
			await _participant.SetGameToClose();
			await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsPage(_participant));
		}
	}
}