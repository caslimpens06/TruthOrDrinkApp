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
		public GameStatisticsParticipantPage(List<Participant> participants)
		{
			InitializeComponent();
			BindingContext = new GameStatisticsParticipantViewModel(participants);
		}
		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}