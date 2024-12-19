using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;
using ZXing.Net.Maui;

namespace TruthOrDrink.View;

public partial class QRScannerPage : ContentPage
{
	public QRScannerPage(Participant participant)
	{
		InitializeComponent();
		BindingContext = new QRScannerViewModel(participant);
	}

	private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs args)
	{
		MainThread.BeginInvokeOnMainThread(() =>
			(BindingContext as QRScannerViewModel)?.ProcessBarcodeCommand.Execute(args.Results.FirstOrDefault()?.Value));
	}
}
