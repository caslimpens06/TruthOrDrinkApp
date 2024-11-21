using Microsoft.Maui.Controls;
using Camera.MAUI;
using System.Runtime.CompilerServices;
using ZXing.Net.Maui.Controls;
using ZXing.Net.Maui;

namespace TruthOrDrink
{
	public partial class QRScanner : ContentPage
	{
		public QRScanner()
		{
			InitializeComponent();
			BarcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
			{
				Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
				AutoRotate = true,
				Multiple = true
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
				await DisplayAlert("Barcode gedetecteerd", "Spelcode: " + first.Value, "Ok");
				//await Navigation.PushAsync(new GamePage());
			});
		}

	}
}

