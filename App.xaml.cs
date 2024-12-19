using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Pages;
using TruthOrDrink.View;

namespace TruthOrDrink
{
	public partial class App : Application
	{
		private readonly SQLiteService sqliteservice = new SQLiteService();

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
					MainPage = new NavigationPage(new OfflineMode());
				});
			}
		}

		private void CheckInternetConnectionOnStart()
		{
			if (Connectivity.NetworkAccess == NetworkAccess.None && MainPage != null)
			{
				MainPage.Dispatcher.Dispatch(async () =>
				{
					MainPage = new NavigationPage(new OfflineMode());
				});
			}
		}

		private async Task CreateLocalDatabaseTables() 
		{
			await sqliteservice.InitializeAsync();
			await sqliteservice.PopulateQuestionsForOfflineGame();
		}


		public static void CloseApp()
		{
			Environment.Exit(0);
		}
	}
}
