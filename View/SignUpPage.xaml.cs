using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class SignUpPage : ContentPage
	{
		private SignUpViewModel ViewModel => BindingContext as SignUpViewModel;

		public SignUpPage()
		{
			InitializeComponent();
			BindingContext = new SignUpViewModel();
		}

		protected override bool OnBackButtonPressed()
		{
			// Disable back button during login process
			return ViewModel?.IsBackButtonDisabled ?? false;
		}
	}
}
