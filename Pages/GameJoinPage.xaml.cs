using QRCoder;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class GameJoinPage : ContentPage
	{
		private int _hostid;
		private int NewCodeFor;
		private int _gameid;
		private Session _session;

		public GameJoinPage(Session session)
		{
			InitializeComponent();
			_hostid = session.SessionCode;
			_gameid = session.GameId;
			GenerateGameCode();

		}

		private async void GenerateGameCode()
		{
			int newCode = await GenerateUniqueGameCode();
			NewCodeFor = newCode;
			SupabaseService supabaseService = new SupabaseService();
			
			Session session = new Session(newCode, _hostid, _gameid);
			_session = session;

			session.AddSessionToDatabase();
			
			GameCodeLabel.Text = $"Gamecode: {newCode.ToString()}";

			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(newCode.ToString(), QRCodeGenerator.ECCLevel.L);
			PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
			byte[] qrCodeBytes = qRCode.GetGraphic(20);
			QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
			PlayButton.IsEnabled = true;
		}

		private static async Task<int> GenerateUniqueGameCode()
		{
			int newCode;
			SupabaseService supabaseService = new SupabaseService();

			List<int> existingGameIds = await supabaseService.GetExistingSessionIdsAsync();

			do
			{
				newCode = new Random().Next(100000, 1000000);
			}
			while (existingGameIds.Contains(newCode));

			return newCode;
		}


		private void Play(object sender, EventArgs e)
		{
			Navigation.PushAsync(new HostJoinParticipantPage(_session));
		}
		
		
	}
}
