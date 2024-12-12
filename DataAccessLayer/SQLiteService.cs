using SQLite;
using TruthOrDrink.Model;

namespace TruthOrDrink.DataAccessLayer
{
	public class SQLiteService
	{
		private SQLiteAsyncConnection _database;

		// Local database for caching
		public SQLiteService()
		{
			_database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
		}
		
		public async Task InitializeAsync()
		{
			try
			{
				await _database.CreateTableAsync<Host>();
				Console.WriteLine("Create Table Success");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating tables: {ex.Message}");
			}
		}

		public async Task PopulateTableAsync(Host host)
		{
			try
			{
				await _database.InsertAsync(host);
				Console.WriteLine($"Host {host.Name} added successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error inserting hosts: {ex.Message}");
			}
		}

		public async Task SaveHostAsync(Host host)
		{
			try
			{
				await _database.InsertAsync(host);
				Console.WriteLine("Host added successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error inserting host: {ex.Message}");
			}
		}

		public async Task<Host> GetHostAsync(Host host)
		{
			try
			{
				return await _database.FindAsync<Host>(host.HostId);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching host: {ex.Message}");
				return null;
			}
		}
		public async Task UpdateHostAsync(Host newhost)
		{
			try
			{
				if (newhost != null)
				{
					await _database.UpdateAsync(newhost);
					Console.WriteLine("Host updated successfully.");
				}
				else
				{
					Console.WriteLine($"Host with HostId {newhost.HostId} not found.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating host: {ex.Message}");
			}
		}


	}
}
