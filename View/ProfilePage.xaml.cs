using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class ProfilePage : ContentPage
	{
		public ProfilePage(Host host)
		{
			InitializeComponent();
			BindingContext = new ProfileViewModel(host);
		}
	}
}