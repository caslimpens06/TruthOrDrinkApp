
namespace TruthOrDrink.Model
{
	public class Host
	{
		private int _hostid;
		private string _name;
		private string _email;
		private string _password;
		private readonly SupabaseService _supabaseService = new SupabaseService();

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

		public Host(string name, string email, string password)
		{
			_name = name;
			_email = email;
			_password = password;
		}

		public Host(int hostid, string email, string password)
		{
			_hostid = hostid;
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
		

		public async Task<bool> CheckIfHostExistsAsync()
		{
			return await _supabaseService.CheckIfHostExistsAsync(this);
		}

		public async Task AddHostAsync()
		{
			await _supabaseService.AddHostAsync(this);
		}

		public async Task<bool> ValidateCredentialsAsync()
		{
			return await _supabaseService.ValidateCredentialsAsync(this);

		}

		public async Task<int> GetHostPrimaryKey()
		{
			return await _supabaseService.GetHostPrimaryKey(this);
		}

		public async Task<Host> LoadHostData()
		{
			return await _supabaseService.LoadHostData(this);
		}
	}
}
