using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class WelcomePageViewModel : INotifyPropertyChanged
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private Host _host;

		public Host Host
		{
			get => _host;
			set
			{
				_host = value;
				OnPropertyChanged(nameof(Host));
			}
		}

		public ICommand NavigateToSignupCommand { get; }
		public ICommand NavigateToHostCommand { get; }
		public ICommand NavigateToParticipantCommand { get; }

		public WelcomePageViewModel()
		{

			NavigateToSignupCommand = new Command(async () => await NavigateToSignup());
			NavigateToHostCommand = new Command(async () => await NavigateToHost());
			NavigateToParticipantCommand = new Command(async () => await NavigateToParticipant());

			CheckLoginStatus();
		}

		private async void CheckLoginStatus()
		{
			Host = await _sqliteService.GetHostAsync();

			if (Host != null)
			{
				await App.Current.MainPage.Navigation.PushAsync(new HostMainPage(Host));
			}
		}

		private async Task NavigateToSignup()
		{
			await App.Current.MainPage.Navigation.PushAsync(new SignUpPage());
		}

		private async Task NavigateToHost()
		{
			await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
		}

		private async Task NavigateToParticipant()
		{
			await App.Current.MainPage.Navigation.PushAsync(new GuestIdentifierPage());
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}