﻿using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class QuestionInputViewModel : INotifyPropertyChanged
	{
		private string _questionText;
		private readonly Participant _participant;

		public QuestionInputViewModel(Participant participant)
		{
			_participant = participant;
			SendQuestionCommand = new Command(OnSendQuestion);
			DoneAddingQuestionsCommand = new Command(OnDoneAddingQuestions);
		}

		public string QuestionText
		{
			get => _questionText;
			set
			{
				_questionText = value;
				OnPropertyChanged(nameof(QuestionText));
			}
		}

		public ICommand SendQuestionCommand { get; }
		public ICommand DoneAddingQuestionsCommand { get; }

		private async void OnSendQuestion()
		{
			if (string.IsNullOrWhiteSpace(QuestionText))
			{
				await Application.Current.MainPage.DisplayAlert("Fout", "Voer een vraag in voordat je verder gaat.", "OK");
				return;
			}

			Question question = new Question(QuestionText);
			await question.AddQuestionByParticipant();
			await Application.Current.MainPage.DisplayAlert("Vraag verzonden", $"De volgende vraag is verstuurd: {question.Text}", "OK");
			QuestionText = string.Empty;
		}

		private async void OnDoneAddingQuestions()
		{
			await _participant.SetDoneAddingQuestions();
			await Application.Current.MainPage.Navigation.PushModalAsync(new WaitOnHostPage(_participant));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}