using Microsoft.Maui.Controls;
using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class HostMainPage : FlyoutPage
	{
		public HostMainPage(Host host)
		{
			InitializeComponent();
			BindingContext = new HostMainViewModel(host);
		}

		protected override bool OnBackButtonPressed()
		{
			IsPresented = !IsPresented; // Toggle the flyout menu
			return true; // Prevent back navigation
		}
	}
}