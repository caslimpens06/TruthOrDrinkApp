using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class QuestionInputPage : ContentPage
	{
		public QuestionInputPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new QuestionInputViewModel(participant);
		}
	}
}