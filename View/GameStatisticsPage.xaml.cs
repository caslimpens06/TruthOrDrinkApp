using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class GameStatisticsPage : ContentPage
	{
		public GameStatisticsPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new GameStatisticsViewModel(participant);
		}
	}
}