﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

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

		public GuestViewModel()
		{
			// For the object binding to work.
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
				await App.Current.MainPage.DisplayAlert("Ongeldige sessiecode", "Voer een geldige 6-cijferige sessiecode in.", "OK");
				return;
			}

			int parsedSessionCode = Int32.Parse(SessionCode);
			Session session = new Session(parsedSessionCode);

			if (await session.CheckIfSessionExistsAsync())
			{
				bool hasStarted = await session.CheckIfSessionHasStarted();
				if (!hasStarted)
				{
					if (await session.CheckMaxPlayerCount())
					{
						Participant participant = new Participant(_participant.ParticipantId, parsedSessionCode, _participant.Name);
					await participant.JoinParticipantToSession();

					if (await session.CheckIfCustomGame())
					{
						await App.Current.MainPage.Navigation.PushAsync(new QuestionInputPage(participant));
					}
					else
					{
						App.Vibrate();
						await App.Current.MainPage.Navigation.PushAsync(new WaitOnHostPage(participant));
					}
					}
					else
					{
						await App.Current.MainPage.DisplayAlert("Fout", "Je kan niet meer deelnemen, er zitten teveel deelnemers in het spel.", "OK");
					}
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Fout", "Je kan niet meer deelnemen, het spel is al gestart.", "OK");
				}
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Ongeldige sessiecode", "Verifieer of de host al een spel heeft gemaakt.", "OK");
			}
		}

		private async Task ScanQRCode()
		{
			await App.Current.MainPage.Navigation.PushAsync(new QRScannerPage(_participant));
		}

		private async Task LeaveGame()
		{
			await _participant.RemoveParticipantAsync();
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}
	}
}