using Microsoft.Maui.Controls;
using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class GuestIdentifierPage : ContentPage
	{
		public GuestIdentifierPage()
		{
			InitializeComponent();
			BindingContext = new GuestIdentifierViewModel();
		}
	}
}