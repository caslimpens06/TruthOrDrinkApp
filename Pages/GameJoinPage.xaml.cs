using System.Linq;
using QRCoder;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class GameJoinPage : ContentPage
	{
		private Host _host;
		private Game _game;
		private int NewCodeFor;

		public GameJoinPage(Host host)
		{
			InitializeComponent();
			_host = host;
			GenerateGameCode();

		}

		private async void GenerateGameCode()
		{
			int newCode = await GenerateUniqueGameCode();
			NewCodeFor = newCode;
			SupabaseService supabaseService = new SupabaseService();
			Game _game = new Game(newCode, _host.HostId);
			await supabaseService.AddGameToDatabaseAsync(_game);
			
			GameCodeLabel.Text = $"Gamecode: {newCode.ToString()}";

			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(newCode.ToString(), QRCodeGenerator.ECCLevel.L);
			PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
			byte[] qrCodeBytes = qRCode.GetGraphic(20);
			QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
			PlayButton.IsEnabled = true;
		}

		private async Task<int> GenerateUniqueGameCode()
		{
			int newCode;
			SupabaseService supabaseService = new SupabaseService();

			var existingGameIds = await supabaseService.GetExistingGameIdsAsync();

			do
			{
				newCode = new Random().Next(100000, 1000000);
			}
			while (existingGameIds.Contains(newCode));

			return newCode;
		}


		private void Play(object sender, EventArgs e)
		{
			Game game = new Game(NewCodeFor, _host.HostId);
			Navigation.PushModalAsync(new HostControlGamePage(game));
		}
		
		
	}
}
