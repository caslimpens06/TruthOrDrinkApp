using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class GuestPage : ContentPage
	{
		public GuestPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new GuestViewModel(participant);
		}
	}
}