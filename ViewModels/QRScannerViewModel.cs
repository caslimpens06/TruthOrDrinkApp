using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels;

public partial class QRScannerViewModel : ObservableObject
{
	private readonly Participant _participant;

	public QRScannerViewModel(Participant participant)
	{
		_participant = participant;
	}

	private bool _isProcessing;
	private string _lastProcessedBarcode;

	[RelayCommand]
	public async Task ProcessBarcode(string barcodeValue)
	{
		if (_isProcessing || barcodeValue == _lastProcessedBarcode) return;

		_isProcessing = true;
		_lastProcessedBarcode = barcodeValue;

		try
		{
			if (int.TryParse(barcodeValue, out int sessionCode))
			{
				Session session = new Session(sessionCode);

				if (await session.CheckIfSessionExistsAsync())
				{
					bool hasStarted = await session.CheckIfSessionHasStarted();
					if (!hasStarted)
					{
						if (await session.CheckMaxPlayerCount())
						{
							Participant participant = new Participant(_participant.ParticipantId, sessionCode, _participant.Name);
							await participant.JoinParticipantToSession();

							if (await session.CheckIfCustomGame())
							{
								App.Vibrate();
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
						await App.Current.MainPage.Navigation.PopAsync();
					}
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Ongeldige sessiecode", "Verifieer of de host al een spel heeft gemaakt.", "OK");
				}
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Ongeldige sessiecode", "De gescande code is niet geldig.", "OK");
			}
		}
		finally
		{
			_isProcessing = false;
		}
	}
}
