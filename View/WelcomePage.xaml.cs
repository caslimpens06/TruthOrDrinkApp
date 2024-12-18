using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class WelcomePage : ContentPage
	{
		public WelcomePage()
		{
			InitializeComponent();
			BindingContext = new WelcomePageViewModel(Navigation);
		}
	}
}