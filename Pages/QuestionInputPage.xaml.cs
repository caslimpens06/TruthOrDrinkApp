using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink
{
	public partial class QuestionInputPage : ContentPage
	{

		private Participant _participant;
		public QuestionInputPage(Participant participant)
		{
			InitializeComponent();
			_participant = participant;
		}

		private async void OnSendQuestionClicked(object sender, EventArgs e)
		{
			Question question = new Question(QuestionEntry.Text);

			if (string.IsNullOrWhiteSpace(question.Text))
			{
				await DisplayAlert("Fout", "Voer een vraag in voordat je verder gaat.", "OK");
				return;
			}

			await question.AddQuestionByParticipant();
			await DisplayAlert("Vraag verzonden", $"De volgende vraag is verstuurd: {question.Text}", "OK");
			QuestionEntry.Text = string.Empty;
		}

		private async void DoneAddingQuestionsClicked(object sender, EventArgs e)
		{
			DoneAddingQuestions.IsEnabled = false;
			await _participant.SetDoneAddingQuestions();
			await Navigation.PushModalAsync(new WaitOnHostPage(_participant));
		}
	}
}
