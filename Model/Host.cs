namespace TruthOrDrink.Model
{
	public class Host
	{
		private int _hostid;
		private string _name;
		private string _email;
		private string _password;
		private int _gameid; // 0 t/m 5

		public int HostId
		{
			get { return _hostid; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string Email
		{
			get { return _email; }
		}

		public string Password
		{
			get { return _password; }
		}

		public int GameId
		{
			get { return _gameid; }
		}

		public Host(string name, string email, string password)
		{
			_name = name;
			_email = email;
			_password = password;
		}

		public Host(int hostid, string email, string password)
		{
			_hostid = hostid;
			_name = "";
			_email = email;
			_password = password;
		}

		public Host(string email, string password)
		{
			_hostid = 0;
			_name = "";
			_email = email;
			_password = password;
		}
		public Host(int hostid) 
		{
			_hostid = hostid;
		}
	}
}
