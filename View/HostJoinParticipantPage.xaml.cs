using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink
{
	public partial class HostJoinParticipantPage : ContentPage
	{
		public HostJoinParticipantPage(Session session)
		{
			InitializeComponent();
			BindingContext = new HostJoinParticipantViewModel(session);
		}
	}
}