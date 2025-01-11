using Camera.MAUI;
using ZXing.Net.Maui.Controls;
namespace TruthOrDrink
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
			var builder = MauiApp.CreateBuilder();

			builder
				.UseMauiApp<App>()
				.UseMauiCameraView()
				.UseBarcodeReader();
			builder.Services.AddSingleton<IAccelerometer>(Accelerometer.Default);
			return builder.Build();
		}
    }
}
