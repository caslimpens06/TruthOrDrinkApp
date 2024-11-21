using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
	public async Task AddPlayerAsync(User user)
	{
		var playerData = new
		{
			Name = user.Name,
			Email = user.Email,
			Password = user.Password
		};

		var content = new StringContent(JsonSerializer.Serialize(playerData), Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync($"{_baseUrl}/rest/v1/User", content);

		if (!response.IsSuccessStatusCode)
		{
			string errorResponse = await response.Content.ReadAsStringAsync();
			throw new Exception($"Failed to add player: {errorResponse}");
		}
	}

	// Valideer of gebruiker bestaat
	public async Task<bool> ValidateCredentialsAsync(User user)
	{
		if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
			throw new ArgumentException("Email and Password must be provided.");

		var url = $"{_baseUrl}/rest/v1/User?Email=eq.{user.Email}&Password=eq.{user.Password}";

		var response = await _httpClient.GetAsync(url);

		// Return true if the request is successful (status 200)
		return response.IsSuccessStatusCode;
	}
}

