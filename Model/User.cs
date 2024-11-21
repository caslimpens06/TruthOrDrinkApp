using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink
{
    class User
    {
        private string _name;
        private string _email;
        private string _password;

        public User(string name, string email, string password) 
        {
            name = _name;
            email = _email; 
            password = _password;
        }
		public User(string email, string password)
		{
			email = _email;
			password = _password;
		}

        public bool ValidateCredentials() 
        { 
        return true;
            // Call dalsql voor datavalidatie
        }
	}
}
