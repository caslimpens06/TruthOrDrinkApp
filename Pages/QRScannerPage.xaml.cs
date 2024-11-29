using Microsoft.Maui.Controls;
using Camera.MAUI;
using System.Runtime.CompilerServices;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;
using Microsoft.IdentityModel.Tokens;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;


namespace TruthOrDrink
{
	public partial class QRScannerPage : ContentPage
	{
		private int _participantid;

		public QRScannerPage(int participantid)
		{
			InitializeComponent();
			_participantid = participantid;
			BarcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
			{
				Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
				AutoRotate = true,
				Multiple = true,
			};
		}


		private bool isBarcodeProcessed = false;

		private void Barcodereader_BarcodeDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs args)
		{
			var first = args.Results?.FirstOrDefault();

			if (first == null || isBarcodeProcessed)
			{
				return;
			}

			isBarcodeProcessed = true;

			Dispatcher.DispatchAsync(async () =>
			{
			int gamecode = Int32.Parse(first.Value);
			SupabaseService supabaseService = new SupabaseService();
			bool gameExists = await supabaseService.CheckIfSessionExistsAsync(gamecode);

			if (gameExists)
			{
					Participant participant = new Participant(_participantid, gamecode);
					Session session = new Session(gamecode);
					await supabaseService.JoinParticipantToSession(participant);

					if (await session.CheckIfCustomGame()) {
						await Navigation.PushModalAsync(new QuestionInputPage(participant));
					}

					else {
						await Navigation.PushModalAsync(new WaitOnHostPage(participant));
					}
			}
			else 
			{
				await DisplayAlert("Ongeldige gamecode", "VerifiŽer of de host al een spel heeft gemaakt.", "OK");
			}
			});
		}

	}
}

