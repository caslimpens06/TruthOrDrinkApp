using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink.View
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage(Host host)
		{
			InitializeComponent();
			BindingContext = new SettingsPageViewModel(host);
		}
	}
}
