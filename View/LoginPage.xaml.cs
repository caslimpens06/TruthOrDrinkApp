using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();
			BindingContext = new LoginViewModel();
		}
	}
}