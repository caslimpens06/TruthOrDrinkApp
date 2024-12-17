using Microsoft.Maui.Controls;
using TruthOrDrink.ViewModels;
using TruthOrDrink.Model;

namespace TruthOrDrink
{
	public partial class QRScannerPage : ContentPage
	{
		public QRScannerPage(Participant participant)
		{
			InitializeComponent();
			BindingContext = new QRScannerViewModel(participant);
		}

		private void OnBarcodeDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs args)
		{
			var viewModel = BindingContext as QRScannerViewModel;
			viewModel?.ProcessBarcodeCommand.Execute(args.Results.FirstOrDefault()?.Value);
		}
	}
}