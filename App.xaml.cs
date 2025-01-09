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

			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
			
			CreateLocalDatabase();
			MainPage = new NavigationPage(new WelcomePage());
			CheckInternetConnectionOnStart();

		}

		private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
		{
			if (e.NetworkAccess == NetworkAccess.None && MainPage != null && MainPage.Dispatcher != null)
			{
				MainPage.Dispatcher.Dispatch(() =>
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

		private async Task GetLocationAsync()
		{
			try
			{
				if (Connectivity.NetworkAccess == NetworkAccess.None)
				{
					Console.WriteLine("Geen internetverbinding. Kan locatie niet ophalen.");
					return;
				}

				var location = await Geolocation.GetLastKnownLocationAsync();

				if (location == null)
				{
					location = await Geolocation.GetLocationAsync(new GeolocationRequest
					{
						DesiredAccuracy = GeolocationAccuracy.Medium,
						Timeout = TimeSpan.FromSeconds(5)
					});
				}

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
					}
					else
					{
						Console.WriteLine("Geen placemarks gevonden.");
					}
				}
				else
				{
					Console.WriteLine("Locatie niet beschikbaar.");
				}
			}
			catch (PermissionException)
			{
				Console.WriteLine("Locatiepermissie geweigerd.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Fout bij ophalen locatie: {ex.Message}");
			}
		}


		private async Task CreateLocalDatabase() 
		{
			await sqliteservice.InitializeAsync();
			GetLocationAsync(); // don't await because system can continue without fully loading the questions for offline mode
			sqliteservice.PopulateQuestionsForOfflineGame(); // don't await because system can continue without fully loading the questions for offline mode
			sqliteservice.PopulateDrinks(); // don't await because system can continue without fully loading the questions for offline mode
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
