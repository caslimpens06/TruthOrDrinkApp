using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class LoginPage : ContentPage
	{
		private LoginViewModel ViewModel => BindingContext as LoginViewModel;

		public LoginPage()
		{
			InitializeComponent();
			BindingContext = new LoginViewModel();
		}

		protected override bool OnBackButtonPressed()
		{
			// Disable back button during login process
			return ViewModel?.IsBackButtonDisabled ?? false;
		}
	}
}
