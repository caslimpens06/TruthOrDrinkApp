using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel.Communication;
using TruthOrDrink.Model;
using Npgsql;
using System.Net.Http.Json;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using System.Xml.Linq;

namespace TruthOrDrink;
public class SupabaseService
{
	string connectionString = "Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Username=postgres.etzhitxivaaleejoqevs;Password=Joepie2021@#$%;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;";

	public SupabaseService() {}

	public async Task<bool> ValidateCredentialsAsync(Host host, Button loginButton)
	{
			// Disable the button to prevent double-clicking
			loginButton.IsEnabled = false;

		    string query = "SELECT \"HostId\" FROM \"Host\" WHERE \"Email\" = @Email AND \"Password\" = @Password;";

			await using var connection = new NpgsqlConnection(connectionString);
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("@Email", host.Email);
			command.Parameters.AddWithValue("@Password", host.Password);

			var result = await command.ExecuteScalarAsync();

			if (result != null)
			{
				loginButton.IsEnabled = true;
				return true;
				
			}
			else
			{
				loginButton.IsEnabled = true;
				return false;
			}
	}

	public async Task<bool> AddParticipantIfNotExists(Participant participant)
	{

		string checkQuery = "SELECT 1 FROM \"Participant\" WHERE \"Name\" = @name AND \"Gender\" = @gender LIMIT 1;";

		string insertQuery = "INSERT INTO \"Participant\" (\"Name\", \"Gender\") VALUES (@name, @gender);";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();


			await using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
			{
				checkCommand.Parameters.AddWithValue("@name", participant.Name);
				checkCommand.Parameters.AddWithValue("@gender", participant.Gender);

				var exists = await checkCommand.ExecuteScalarAsync();
				if (exists != null)
				{
					// Participant already exists
					return false;
				}
			}

			// Add the participant if it does not exist
			await using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
			{
				insertCommand.Parameters.AddWithValue("@name", participant.Name);
				insertCommand.Parameters.AddWithValue("@gender", participant.Gender);

				await insertCommand.ExecuteNonQueryAsync();
			}
		}

		return true; // Participant was newly created
	}


	public async Task AddPlayerAsync(Host host)
	{
		string sqlQuery = "INSERT INTO \"Host\" (\"Name\", \"Email\", \"Password\") VALUES (@name, @email, @password);";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@name", host.Name);
				command.Parameters.AddWithValue("@email", host.Email);
				command.Parameters.AddWithValue("@password", host.Password);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<bool> CheckIfUserExistsAsync(string email)
	{
		string sqlQuery = "SELECT 1 FROM \"Host\" WHERE \"Email\" = @email LIMIT 1;";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@email", email);

				var result = await command.ExecuteScalarAsync();

				return result != null;
			}
		}
	}

	public async Task<bool> JoinParticipantToGame(Participant participant)
	{
		bool gameExists = await CheckIfGameExistsAsync(participant.Gamecode);

		if (!gameExists)
		{
			return false;
		}

		string sqlQuery = "INSERT INTO \"JoinedParticipants\" (\"ParticipantId\", \"GameId\") VALUES (@participantId, @gameId);";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@participantId", participant.ParticipantId);
				command.Parameters.AddWithValue("@gameId", participant.Gamecode);

				await command.ExecuteNonQueryAsync();
			}
		}
		return true;
	}

	public async Task<bool> CheckIfGameExistsAsync(int gamecode)
	{
		string sqlQuery = "SELECT 1 FROM \"Game\" WHERE \"GameId\" = @gamecode LIMIT 1;";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@gamecode", gamecode);

				var result = await command.ExecuteScalarAsync();

				return result != null;
			}
		}
	}

	public async Task<int> GetParticipantPrimarykey(string name)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			// Query to get the ParticipantId based on the Name
			string sqlQuery = "SELECT \"ParticipantId\" FROM \"Participant\" WHERE \"Name\" = @name LIMIT 1;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@name", name);

				// Execute the query asynchronously
				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						// Return the ParticipantId as an int
						return Convert.ToInt32(reader["ParticipantId"]);
					}
				}
			}
			return -1;
		}
	}

	public async Task<List<int>> GetExistingGameIdsAsync()
	{
		const string query = "SELECT \"Game\".\"GameId\" FROM \"Game\"";
		var existingGameIds = new List<int>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);

			await using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				existingGameIds.Add(reader.GetInt32(0)); // Assuming GameId is an integer
			}
			

			return existingGameIds;

		}
	}

	public async Task AddGameToDatabaseAsync(Game game)
	{
		string query = "INSERT INTO \"Game\" (\"GameId\", \"HostId\") VALUES (@GameId, @HostId)";

		await using var connection = new NpgsqlConnection(connectionString);

			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("@GameId", game.GameId);   // Pass GameId as an integer
			command.Parameters.AddWithValue("@HostId", game.HostId);  // Pass HostId as an integer

			await command.ExecuteNonQueryAsync();
	}

	public async void RemoveParticipant(Participant participant)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "DELETE FROM \"Participant\" WHERE \"ParticipantId\" = @participantid;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				// Add parameter for Name
				command.Parameters.AddWithValue("@participantid", participant.ParticipantId);

				// Execute the query asynchronously
				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<int> GetHostPrimaryKey(Host host)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			// Query to get the HostId based on the Email
			string sqlQuery = "SELECT \"HostId\" FROM \"Host\" WHERE \"Email\" = @Email LIMIT 1;";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@Email", host.Email);

				// Execute the query asynchronously
				await using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						int primaryKey = Convert.ToInt32(reader["HostId"]);
						return primaryKey;
					}
				}
			}

			// If no data found, return -1
			return -1;
		}
	}

	public async Task<List<Participant>> GetParticipantsByGame(Game game)
	{
		string query = "SELECT p.\"ParticipantId\", p.\"Name\", p.\"Gender\", jp.\"GameId\" FROM \"JoinedParticipants\" jp JOIN \"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE jp.\"GameId\" = @GameId";


		var participants = new List<Participant>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("@GameId", game.GameId);

			await using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				var participantId = reader.GetInt32(0);
				var name = reader.GetString(1);
				var gender = reader.GetString(2);

				var participant = new Participant(name, gender)
				{
					Name = name,
					Gender = gender
				};
				participants.Add(participant);
			}

			return participants;
		}
	}

	public async Task<bool> CheckIfGameHasStarted(int gameCode)
	{
		try
		{

			string query = "SELECT \"GameHasStarted\" FROM \"Game\" WHERE \"GameId\" = @GameId";

			await using var connection = new NpgsqlConnection(connectionString);

			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);

			command.Parameters.AddWithValue("@GameId", gameCode);

			var result = await command.ExecuteScalarAsync();

			if (result is bool gameHasStarted)
			{
				return gameHasStarted;
			}
			else
			{
				return true;
			}
		}
		catch (Exception)
		{
			return true;
		}
	}
}

