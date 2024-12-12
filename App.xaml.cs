using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink
{
	public partial class App : Application
	{
		private SQLiteService sqlliteservice = new SQLiteService();

		public App()
		{
			InitializeComponent();

			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

			CreateLocalDatabaseTables();

			MainPage = new NavigationPage(new WelcomePage());

			CheckInternetConnectionOnStart();
		}

		private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
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

		private void CheckInternetConnectionOnStart()
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

		private async Task CreateLocalDatabaseTables() 
		{
			await sqlliteservice.InitializeAsync();
		}


		private static void CloseApp()
		{
			Environment.Exit(0);
		}
	}
}
