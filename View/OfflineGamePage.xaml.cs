using TruthOrDrink.ViewModels;

namespace TruthOrDrink.View;

public partial class OfflineGamePage : ContentPage
{
	public OfflineGamePage()
	{
		InitializeComponent();
		BindingContext = new OfflineGameViewModel();
	}
}
