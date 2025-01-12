using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink
{
	public partial class App : Application
	{
		private readonly SQLiteService sqliteservice = new SQLiteService();

		public App()
		{
			InitializeComponent();
			MainPage = new NavigationPage(new WelcomePage());
			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
			
			CreateLocalDatabase();

			CheckInternetConnectionOnStart();
			Console.WriteLine("after check conn");
		}

		private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
		{
			if (e.NetworkAccess == NetworkAccess.None)
			{
				Console.WriteLine("Network lost. Switching to offline mode.");

				if (MainPage != null && MainPage.Dispatcher != null)
				{
					MainPage.Dispatcher.Dispatch(() =>
					{
						MainPage = new NavigationPage(new OfflineMode());
					});
				}
				else
				{
					Console.WriteLine("MainPage or Dispatcher not available. Setting MainPage directly.");
					MainPage = new NavigationPage(new OfflineMode());
				}
			}
		}

		private void CheckInternetConnectionOnStart()
		{
			if (Connectivity.NetworkAccess == NetworkAccess.None)
			{
				Console.WriteLine("No internet detected. Navigating to offline mode.");
				MainPage = new NavigationPage(new OfflineMode());
			}
			else
			{
				Console.WriteLine("Internet detected. Navigating to Welcome Page.");
				MainPage = new NavigationPage(new WelcomePage());
			}
		}

		private async Task GetLocationAsync()
		{
			try
			{
				if (Connectivity.NetworkAccess == NetworkAccess.None)
				{
					Console.WriteLine("No Connection. Location failed.");
					return;
				}

				var location = await Geolocation.GetLastKnownLocationAsync();

				if (location != null)
				{
					var placemarks = await Geocoding.GetPlacemarksAsync(location);

					if (placemarks != null && placemarks.Any())
					{
						var placemark = placemarks.FirstOrDefault();
						if (placemark != null)
						{
							Console.WriteLine($"Province/State: {placemark.AdminArea}");
							Console.WriteLine($"Country: {placemark.CountryName}");

							Settings settings = new Settings(placemark.CountryName, placemark.AdminArea);
							await settings.SaveLocationLocallyAsync();
						}
						else
						{
							Console.WriteLine("No placemarks found.");
						}
					}
					else
					{
						Console.WriteLine("No placemarks found.");
					}
				}
				else
				{
					Console.WriteLine("Location not available.");
				}
			}
			catch (PermissionException)
			{
				Console.WriteLine("Locatiepermissie geweigerd.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"ERROR: {ex.Message}");
			}
		}


		private async Task CreateLocalDatabase() 
		{
			await sqliteservice.InitializeAsync();
			GetLocationAsync(); // don't await because system can continue without fully loading the questions for offline mode
			sqliteservice.PopulateQuestionsForOfflineGame(); // don't await because system can continue without fully loading the questions for offline mode
			sqliteservice.PopulateDrinks(); // don't await because system can continue without fully loading the questions for offline mode
			Console.WriteLine("Initialisation PASS");
		}

		public static void Vibrate() 
		{
			Vibration.Vibrate(TimeSpan.FromMilliseconds(200));
		}

		public static void CloseApp()
		{
			Environment.Exit(0);
		}
	}
}
