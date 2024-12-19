using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class WaitOnHostPage : ContentPage
	{
		public WaitOnHostPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new WaitOnHostViewModel(participant);
		}

		protected override bool OnBackButtonPressed()
		{
			return true; // Prevent back navigation
		}
	}
}