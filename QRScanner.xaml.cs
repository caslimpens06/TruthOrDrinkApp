using Microsoft.Maui.Controls;
using Camera.MAUI;
using System.Runtime.CompilerServices;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;
using Microsoft.IdentityModel.Tokens;


namespace TruthOrDrink
{
	public partial class QRScanner : ContentPage
	{
		private int _participantid;

		public QRScanner(int participantid)
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
			bool gameExists = await supabaseService.CheckIfGameExistsAsync(gamecode);

			if (gameExists)
			{
				await supabaseService.JoinParticipantToGame(_participantid, gamecode);
				await Navigation.PushModalAsync(new GamePage());
			}
			else 
			{
				await DisplayAlert("Ongeldige gamecode", "Verifiëer of de host al een spel heeft gemaakt.", "OK");
			}
			});
		}

	}
}

