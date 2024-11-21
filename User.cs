namespace TruthOrDrink
{
	public class User
	{
		private string _name;
		private string _email;
		private string _password;

		// Public properties
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

		public User(string name, string email, string password)
		{
			_name = name;
			_email = email;
			_password = password;
		}


		public User(string email, string password)
		{
			_email = email;
			_password = password;
		}


		public bool ValidateCredentials()
		{

			if (!string.IsNullOrEmpty(_email) && !string.IsNullOrEmpty(_password))
			{

				return true;
			}
			return false;
		}
	}
}
