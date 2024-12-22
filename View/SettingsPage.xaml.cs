using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage()
		{
			InitializeComponent();
			BindingContext = new SettingsPageViewModel();
		}
	}
}
