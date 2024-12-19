using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class HostChooseGameViewModel : ObservableObject
	{
		private readonly Host _host;

		public HostChooseGameViewModel(Host host)
		{
			_host = host;
			NavigateToGame1Command = new AsyncRelayCommand(NavigateToGame1);
			NavigateToGame2Command = new AsyncRelayCommand(NavigateToGame2);
			NavigateToGame3Command = new AsyncRelayCommand(NavigateToGame3);
			NavigateToGame4Command = new AsyncRelayCommand(NavigateToGame4);
			NavigateToGame5Command = new AsyncRelayCommand(NavigateToGame5);
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
		}

		public IAsyncRelayCommand NavigateToGame1Command { get; }
		public IAsyncRelayCommand NavigateToGame2Command { get; }
		public IAsyncRelayCommand NavigateToGame3Command { get; }
		public IAsyncRelayCommand NavigateToGame4Command { get; }
		public IAsyncRelayCommand NavigateToGame5Command { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		private async Task NavigateToGame1() => await NavigateToGame(1);
		private async Task NavigateToGame2() => await NavigateToGame(2);
		private async Task NavigateToGame3() => await NavigateToGame(3);
		private async Task NavigateToGame4() => await NavigateToGame(4);
		private async Task NavigateToGame5() => await NavigateToGame(5);

		private async Task NavigateToGame(int gameId)
		{
			Session session = new Session(_host.HostId, gameId);
			await Application.Current.MainPage.Navigation.PushAsync(new GameJoinPage(session));
		}

		private async Task LeaveGame()
		{
			await Application.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}