using System.Collections.ObjectModel;
using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class HostControlsGamePage : ContentPage
{
	private readonly ObservableCollection<Question> _questions = new();
	private readonly Game _game;
	private List<int> tappedQuestions = new List<int>();
	private readonly Session _session;

	public HostControlsGamePage(Session session)
	{
		InitializeComponent();
		_game = new Game(session.GameId);
		BindingContext = this;
		_session = session;

		_ = LoadQuestionsAsync();
	}

	public ObservableCollection<Question> Questions => _questions;

	public string SelectedQuestion { get; set; }

	private async Task LoadQuestionsAsync()
	{
		_questions.Clear();
		var questionsList = await _game.GetQuestionsAsync();

		foreach (Question question in questionsList)
		{
			// Add the Question object to the collection
			_questions.Add(question);
		}
	}

	private void StopGame_Clicked(object sender, EventArgs e)
	{
		// Stop the game and perform necessary cleanup
	}

	private void OnFrameTapped(object sender, EventArgs e)
	{
		var tappedFrame = sender as Frame;
		if (tappedFrame != null)
		{
			var tappedQuestion = tappedFrame.BindingContext as Question;

			if (tappedQuestion != null)
			{
				if (tappedQuestions.Contains(tappedQuestion.QuestionId))
				{
					DisplayAlert("Waarschuwing", "Deze vraag is al gespeeld. Je kan de vraag niet nog een keer spelen.", "OK");
				}
				else
				{
					tappedQuestions.Add(tappedQuestion.QuestionId);
					tappedFrame.BackgroundColor = Colors.Green;
					tappedQuestion.SetCurrentQuestion(_session);
				}
			}
		}
	}
}
