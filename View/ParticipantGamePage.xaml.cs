using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class ParticipantGamePage : ContentPage
	{
		public ParticipantGamePage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new ParticipantGameViewModel(participant);
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}