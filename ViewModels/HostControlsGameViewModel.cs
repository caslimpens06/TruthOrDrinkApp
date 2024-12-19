using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class HostControlsGameViewModel : ObservableObject
	{
		private readonly Game _game;
		private readonly Session _session;
		private readonly Participant _participant;
		private bool _questionsAreClickable = true;
		private readonly List<int> _tappedQuestionIds = new();

		public HostControlsGameViewModel(Session session)
		{
			_session = session;
			_game = new Game(session.GameId);
			_participant = new Participant(session.SessionCode);

			StopGameCommand = new AsyncRelayCommand(StopGame);
			OnQuestionTappedCommand = new AsyncRelayCommand<QuestionViewModel>(OnQuestionTapped);

			LoadQuestionsAsync();
		}

		public ObservableCollection<QuestionViewModel> Questions { get; } = new();
		public IAsyncRelayCommand<QuestionViewModel> OnQuestionTappedCommand { get; }
		public IAsyncRelayCommand StopGameCommand { get; }

		private async Task LoadQuestionsAsync()
		{
			Questions.Clear();
			var questionsList = await _game.GetQuestionsAsync();

			foreach (var question in questionsList)
			{
				Questions.Add(new QuestionViewModel(question));
			}
		}

		private async Task OnQuestionTapped(QuestionViewModel tappedQuestion)
		{
			if (!_questionsAreClickable || tappedQuestion == null || tappedQuestion.IsTapped)
			{
				await Application.Current.MainPage.DisplayAlert("Waarschuwing", "Deze vraag is al gespeeld. Je kan hem niet nog een keer spelen.", "OK");
				return;
			}

			_questionsAreClickable = false;

			tappedQuestion.IsTapped = true;
			tappedQuestion.BackgroundColor = Colors.Green;

			_tappedQuestionIds.Add(tappedQuestion.QuestionId);

			await SetCurrentQuestion(tappedQuestion.Question);

			if (await CheckIfEveryParticipantAnswered(tappedQuestion.Question))
			{
				_questionsAreClickable = true;
			}

			if (Questions.All(q => q.IsTapped))
			{
				if (await CheckIfSessionDone())
				{
					await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsPage(_participant));
				}
			}
		}

		private async Task StopGame()
		{
			await _participant.SetGameToClose();
			await Application.Current.MainPage.Navigation.PushAsync(new GameStatisticsPage(_participant));
		}

		private async Task<bool> CheckIfSessionDone()
		{
			while (!await _participant.CheckIfAllQuestionsAnswered())
			{
				await Task.Delay(500);
			}
			return true;
		}

		private async Task<bool> CheckIfEveryParticipantAnswered(Question tappedQuestion)
		{
			while (!await tappedQuestion.CheckIfAnswerHasBeenGiven(_session))
			{
				await Task.Delay(1000);
			}
			return true;
		}

		private async Task SetCurrentQuestion(Question tappedQuestion)
		{
			await tappedQuestion.SetCurrentQuestion(_session);
		}
	}

	public partial class QuestionViewModel : ObservableObject
	{
		private readonly Question _question;
		private Color _backgroundColor = Colors.White;
		private bool _isTapped;

		public QuestionViewModel(Question question)
		{
			_question = question;
		}

		public int QuestionId => _question.QuestionId;
		public string Text => _question.Text;
		public Question Question => _question;

		public Color BackgroundColor
		{
			get => _backgroundColor;
			set => SetProperty(ref _backgroundColor, value);
		}

		public bool IsTapped
		{
			get => _isTapped;
			set => SetProperty(ref _isTapped, value);
		}
	}
}
