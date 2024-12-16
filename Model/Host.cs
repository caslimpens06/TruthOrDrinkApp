using SQLite;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	[SQLite.Table("Host")]
	public class Host
	{
		private int _hostid;
		private string _name;
		private string _email;
		private string _password;
		private readonly SupabaseService _supabaseService = new SupabaseService();

		[PrimaryKey]
		public int HostId
		{
			get { return _hostid; }
			set { _hostid = value; }
		}
		public string Name
		{
			get { return _name; }
			set { _name  = value; }
		}
		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		public Host(string name, string email, string password)
		{
			_name = name;
			_email = email;
			_password = password;
		}
		public Host() { } // Local database needs parameterless constructor

		public Host(int hostid, string email, string password)
		{
			_hostid = hostid;
			_email = email;
			_password = password;
		}

		public Host(int hostid, string name, string email, string password)
		{
			_hostid = hostid;
			_name = name;
			_email = email;
			_password = password;
		}

		public Host(string email, string password)
		{
			_email = email;
			_password = password;
		}
		public Host(int hostid) 
		{
			_hostid = hostid;
		}
		public Host(string email)
		{
			_email = email;
		}

		public async Task<bool> CheckIfHostExistsAsync()
		{
			return await _supabaseService.CheckIfHostExistsAsync(this);
		}

		public async Task AddHostAsync()
		{
			await _supabaseService.AddHostAsync(this);
		}

		public async Task<string> ValidateCredentialsAsync()
		{
			return await _supabaseService.ValidateCredentialsAsync(this);

		}

		public async Task<int> GetHostPrimaryKey()
		{
			return await _supabaseService.GetHostPrimaryKey(this);
		}

		public async Task<string> GetHostName()
		{
			return await _supabaseService.GetHostName(this);
		}
		public async Task UpdateHostCredentials()
		{
			await _supabaseService.UpdateHostCredentials(this);
		}
		public async Task DeleteAllSessions()
		{
			await _supabaseService.DeleteAllSessions(this);
		}
		public async Task<Host> LoadHostData()
		{
			return await _supabaseService.LoadHostData(this);
		}

	}
}
