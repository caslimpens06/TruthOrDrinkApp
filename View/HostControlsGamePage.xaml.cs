using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class HostControlsGamePage : ContentPage
	{
		public HostControlsGamePage(Session session)
		{
			InitializeComponent();
			BindingContext = new HostControlsGameViewModel(session);
		}
	}
}