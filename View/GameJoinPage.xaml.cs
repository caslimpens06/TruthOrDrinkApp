using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class GameJoinPage : ContentPage
	{
		public GameJoinPage(Session session)
		{
			InitializeComponent();
			BindingContext = new GameJoinViewModel(session);
		}
	}
}