using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class WaitOnHostPage : ContentPage
	{
		public WaitOnHostPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new WaitOnHostViewModel(participant, Navigation);
		}

		protected override bool OnBackButtonPressed()
		{
			return true; // Prevent back navigation
		}
	}
}