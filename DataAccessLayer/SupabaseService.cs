using Npgsql;
using TruthOrDrink.Model;

namespace TruthOrDrink.DataAccessLayer;
public class SupabaseService
{
	private readonly string connectionString = "Host=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Username=postgres.etzhitxivaaleejoqevs;Password=TruthOrDrinkTables@#$%;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;";

	public SupabaseService() {}

	public async Task<string> ValidateCredentialsAsync(Host host)
	{
		string query = "SELECT \"Password\" FROM \"Host\" WHERE \"Email\" = @Email;";
		await using var connection = new NpgsqlConnection(connectionString);
		await connection.OpenAsync();
		await using var command = new NpgsqlCommand(query, connection);
		command.Parameters.AddWithValue("@Email", host.Email);
		var result = await command.ExecuteScalarAsync();
		return result?.ToString();
	}

	public async Task<bool> AddParticipantIfNotExists(Participant participant)
	{
		
		string checkQuery = "SELECT 1 FROM \"Participant\" WHERE \"Name\" = @name LIMIT 1;";

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
					return false;
				}
			}

			await using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
			{
				insertCommand.Parameters.AddWithValue("@name", participant.Name);
				insertCommand.Parameters.AddWithValue("@gender", participant.Gender);

				await insertCommand.ExecuteNonQueryAsync();
			}
		}

		return true;
	}


	public async Task AddHostAsync(Host host)
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

	public async Task<bool> CheckIfHostExistsAsync(Host host)
	{
		string query = "SELECT 1 FROM \"Host\" WHERE \"Email\" = @Email LIMIT 1;";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Email", host.Email);

				var result = await command.ExecuteScalarAsync();

				return result != null;
			}
		}
	}

	public async Task JoinParticipantToSession(Participant participant)
	{
		Console.WriteLine("test");
		string query = "INSERT INTO \"JoinedParticipant\" (\"ParticipantId\", \"SessionId\") VALUES (@ParticipantId, @SessionId);";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ParticipantId", participant.ParticipantId);
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<bool> CheckIfSessionExistsAsync(Session session)
	{
		string query = "SELECT 1 FROM \"Session\" WHERE \"SessionId\" = @SessionCode LIMIT 1;";

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionCode", session.SessionCode);

				var result = await command.ExecuteScalarAsync();

				return result != null;
			}
		}
	}

	public async Task<int> GetParticipantPrimarykey(Participant participant)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			// Query to get the ParticipantId based on the Name
			string sqlQuery = "SELECT \"ParticipantId\" FROM \"Participant\" WHERE \"Name\" = @name LIMIT 1;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@name", participant.Name);

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


	public async Task RemoveParticipantAsync(Participant participant)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "DELETE FROM \"Participant\" WHERE \"ParticipantId\" = @participantid;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@participantid", participant.ParticipantId);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<int> GetHostPrimaryKey(Host host)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "SELECT \"HostId\" FROM \"Host\" WHERE \"Email\" = @Email LIMIT 1;";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@Email", host.Email);

				await using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						int primaryKey = Convert.ToInt32(reader["HostId"]);
						return primaryKey;
					}
				}
			}

			return -1;
		}
	}
	
	public async Task<string> GetHostName(Host host)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "SELECT \"Name\" FROM \"Host\" WHERE \"Email\" = @Email LIMIT 1;";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@Email", host.Email);

				await using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						string name = reader["Name"].ToString();
						return name;
					}
				}
			}
			return null;
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

	public async Task UpdateHostCredentials(Host newhost)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"Host\" SET \"Name\" = @Name, \"Password\" = @Password WHERE \"HostId\" = @HostId";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Name", newhost.Name);
				command.Parameters.AddWithValue("@Password", newhost.Password);
				command.Parameters.AddWithValue("@HostId", newhost.HostId);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<bool> CheckIfSessionHasStarted(Session session)
	{
		string query = "SELECT \"SessionHasStarted\" FROM \"Session\" WHERE \"SessionId\" = @sessionId;";

		await using var connection = new NpgsqlConnection(connectionString);

		await connection.OpenAsync();

		await using var command = new NpgsqlCommand(query, connection);

		command.Parameters.AddWithValue("@sessionId", session.SessionCode);

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


	public async Task<Game> GetGameBySessionId(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT \"Game\".\"GameId\", \"Game\".\"Name\" FROM \"Session\" JOIN \"Game\" ON \"Session\".\"GameId\" = \"Game\".\"GameId\" WHERE \"Session\".\"SessionId\" = @SessionCode;";

			
				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@SessionCode", participant.SessionCode);

					using (var reader = await command.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
						return new Game(reader.GetInt32(reader.GetOrdinal("GameId")), reader.GetString(reader.GetOrdinal("Name")), participant.SessionCode);
						}
					}
				}
		}

		// Return null if no game is found for the given SessionId
		return new Game(0, "");
	}



	public async Task<List<Question>> GetQuestionsAsync(Game game)
	{
		var questions = new List<Question>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery;

			if (game.GameId == 5)
			{
				sqlQuery = "SELECT \"QuestionId\", \"QuestionText\", \"GameId\" FROM \"Question\" WHERE \"GameId\" = @GameId AND \"SessionId\" = @SessionCode;";
			}
			else
			{
				sqlQuery = "SELECT \"QuestionId\", \"QuestionText\", \"GameId\" FROM \"Question\" WHERE \"GameId\" = @GameId;";
			}

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@GameId", game.GameId);

				if (game.GameId == 5)
				{
					command.Parameters.AddWithValue("@SessionCode", game.SessionCode);
				}

				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						Question question = new Question(Convert.ToInt32(reader["QuestionId"]), reader["QuestionText"].ToString(), Convert.ToInt32(reader["GameId"]));

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

			string sqlQuery = "INSERT INTO \"Answer\" (\"QuestionId\", \"Response\", \"JoinedParticipantId\") VALUES (@QuestionId, @Response, @ParticipantId);";

			await using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
				command.Parameters.AddWithValue("@Response", answer.Response);
				command.Parameters.AddWithValue("@ParticipantId", answer.ParticipantId);

				await command.ExecuteNonQueryAsync();
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

	public async Task<bool> CheckIfCustomGame(Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT EXISTS (SELECT 1 FROM public.\"Session\" WHERE \"SessionId\" = @SessionId AND \"GameId\" = 5)";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", session.SessionCode);
				return (bool)await command.ExecuteScalarAsync();
			}
		}
	}


	public async Task StartGame(Session session)
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

	public async Task AddQuestionByParticipant(Question question, int sessioncode)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "INSERT INTO public.\"Question\" (\"QuestionText\", \"GameId\", \"SessionId\") VALUES (@QuestionText, 5, @SessionCode)";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@QuestionText", question.Text);
				command.Parameters.AddWithValue("@SessionCode", sessioncode);
				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task SetDoneAddingQuestions(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"JoinedParticipant\" SET \"DoneAddingQuestions\" = TRUE WHERE \"ParticipantId\" = @ParticipantId";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@ParticipantId", participant.ParticipantId);
				await command.ExecuteNonQueryAsync();
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

	public async Task SetCurrentQuestion(Question question, Session session)
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

	public async Task SetAllQuestionsToAnswered(Participant participant)
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

			string query = "SELECT p.\"Name\", p.\"Gender\", COUNT(a.\"Response\") AS drink_count FROM public.\"Answer\" a JOIN public.\"JoinedParticipant\" jp ON a.\"JoinedParticipantId\" = jp.\"ParticipantId\" JOIN public.\"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE a.\"Response\" = 'Drink' AND jp.\"SessionId\" = @SessionId GROUP BY p.\"Name\", p.\"Gender\" ORDER BY drink_count DESC LIMIT 1;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return new Participant(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), "this is a placeholder to distinguish the truth/drink count");
					}
				}
			}
		}

		return null;
	}

	public async Task<Participant> GetTopTruth(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT p.\"Name\", p.\"Gender\", COUNT(a.\"Response\") AS truth_count FROM public.\"Answer\" a JOIN public.\"JoinedParticipant\" jp ON a.\"JoinedParticipantId\" = jp.\"ParticipantId\" JOIN public.\"Participant\" p ON jp.\"ParticipantId\" = p.\"ParticipantId\" WHERE a.\"Response\" = 'Truth' AND jp.\"SessionId\" = @SessionId GROUP BY p.\"Name\", p.\"Gender\" ORDER BY truth_count DESC LIMIT 1;";

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

		return null;
	}

	public async Task<bool> CheckIfAnswerHasBeenGiven(Question question, Session session)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT COUNT(*) = (SELECT COUNT(*) FROM public.\"JoinedParticipant\" WHERE \"SessionId\" = @SessionId) AS all_answered FROM public.\"Answer\" a WHERE a.\"QuestionId\" = @QuestionId AND a.\"JoinedParticipantId\" IN (SELECT \"ParticipantId\" FROM public.\"JoinedParticipant\" WHERE \"SessionId\" = @SessionId);";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@QuestionId", question.QuestionId);
				command.Parameters.AddWithValue("@SessionId", session.SessionCode);

				var result = await command.ExecuteScalarAsync();

				if (result == null || result == DBNull.Value)
					return false;

				return (bool)result;
			}
		}
	}

	public async Task SetSessionToClose(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"Session\" SET \"CloseGame\" = TRUE WHERE \"SessionId\" = @SessionId;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<Host> LoadHostData(Host host)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT \"Name\", \"Email\", \"Password\" FROM public.\"Host\" WHERE \"HostId\" = @HostId;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@HostId", host.HostId);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return new Host(reader[0].ToString(), reader[1].ToString(), reader[2].ToString());
					}
				}
			}
		}

		return null;
	}

	public async Task<bool> CheckIfGameClosed(Participant participant)
	{
		using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT \"CloseGame\" FROM public.\"Session\" WHERE \"SessionId\" = @SessionId;";

			using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return reader.GetBoolean(0);
					}
				}
			}
		}

		return false;
	}

	public async Task DeleteAllSessions(Host host)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string sqlQuery = "DELETE FROM \"Session\" WHERE \"HostId\" = @HostId;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("@HostId", host.HostId);

				await command.ExecuteNonQueryAsync();
			}
		}
	}

	public async Task<List<Question>> GetAllQuestions()
	{
		var questions = new List<Question>();

		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT \"QuestionId\", \"QuestionText\", \"GameId\" FROM public.\"Question\" WHERE \"GameId\" != 5;";

			await using (var command = new NpgsqlCommand(query, connection))
			{
				await using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						Question question = new Question(Convert.ToInt32(reader["QuestionId"]), Convert.ToString(reader["QuestionText"]), Convert.ToInt32(reader["GameId"]));

						questions.Add(question);
						Console.WriteLine(question.QuestionId + question.Text + question.GameId);
					}
				}
			}
		}

		return questions;
	}

	public async Task AddDrinksToSession(Session session)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();

				string query = "UPDATE public.\"Session\" SET \"SelectedDrinks\" = @DrinksinJson::jsonb WHERE \"SessionId\" = @SessionId";

				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@DrinksinJson", session.SelectedDrinksJson);
					command.Parameters.AddWithValue("@SessionId", session.SessionCode);

					await command.ExecuteNonQueryAsync();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error adding drinks to Supabase: {ex.Message}");
		}
	}

	public async Task<string> GetDrinksBySession(Participant participant)
	{
		try
		{
			using (var connection = new NpgsqlConnection(connectionString))
			{
				await connection.OpenAsync();

				string query = "SELECT \"SelectedDrinks\" FROM public.\"Session\" WHERE \"SessionId\" = @SessionId";

				using (var command = new NpgsqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@SessionId", participant.SessionCode);

					var result = await command.ExecuteScalarAsync();
					return result.ToString();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching drinks for session: {ex.Message}");
			return null;
		}
	}

	public async Task<bool> SaveMaxPlayerCountOnline(Settings settings, Host host)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "UPDATE public.\"Host\" SET \"MaxPlayerCount\" = @MaxPlayerCount WHERE \"HostId\" = @HostId;";

			await using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@MaxPlayerCount", settings.MaxPlayerCount);
				command.Parameters.AddWithValue("@HostId", host.HostId);

				int rowsAffected = await command.ExecuteNonQueryAsync();
				return rowsAffected > 0;
			}
		}
	}

	public async Task<bool> CheckMaxPlayerCount(Session session)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			string query = "SELECT CASE WHEN h.\"MaxPlayerCount\" <= (SELECT COUNT(*) FROM \"JoinedParticipant\" p WHERE p.\"SessionId\" = s.\"SessionId\") THEN false ELSE true END AS \"IsMaxPlayerCountValid\" FROM \"Host\" h JOIN \"Session\" s ON h.\"HostId\" = s.\"HostId\" WHERE s.\"SessionId\" = @SessionId;";

			await using (var command = new NpgsqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@SessionId", session.SessionCode);
				var result = await command.ExecuteScalarAsync();
				return result != DBNull.Value && Convert.ToBoolean(result);
			}
		}
	}


}

