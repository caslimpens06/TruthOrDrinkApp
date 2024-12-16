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
				await _database.CreateTablesAsync<Host, Question>();

				Console.WriteLine("Create Table Success");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating tables: {ex.Message}");
			}
		}

		public async Task SaveHostAsync(Host host)
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
	}
}
