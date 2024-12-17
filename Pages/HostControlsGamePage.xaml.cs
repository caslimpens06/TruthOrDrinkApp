using System.Collections.ObjectModel;
using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class HostControlsGamePage : ContentPage
{
	private readonly ObservableCollection<Question> _questions = new();
	private readonly Game _game;
	private readonly List<int> tappedQuestions = new List<int>();
	private readonly Session _session;
	private readonly Participant _participant;
	private bool _questionsareclickable = true;
	

	public HostControlsGamePage(Session session)
	{
		InitializeComponent();
		_game = new Game(session.GameId);
		BindingContext = this;
		_session = session;
		_ = LoadQuestionsAsync();
		_participant = new Participant(_session.SessionCode);
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}

	public ObservableCollection<Question> Questions => _questions;

	public string SelectedQuestion { get; set; }

	private async Task LoadQuestionsAsync()
	{
		_questions.Clear();
		var questionsList = await _game.GetQuestionsAsync();

		foreach (Question question in questionsList)
		{
			_questions.Add(question);
		}
	}

	private async void StopGameClicked(object sender, EventArgs e)
	{
		await _participant.SetGameToClose();
		await Navigation.PushAsync(new GameStatisticsPage(_participant));
	}

	private async void OnFrameTapped(object sender, EventArgs e)
	{
		if (_questionsareclickable) {
			_questionsareclickable = false;
			StopGameButton.IsEnabled = false;

			var tappedFrame = sender as Frame;
			if (tappedFrame != null)
			{

				var tappedQuestion = tappedFrame.BindingContext as Question;
				if (tappedQuestion != null)
				{
					if (tappedQuestions.Contains(tappedQuestion.QuestionId))
					{
						_questionsareclickable = true;

						await DisplayAlert("Waarschuwing", "Deze vraag is al gespeeld. Je kan de vraag niet nog een keer spelen.", "OK");
					}
					else
					{
						tappedQuestions.Add(tappedQuestion.QuestionId);
						tappedFrame.BackgroundColor = Colors.Green;

						await SetCurrentQuestion(tappedQuestion);

						if (await CheckIfEveryParticipantAnswered(tappedQuestion))
						{
							_questionsareclickable = true;
							StopGameButton.IsEnabled = true;
						}

						if (tappedQuestions.Count == _questions.Count)
						{
							if (await CheckIfSessionDone())
							{
								await Navigation.PushAsync(new GameStatisticsPage(_participant));
							}
						}
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
}
