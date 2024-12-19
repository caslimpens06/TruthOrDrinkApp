using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class WelcomePage : ContentPage
	{
		public WelcomePage()
		{
			InitializeComponent();
			BindingContext = new WelcomePageViewModel();
		}
	}
}