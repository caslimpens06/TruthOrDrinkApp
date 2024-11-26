using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink.Model
{
	public class Game
	{
		private int _gameid;
		private int _hostid;

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

		public Game(int gameid, int hostid)
		{
			_gameid = gameid;
			_hostid = hostid;
		}
	}
}
