using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View;

public partial class OfflineGameStatisticsPage : ContentPage
{
	public OfflineGameStatisticsPage(List<Participant> participants)
	{
		InitializeComponent();
		BindingContext = new OfflineGameStatisticsViewModel(participants);
	}
	protected override bool OnBackButtonPressed()
	{
		return true;
	}
}