using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
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
			IsPresented = !IsPresented;
			return true;
		}
	}
}