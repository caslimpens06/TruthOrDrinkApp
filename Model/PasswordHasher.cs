using Microsoft.AspNetCore.Identity;

namespace TruthOrDrink.Model
{
	public class PasswordHasher
	{
		private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();
		private string _password;
		private string _hashedPassword;
		public PasswordHasher(string password)
		{
			_password = password;
		}

		public PasswordHasher(string password, string hashedpassword)
		{
			_password = password;
			_hashedPassword = hashedpassword;
		}

		public string HashPassword()
		{
			if (string.IsNullOrEmpty(_password))
			{
				return null;
			}

			return _passwordHasher.HashPassword(null, _password);
		}

		public bool VerifyPassword()
		{
			if (string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_hashedPassword))
			{
				return false;
			}

			var result = _passwordHasher.VerifyHashedPassword(null, _hashedPassword, _password);
			return result == PasswordVerificationResult.Success;
		}

	}
}
