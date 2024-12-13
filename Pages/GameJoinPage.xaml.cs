using QRCoder;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink
{
	public partial class GameJoinPage : ContentPage
	{
		private readonly int _hostid;
		private readonly int _gameid;
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
			int NewCodeFor = newCode;
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


		private async void Play(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new HostJoinParticipantPage(_session));
		}
		
		
	}
}
