using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class OfflineGamePage : ContentPage
	{
		public OfflineGamePage()
		{
			InitializeComponent();
			BindingContext = new OfflineGameViewModel();
		}
	}
}