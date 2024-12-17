using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class GuestViewModel : ObservableObject
	{
		private readonly Participant _participant;
		private string _sessionCode;

		public GuestViewModel(Participant participant)
		{
			_participant = participant;
			ConnectCommand = new AsyncRelayCommand(Connect);
			QRCommand = new AsyncRelayCommand(ScanQRCode);
			LeaveGameCommand = new AsyncRelayCommand(LeaveGame);
		}

		public string SessionCode
		{
			get => _sessionCode;
			set => SetProperty(ref _sessionCode, value);
		}

		public IAsyncRelayCommand ConnectCommand { get; }
		public IAsyncRelayCommand QRCommand { get; }
		public IAsyncRelayCommand LeaveGameCommand { get; }

		private async Task Connect()
		{
			if (string.IsNullOrWhiteSpace(SessionCode) || SessionCode.Length != 6)
			{
				await Application.Current.MainPage.DisplayAlert("Invalid Game Code", "Please enter a valid 6-digit game code.", "OK");
				return;
			}

			int parsedSessionCode = int.Parse(SessionCode);
			Session session = new Session(parsedSessionCode);

			if (await session.CheckIfSessionExistsAsync())
			{
				bool hasStarted = await session.CheckIfSessionHasStarted();
				if (!hasStarted)
				{
					await _participant.JoinParticipantToSession();
					await Application.Current.MainPage.Navigation.PushAsync(new WaitOnHostPage(_participant));
				}
				else
				{
					await Application.Current.MainPage.DisplayAlert("Error", "Unfortunately, you cannot join anymore.", "OK");
				}
			}
			else
			{
				await Application.Current.MainPage.DisplayAlert("Invalid Game Code", "Please verify if the host has created a game.", "OK");
			}
		}

		private async Task ScanQRCode()
		{
			await Application.Current.MainPage.Navigation.PushAsync(new QRScannerPage(_participant));
		}

		private async Task LeaveGame()
		{
			await _participant.RemoveParticipantAsync();
			await Application.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}