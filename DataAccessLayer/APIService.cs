using System.Text.Json;
using TruthOrDrink.Model;

namespace TruthOrDrink.DataAccessLayer
{
	public class APIService
	{
		private static readonly HttpClient client = new HttpClient();

		public APIService() { }

		public async static Task<List<Drink>> GetDrinksData()
		{
			string apiUrl = "https://truthordrinkapi.onrender.com/api/myapi/drinks";

			try
			{
				HttpResponseMessage response = await client.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();

					var drinks = JsonSerializer.Deserialize<List<Drink>>(responseBody);

					if (drinks != null && drinks.Count > 0)
					{
						Console.WriteLine($"Successfully fetched {drinks.Count} drinks.");
						return drinks;
					}
					else
					{
						Console.WriteLine("No drinks found or failed to deserialize.");
					}
				}
				else
				{
					Console.WriteLine($"API request failed with status code: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error occurred: {ex.Message}");
			}

			return null;
		}

	}
}
