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

namespace TruthOrDrink;
public class SupabaseService
{
	private readonly string _baseUrl = "https://etzhitxivaaleejoqevs.supabase.co"; // Supabase URL
	private readonly string _apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImV0emhpdHhpdmFhbGVlam9xZXZzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzIyMTc3ODIsImV4cCI6MjA0Nzc5Mzc4Mn0.m9JCU69FmqBBz4LeUR_SHh2v-AnmqcjLd1r0wJxMtXM"; // Supabase API Key
	private readonly HttpClient _httpClient;

	public SupabaseService()
	{
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
		_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
	}

	// Maak user aan in tabel
	public async Task AddPlayerAsync(Host host)
	{
		var playerData = new
		{
			Name = host.Name,
			Email = host.Email,
			Password = host.Password
		};

		var content = new StringContent(JsonSerializer.Serialize(playerData), Encoding.UTF8, "application/json");

		await _httpClient.PostAsync($"{_baseUrl}/rest/v1/Host", content);
	}

	// Valideer of credentials correct zijn voor login
	public async Task<bool> ValidateCredentialsAsync(Host host)
	{
		var url = $"{_baseUrl}/rest/v1/Host?Email=eq.{host.Email}&Password=eq.{host.Password}";
		var response = await _httpClient.GetAsync(url);
		
		return response.IsSuccessStatusCode;
	}
	
	// Check of account bestaat via email.
	public async Task<bool> CheckIfUserExistsAsync(string email)
	{
		var url = $"{_baseUrl}/rest/v1/Host?Email=eq.{email}";
		var response = await _httpClient.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();

			if (!string.IsNullOrEmpty(content) && content != "[]")
			{
				return true;
			}
		}
		return false;
	}

	public async void AddParticipantAsync(Participant participant)
	{
		// Create the payload to send in the POST request
		var playerData = new
		{
			Name = participant.Name,
		};

		// Serialize the data to JSON format
		var content = new StringContent(JsonSerializer.Serialize(playerData), Encoding.UTF8, "application/json");

		// Perform the POST request to add the participant
		var response = await _httpClient.PostAsync($"{_baseUrl}/rest/v1/Participant", content);
	}

	public async Task<int> GetPrimaryKeyByName(string name)
	{
		// Connection string to Supabase database
		string connectionString = "Host=aws-0-eu-central-1.pooler.supabase.com;Port=6543;Username=postgres.etzhitxivaaleejoqevs;Password=Joepie2021@#$%;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;";

		// Open connection asynchronously
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();  // Use OpenAsync to make the connection asynchronous
			Console.WriteLine("Connection successful!");

			// Query to get the ParticipantId based on the Name
			string sqlQuery = "SELECT \"ParticipantId\" FROM \"Participant\" WHERE \"Name\" = @name LIMIT 1;";

			using (var command = new NpgsqlCommand(sqlQuery, connection))
			{
				// Add the parameter to the query
				command.Parameters.AddWithValue("@name", name);

				// Execute the query asynchronously
				using (var reader = await command.ExecuteReaderAsync())  // ExecuteReaderAsync is non-blocking
				{
					if (await reader.ReadAsync())  // Use ReadAsync to prevent blocking
					{
						// Return the ParticipantId as an int
						return Convert.ToInt32(reader["ParticipantId"]);
					}
				}
			}

			// If no data is found, return a default value (e.g., -1 or any error code)
			return -1;
		}
	}






	public async Task JoinParticipantToGame(int participantid, int gamecode)
	{
		var playerData = new
		{
			ParticipantId = participantid,
			GameId = gamecode,
		};

		var content = new StringContent(JsonSerializer.Serialize(playerData), Encoding.UTF8, "application/json");

		await _httpClient.PostAsync($"{_baseUrl}/rest/v1/JoinedParticipants", content);
	}

	public async Task<bool> CheckIfGameExistsAsync(int gamecode)
	{
		var url = $"{_baseUrl}/rest/v1/Game?GameId=eq.{gamecode}&select=GameId";
		var response = await _httpClient.GetAsync(url);

		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();

			if (!string.IsNullOrEmpty(content) && content != "[]")
			{
				return true;
			}
		}
		return false;
	}
}

