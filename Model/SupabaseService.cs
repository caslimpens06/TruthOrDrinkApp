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
using Npgsql.Replication.PgOutput.Messages;

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

	public async Task<bool> JoinParticipantToSession(Participant participant)
	{
		bool gameExists = await CheckIfSessionExistsAsync(participant.SessionCode);

		if (!gameExists)
		{
			return false;
		}

		string sqlQuery = "INSERT INTO \"JoinedParticipant\" (\"ParticipantId\", \"SessionId\") VALUES (@ParticipantId, @SessionId);";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@ParticipantId", participant.ParticipantId);
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				await command.ExecuteNonQueryAsync();
			}
		}
		return true;
	}

	public async Task<bool> CheckIfSessionExistsAsync(int sessioncode)
	{
		string query = "SELECT 1 FROM \"Session\" WHERE \"SessionId\" = @SessionCode LIMIT 1;";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionCode", sessioncode);

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

	public async Task<List<int>> GetExistingSessionIdsAsync()
	{
		string query = "SELECT \"Session\".\"SessionId\" FROM \"Session\";";

		var existingGameIds = new List<int>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);

			await using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				existingGameIds.Add(reader.GetInt32(0));
			}
			

			return existingGameIds;

		}
	}

	public async Task AddSessionToDatabase(Session session)
	{
		Console.WriteLine($"{session.SessionCode} {session.HostId} {session.GameId}");

		// Modify your query to include GameId as well
		string query = "INSERT INTO \"Session\" (\"SessionId\", \"HostId\", \"GameId\") VALUES (@SessionId, @HostId, @GameId)";

		await using var connection = new NpgsqlConnection(connectionString);
		await connection.OpenAsync();

		await using var command = new NpgsqlCommand(query, connection);

		// Assuming session.GameId and session.HostId are integers
		command.Parameters.AddWithValue("@SessionId", session.SessionCode);  // Use GameId for SessionId if that's the logic
		command.Parameters.AddWithValue("@HostId", session.HostId);     // Pass HostId
		command.Parameters.AddWithValue("@GameId", session.GameId);     // Add GameId as a new parameter

		await command.ExecuteNonQueryAsync();
	}


	public async void RemoveParticipantAsync(Participant participant)
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

	public async Task<List<Participant>> GetParticipantsBySession(Session session)
	{
		string query = "SELECT p.\"ParticipantId\", p.\"Name\", p.\"Gender\", jp.\"SessionId\" FROM \"JoinedParticipant\" jp JOIN \"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE jp.\"SessionId\" = @SessionId";


		var participants = new List<Participant>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("@SessionId", session.SessionCode);

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

	public async Task<bool> CheckIfSessionHasStarted(int gameCode)
	{
		try
		{
			string query = "SELECT \"SessionHasStarted\" FROM \"Session\" WHERE \"SessionId\" = @sessionId;";

			await using var connection = new NpgsqlConnection(connectionString);

			await connection.OpenAsync();

			await using var command = new NpgsqlCommand(query, connection);

			command.Parameters.AddWithValue("@sessionId", gameCode);

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


	/////
	/////
	/////
	//Logica voor gamequestionanswer



	public async Task<Game> GetGameBySessionId(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			// Define the SQL query with JOIN to get GameId and GameName
			string query = "SELECT \"Game\".\"GameId\", \"Game\".\"Name\" FROM \"Session\" JOIN \"Game\" ON \"Session\".\"GameId\" = \"Game\".\"GameId\" WHERE \"Session\".\"SessionId\" = @SessionCode;";

			try
			{
				// Create the command
				using (var command = new NpgsqlCommand(query, connection))
				{
					// Add the parameter for SessionId
					command.Parameters.AddWithValue("@SessionCode", participant.SessionCode);

					// Execute the query and read the result
					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							// Read values and return Game object
							return new Game(
								reader.GetInt32(reader.GetOrdinal("GameId")),
								reader.GetString(reader.GetOrdinal("Name"))
							);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log the exception and rethrow or handle as needed
				Console.WriteLine($"Error retrieving game: {ex.Message}");
				throw;
			}
		}

		// Return null if no game is found for the given SessionId
		return new Game(0, "");
	}







	public async Task<List<Question>> GetQuestionsByGameIdAsync(Game game)
	{
		var questions = new List<Question>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "SELECT \"QuestionId\", \"QuestionText\", \"GameId\" FROM \"Question\" WHERE \"GameId\" = @GameId;";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@GameId", game.GameId);

				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						Question question = new Question(Convert.ToInt32(reader["QuestionId"]),reader["QuestionText"].ToString(), Convert.ToInt32(reader["GameId"]));

						questions.Add(question);
					}
				}
			}
		}

		return questions;
	}

	public async Task SaveAnswerAsync(Answer answer)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			// Single long query to insert a new answer into the Answer table
			string sqlQuery = "INSERT INTO \"Answer\" (\"QuestionId\", \"Response\", \"ParticipantId\") VALUES (@QuestionId, @Response, @ParticipantId);";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
				command.Parameters.AddWithValue("@Response", answer.Response);
				command.Parameters.AddWithValue("@ParticipantId", answer.ParticipantId);

				await command.ExecuteNonQueryAsync();
			}
		}

		// Optional: Log or display the saved answer
		Console.WriteLine($"Answer saved: {answer.Response} for Question {answer.QuestionId} (Participant {answer.ParticipantId})");
	}

	public async Task<bool> CheckIfSessionHasStartedAsync(Participant participant) 
	{
		// The SQL query to check the "SessionHasStarted" status for a specific session
		string query = "SELECT \"SessionHasStarted\" FROM \"Session\" WHERE \"SessionId\" = @sessionId";

		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(query, connection))
			{
				// Add the GameId parameter to the SQL query
				command.Parameters.AddWithValue("@sessionId", participant.SessionCode);

				// Execute the query and get the result
				var result = await command.ExecuteScalarAsync();

				// If result is null or DBNull, return false (session has not started)
				if (result == null || DBNull.Value.Equals(result))
				{
					return false;
				}

				// Return the value as a boolean
				return Convert.ToBoolean(result);
			}
		}
	}

	public async Task<Session> GetGameId(Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT \"GameId\" FROM \"Session\" WHERE \"Session\".\"SessionId\" = @SessionCode";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionCode", session.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						session.GameId = reader.GetInt32(reader.GetOrdinal("GameId"));
					}
				}
			}
		}

		return session;
	}

	public async void StartGame(Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE \"Session\" SET \"SessionHasStarted\" = TRUE WHERE \"SessionId\" = @SessionCode";

			using (var command = new NpgsqlCommand(query, connection))
			{
				Console.WriteLine(session.SessionCode);
				command.Parameters.AddWithValue("@SessionCode", session.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						session.GameId = reader.GetInt32(reader.GetOrdinal("GameId"));
					}
				}
			}
		}
	}



	public async Task<Question> GetCurrentQuestionAsync(Participant participant)
	{
		Question question = null;

		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT s.\"CurrentQuestionId\", q.\"QuestionText\" FROM public.\"Session\" s JOIN public.\"Question\" q ON s.\"CurrentQuestionId\" = q.\"QuestionId\" WHERE s.\"SessionId\" = @SessionCode";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionCode", participant.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						question = new Question(reader.GetInt32(reader.GetOrdinal("CurrentQuestionId")), reader.GetString(reader.GetOrdinal("QuestionText")));
					}
				}
			}
		}

		return question;
	}

	public async void SetCurrentQuestion(Question question, Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"Session\" SET \"CurrentQuestionId\" = @QuestionId WHERE \"SessionId\" = @SessionId";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
				command.Parameters.AddWithValue("@SessionId", session.SessionCode);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async void SetAllQuestionsToAnswered(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"JoinedParticipant\" SET \"AnsweredAllQuestions\" = TRUE WHERE \"ParticipantId\" = @ParticipantId";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ParticipantId", participant.ParticipantId);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<bool> CheckIfAllQuestionsAnswered(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT CASE WHEN COUNT(*) = COUNT(CASE WHEN \"AnsweredAllQuestions\" = TRUE THEN 1 END) THEN TRUE ELSE FALSE END AS all_questions_answered FROM public.\"JoinedParticipant\" WHERE \"SessionId\" = @SessionId";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				return (bool)await command.ExecuteScalarAsync();
			}
		}
	}



	public async Task<Participant> GetTopDrinker(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT p.\"Name\", p.\"Gender\", COUNT(a.\"Response\") AS drink_count FROM public.\"Answer\" a JOIN public.\"JoinedParticipant\" jp ON a.\"ParticipantId\" = jp.\"ParticipantId\" JOIN public.\"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE a.\"Response\" = 'Drink' AND jp.\"SessionId\" = @SessionId GROUP BY p.\"Name\", p.\"Gender\" ORDER BY drink_count DESC LIMIT 1;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return new Participant(reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
						
					}
				}
			}
		}

		return null; // Return null if no participant found
	}

	public async Task<Participant> GetTopTruth(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT p.\"Name\", p.\"Gender\", COUNT(a.\"Response\") AS truth_count FROM public.\"Answer\" a JOIN public.\"JoinedParticipant\" jp ON a.\"ParticipantId\" = jp.\"ParticipantId\" JOIN public.\"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE a.\"Response\" = 'Truth' AND jp.\"SessionId\" = @SessionId GROUP BY p.\"Name\", p.\"Gender\" ORDER BY truth_count DESC LIMIT 1;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return new Participant(reader.GetString(0), reader.GetString(1), reader.GetInt32(2));
					}
				}
			}
		}

		return null; // Return null if no participant found
	}

	public async Task<bool> CheckIfAnswerHasBeenGiven(Question question, Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT CASE WHEN COUNT(DISTINCT \"ParticipantId\") = (SELECT COUNT(DISTINCT \"ParticipantId\") FROM public.\"JoinedParticipant\" WHERE \"SessionId\" = @SessionId) THEN TRUE ELSE FALSE END AS all_participants_answered FROM public.\"Answer\" WHERE \"QuestionId\" = @QuestionId AND \"ParticipantId\" IN (SELECT \"ParticipantId\" FROM public.\"JoinedParticipant\" WHERE \"SessionId\" = @SessionId)";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
				command.Parameters.AddWithValue("@SessionId", session.SessionCode);
				return (bool)await command.ExecuteScalarAsync();
			}
		}
	}



}

