using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	public class Game
	{
		private int _gameid;
		private int _sessioncode;
		private int _hostid;
		private string _name;
		private readonly SupabaseService _supabaseService = new SupabaseService();
		private Host _host;

		public int GameId
		{
			get { return _gameid; }
			set { _gameid = value; }
		}

		public int SessionCode
		{
			get { return _sessioncode; }
			set { _sessioncode = value; }
		}

		public int HostId
		{
			get { return _hostid; }
			set { _hostid = value; }
		}

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		public Game(int gameid, int hostid)
		{
			_gameid = gameid;
			_hostid = hostid;
		}
		public Game(Host host, int GameId) 
		{
			_host = host;
			_gameid = GameId;

		}

		public Game(int gameid, string name)
		{
			_gameid = gameid;
			_name = name;
		}

		public Game(int gameid, string name, int sessioncode)
		{
			_gameid = gameid;
			_name = name;
			_sessioncode = sessioncode;
		}

		public Game(int gameid)
		{
			_gameid = gameid;
		}

		public async Task<List<Question>> GetQuestionsAsync()
		{
			return await _supabaseService.GetQuestionsAsync(this);
		}

	}
}
