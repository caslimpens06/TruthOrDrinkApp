using SQLite;
using SQLitePCL;
using TruthOrDrink.Model;

namespace TruthOrDrink.DataAccessLayer
{
	public class SQLiteService
	{
		private SQLiteAsyncConnection _database;
		private List<Question> _questions;

		// DataAccesLayer for the local database
		public SQLiteService()
		{
			_database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
		}
		
		public async Task InitializeAsync()
		{
			try
			{
				await _database.CreateTablesAsync<Host, Question, Drink>();

				Console.WriteLine("Create Table Success");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating tables: {ex.Message}");
			}
		}

		public async Task SaveHostLocallyAsync(Host host)
		{
			try
			{
				await _database.InsertAsync(host);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error inserting host: {ex.Message}");
			}
		}

		public async Task<Host> GetHostAsync()
		{
			try
			{
				return await _database.Table<Host>().FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching host: {ex.Message}");
				return null;
			}
		}

		public async Task<bool> ClearHostTableAsync()
		{
			try
			{
				var allHosts = await _database.Table<Host>().ToListAsync();
				foreach (var host in allHosts)
				{
					await _database.DeleteAsync(host);
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error clearing host table: {ex.Message}");
				return false;
			}
		}


		public async Task UpdateHostAsync(Host newhost)
		{
			try
			{
				if (newhost != null)
				{
					await _database.UpdateAsync(newhost);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating host: {ex.Message}");
			}
		}

		public async Task PopulateQuestionsForOfflineGame() 
		{
			try
			{
				SupabaseService supabaseService = new SupabaseService();
				_questions = await supabaseService.GetAllQuestions();
				if (_questions != null)
				{
					foreach (Question question in _questions)
					{
						await _database.InsertAsync(question);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading/updating questions: {ex.Message}");
			}
		}

		public async Task PopulateDrinks()
		{
			List<Drink> drinkdata = await GetDrinksFromLocalDatabase();

			// Keep retrying until we get valid drink data
			while (drinkdata == null || drinkdata.Count == 0)
			{
				try
				{
					drinkdata = await APIService.GetDrinksData();

					if (drinkdata == null || drinkdata.Count == 0)
					{
						Console.WriteLine("No drink data received. Retrying...");
						await Task.Delay(3000);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error while fetching drink data: {ex.Message}");
					await Task.Delay(3000);
				}
			}

			try
			{
				// Once we have valid data, process it
				foreach (var drink in drinkdata)
				{
					if (string.IsNullOrEmpty(drink.Name) || string.IsNullOrEmpty(drink.Type))
					{
						Console.WriteLine($"Skipping drink with invalid data: Name = {drink.Name}, Type = {drink.Type}");
						continue;
					}

					drink.Name = drink.Name.Trim();
					drink.Type = drink.Type.Trim();

					await _database.InsertAsync(drink);
					Console.WriteLine($"Inserted drink: {drink.Name} | {drink.Type}");
				}

				Console.WriteLine("Inserted all valid drink data.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error loading/updating drinks: {ex.Message}");
			}
		}

		public async Task<List<Drink>> GetDrinksFromLocalDatabase()
		{
			try
			{
				var drinks = await _database.Table<Drink>().ToListAsync();

				if (drinks == null || drinks.Count == 0 || drinks.Any(drink => string.IsNullOrEmpty(drink.Name) || string.IsNullOrEmpty(drink.Type)))
				{
					return null;
				}
				else
				{
					Console.WriteLine("All drinks are valid.");
					return drinks;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while validating drinks: {ex.Message}");
				return null;
			}
		}


	}
}
