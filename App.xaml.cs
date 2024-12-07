namespace TruthOrDrink
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

			MainPage = new NavigationPage(new WelcomePage());

			CheckInternetConnectionOnStart();
		}

		public void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
		{
			if (e.NetworkAccess == NetworkAccess.None && MainPage != null)
			{
				MainPage.Dispatcher.Dispatch(async () =>
				{
					await MainPage.DisplayAlert("Geen Internet :(", "De app zal zichzelf afsluiten.", "OK");
					CloseApp();
				});
			}
		}

		public void CheckInternetConnectionOnStart()
		{
			if (Connectivity.NetworkAccess == NetworkAccess.None && MainPage != null)
			{
				MainPage.Dispatcher.Dispatch(async () =>
				{
					await MainPage.DisplayAlert("Geen Internet :(", "De app zal zichzelf afsluiten.", "OK");
					CloseApp();
				});
			}
		}

		public static void CloseApp()
		{
			Environment.Exit(0);
		}
	}
}
