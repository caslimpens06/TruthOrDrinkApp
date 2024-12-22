using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using QRCoder;

namespace TruthOrDrink.ViewModels
{
	public class GameJoinViewModel : ObservableObject
	{
		private readonly int _hostid;
		private readonly int _gameid;
		private string _gameCode;
		private Session _session;
		private string _qrCodeImage;
		private bool _isPlayButtonEnabled;

		public GameJoinViewModel(Session session)
		{
			_hostid = session.SessionCode;
			_gameid = session.GameId;
			_session = session;
			GenerateGameCodeCommand = new AsyncRelayCommand(GenerateGameCode);
			PlayCommand = new AsyncRelayCommand(Play);
			IsPlayButtonEnabled = false;
			GenerateGameCodeCommand.Execute(null);
		}

		public string GameCode
		{
			get => _gameCode;
			set => SetProperty(ref _gameCode, value);
		}

		public string QrCodeImage
		{
			get => _qrCodeImage;
			set => SetProperty(ref _qrCodeImage, value);
		}

		public bool IsPlayButtonEnabled
		{
			get => _isPlayButtonEnabled;
			set => SetProperty(ref _isPlayButtonEnabled, value);
		}

		public IAsyncRelayCommand GenerateGameCodeCommand { get; }
		public IAsyncRelayCommand PlayCommand { get; }

		private async Task GenerateGameCode()
		{
			int newCode = await GenerateUniqueGameCode();
			QrCodeImage = GenerateQrCode(newCode);
			IsPlayButtonEnabled = true;
			GameCode = "Sessiecode: " + newCode.ToString();

			_session = new Session(newCode, _hostid, _gameid);
			_session.AddSessionToDatabase();
		}

		private async Task<int> GenerateUniqueGameCode()
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

		private string GenerateQrCode(int code)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(code.ToString(), QRCodeGenerator.ECCLevel.L);

			// Generate the QR code as a PNG byte array
			PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
			byte[] qrCodeBytes = qrCode.GetGraphic(20);

			return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
		}

		private async Task Play()
		{
			await App.Current.MainPage.Navigation.PushAsync(new HostJoinParticipantPage(_session));
		}
	}
}