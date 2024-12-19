using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink.View
{
	public partial class AddOfflineParticipantsPage : ContentPage
	{
		public AddOfflineParticipantsPage(Session session)
		{
			InitializeComponent();
			BindingContext = new AddOfflineParticipantsViewModel(session);
		}
	}
}
