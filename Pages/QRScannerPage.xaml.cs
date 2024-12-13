using TruthOrDrink.Model;
using TruthOrDrink.Pages;
using TruthOrDrink.DataAccessLayer;


namespace TruthOrDrink
{
	public partial class QRScannerPage : ContentPage
	{
		private readonly Participant _participant;

		public QRScannerPage(Participant participant)
		{
			InitializeComponent();
			_participant = participant;
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
			int sessioncode = Int32.Parse(first.Value);
			SupabaseService supabaseService = new SupabaseService();
			Session session = new Session(sessioncode);
			bool sessionExists = await session.CheckIfSessionExistsAsync();

			if (sessionExists)
			{
					Participant participant = new Participant(_participant.ParticipantId, sessioncode);
					await participant.JoinParticipantToSession();

					if (await session.CheckIfCustomGame()) {
						await Navigation.PushAsync(new QuestionInputPage(participant));
					}

					else {
						await Navigation.PushAsync(new WaitOnHostPage(participant));
					}
			}
			else 
			{
				await DisplayAlert("Ongeldige gamecode", "Verifiëer of de host al een spel heeft gemaakt.", "OK");
				await Navigation.PopAsync();
			}
			});
		}

	}
}

