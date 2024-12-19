using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View;

public partial class ControlOfflineGamePage : ContentPage
{
	public ControlOfflineGamePage(Session session, List<Participant> participants)
	{
		InitializeComponent();
		BindingContext = new ControlOfflineGameViewModel(session, participants);
	}
}