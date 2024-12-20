using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class GameStatisticsParticipantPage : ContentPage
	{
		public GameStatisticsParticipantPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new GameStatisticsParticipantViewModel(participant);
		}
		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}