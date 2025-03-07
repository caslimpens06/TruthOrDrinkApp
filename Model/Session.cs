﻿using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	public class Session
	{
		private Host _host;
		private int _hostid;
		private int _gameid;
		private int _sessioncode;
		private string _selectedDrinksJson;
		private readonly SupabaseService _supabaseService = new SupabaseService();
		private readonly SQLiteService _sqliteService = new SQLiteService();

		public int SessionCode
		{
			get { return _sessioncode; }
		}
		public int GameId
		{
			get { return _gameid; }
			set { _gameid = value; }
		}

		public int HostId
		{
			get { return _hostid; }
			set { _hostid = value; }
		}
		
		public string SelectedDrinksJson
		{
			get { return _selectedDrinksJson; }
			set { _selectedDrinksJson = value; }
		}

		public Session(Host host, int gameid)
		{
			_host = host;
			_gameid = gameid;
		}

		public Session(int sessioncode) // gameid = sessioncode in offline-mode (because of constructor ambiguity)
		{
			_sessioncode = sessioncode;
		}

		public Session(int sessioncode, int gameid)
		{
			_sessioncode = sessioncode;
			_gameid = gameid;
		}

		public Session(int sessioncode, int hostid, int gameid)
		{
			_sessioncode = sessioncode;
			_hostid = hostid;
			_gameid = gameid;
		}

		public Session(int sessioncode, string selecteddrinksjson)
		{
			_sessioncode = sessioncode;
			_selectedDrinksJson = selecteddrinksjson;
		}

		public async void AddSessionToDatabase() 
		{
			await _supabaseService.AddSessionToDatabase(this);
		}

		public async Task<List<Participant>> GetParticipantsBySession() 
		{ 
			return await _supabaseService.GetParticipantsBySession(this);
		}

		public async Task<bool> CheckIfCustomGame()
		{
			return await _supabaseService.CheckIfCustomGame(this);
		}
		public async Task StartGame()
		{
			await _supabaseService.StartGame(this);
		}

		public async Task<bool> CheckIfSessionExistsAsync()
		{
			return await _supabaseService.CheckIfSessionExistsAsync(this);
		}

		public async Task<bool> CheckIfSessionHasStarted()
		{
			return await _supabaseService.CheckIfSessionHasStarted(this);
		}
		public async Task<List<Question>> GetLocalQuestions()
		{
			return await _sqliteService.GetQuestionsFromLocalDatabase(this);
		}
		public async Task AddDrinksToSession()
		{
			await _supabaseService.AddDrinksToSession(this);
		}
		public async Task<bool> CheckMaxPlayerCount()
		{
			return await _supabaseService.CheckMaxPlayerCount(this);
		}

	}
}
