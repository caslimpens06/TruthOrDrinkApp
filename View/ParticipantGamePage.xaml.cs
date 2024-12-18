using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class ParticipantGamePage : ContentPage
	{
		public ParticipantGamePage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new ParticipantGameViewModel(participant);
		}
	}
}