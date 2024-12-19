using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View
{
	public partial class OfflineMode : FlyoutPage
	{
		public OfflineMode()
		{
			InitializeComponent();
			BindingContext = new OfflineModeViewModel();
		}

		protected override bool OnBackButtonPressed()
		{
			IsPresented = !IsPresented;
			return true;
		}
	}
}