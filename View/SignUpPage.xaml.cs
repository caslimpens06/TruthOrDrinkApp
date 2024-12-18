using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class SignUpPage : ContentPage
	{
		public SignUpPage()
		{
			InitializeComponent();
			BindingContext = new SignUpViewModel();
		}
	}
}