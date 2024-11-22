using QRCoder;

namespace TruthOrDrink
{
	public partial class GameHostPage : ContentPage
	{
		private static List<string> _existingCodes = new List<string>();

		public GameHostPage()
		{
			InitializeComponent();
			GenerateGameCodeAndQrCode();
		}

		private void GenerateGameCodeAndQrCode()
		{
			string newCode = GenerateUniqueGameCode();
			GameCodeLabel.Text = $"Gamecode: {newCode}";

			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(newCode, QRCodeGenerator.ECCLevel.L);
			PngByteQRCode qRCode = new PngByteQRCode(qrCodeData);
			byte[] qrCodeBytes = qRCode.GetGraphic(20);

			QrCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
		}

		private string GenerateUniqueGameCode()
		{
			string newCode;
			Random random = new Random();

			do
			{
				newCode = random.Next(100000, 999999).ToString();
			}
			while (_existingCodes.Contains(newCode));

			_existingCodes.Add(newCode);

			return newCode;
		}

		private void ButtonPlay_Clicked(object sender, EventArgs e)
		{
			// Start game
		}
	}
}
