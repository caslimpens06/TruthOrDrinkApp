using System.Collections.ObjectModel;
using TruthOrDrink.Model;
using TruthOrDrink.ViewModels;

namespace TruthOrDrink.Pages
{
	public partial class HostControlsGamePage : ContentPage
	{
		public HostControlsGamePage(Session session)
		{
			InitializeComponent();
			BindingContext = new HostControlsGameViewModel(session);
		}

		private async void OnFrameTapped(object sender, EventArgs e)
		{
			var tappedFrame = sender as Frame;
			if (tappedFrame != null)
			{
				var tappedQuestion = tappedFrame.BindingContext as Question;
				if (tappedQuestion != null)
				{
					await ((HostControlsGameViewModel)BindingContext).OnQuestionTapped(tappedQuestion);
				}
			}
		}
	}
}