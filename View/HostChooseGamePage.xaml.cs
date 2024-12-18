using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class HostChooseGamePage : ContentPage
	{
		public HostChooseGamePage(Host host)
		{
			InitializeComponent();
			BindingContext = new HostChooseGameViewModel(host);
		}
	}
}