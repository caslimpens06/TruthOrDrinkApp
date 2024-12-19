using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class OfflineGameViewModel : ObservableObject
	{
		public OfflineGameViewModel()
		{
			JoinGame1Command = new AsyncRelayCommand(async () => await JoinGame(new Session(1)));
			JoinGame2Command = new AsyncRelayCommand(async () => await JoinGame(new Session(2)));
			JoinGame3Command = new AsyncRelayCommand(async () => await JoinGame(new Session(3)));
			JoinGame4Command = new AsyncRelayCommand(async () => await JoinGame(new Session(4)));
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
		}

		public IAsyncRelayCommand JoinGame1Command { get; }
		public IAsyncRelayCommand JoinGame2Command { get; }
		public IAsyncRelayCommand JoinGame3Command { get; }
		public IAsyncRelayCommand JoinGame4Command { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		private async Task JoinGame(Session session)
		{
			await App.Current.MainPage.Navigation.PushAsync(new GameJoinPage(session));
		}

		private async Task LeaveGame()
		{
			await App.Current.MainPage.DisplayAlert("Verlaat spel", "Je hebt het spel verlaten.", "OK");
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}