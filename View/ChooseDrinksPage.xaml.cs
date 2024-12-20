using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View;

public partial class ChooseDrinksPage : ContentPage
{
	public ChooseDrinksPage(Session session, List<Participant> participants)
	{
		InitializeComponent();
		BindingContext = new ChooseDrinksViewModel(session, participants);
	}
}