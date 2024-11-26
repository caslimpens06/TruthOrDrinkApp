namespace TruthOrDrink
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

			// Stel de startpagina in op WelcomePage
			MainPage = new WelcomePage();
		}

		public async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			// Controleer of de verbinding wegvalt tijdens gebruik
			if (e.NetworkAccess == NetworkAccess.None)
			{
				await MainPage.DisplayAlert("Geen internet", "De internetverbinding is verbroken. De app wordt afgesloten.", "OK");

				CloseApp();
			}
		}
		public async void CheckInternetConnectionOnStart()
		{
			// Controleer netwerktoegang
			if (Connectivity.NetworkAccess == NetworkAccess.None)
			{
				// Toon melding via de WelcomePage
				await MainPage.DisplayAlert("Geen internet", "Er is geen internetverbinding. De app wordt afgesloten.", "OK");

				CloseApp();
			}
		}
		public void CloseApp()
		{
			Environment.Exit(0);
		}
	}
}
