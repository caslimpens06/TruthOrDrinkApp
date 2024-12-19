using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class GameStatisticsHostPage : ContentPage
	{
		public GameStatisticsHostPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new GameStatisticsHostViewModel(participant);
		}
		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}