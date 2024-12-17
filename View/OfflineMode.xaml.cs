using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class OfflineMode : ContentPage
	{
		public OfflineMode()
		{
			InitializeComponent();
			BindingContext = new OfflineModeViewModel();
		}
	}
}