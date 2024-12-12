using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	public class Session
	{
		private Host _host;
		private int _hostid;
		private int _gameid;
		private int _sessioncode;
		private readonly SupabaseService _supabaseService = new SupabaseService();

		public Host Host
		{
			get { return _host; }
		}

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

		public Session(Host host, int gameid)
		{
			_host = host;
			_gameid = gameid;
		}

		public Session(int sessioncode)
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
	}
}
