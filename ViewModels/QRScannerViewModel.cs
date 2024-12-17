using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public class QRScannerViewModel : ObservableObject
	{
		private readonly Participant _participant;

		public QRScannerViewModel(Participant participant)
		{
			_participant = participant;
			ProcessBarcodeCommand = new RelayCommand<string>(ProcessBarcode);
		}

		public IRelayCommand<string> ProcessBarcodeCommand { get; }

		private async void ProcessBarcode(string barcodeValue)
		{
			if (!string.IsNullOrEmpty(barcodeValue))
			{
				int sessionCode;
				if (int.TryParse(barcodeValue, out sessionCode))
				{
					Session session = new Session(sessionCode);
					if (await session.CheckIfSessionExistsAsync())
					{
						// Proceed to join the session or navigate to the appropriate page
						await App.Current.MainPage.Navigation.PushAsync(new ParticipantGamePage(_participant));
					}
					else
					{
						await App.Current.MainPage.DisplayAlert("Invalid Session", "The session does not exist.", "OK");
					}
				}
				else
				{
					await App.Current.MainPage.DisplayAlert("Invalid Code", "The scanned code is not valid.", "OK");
				}
			}
		}
	}
}