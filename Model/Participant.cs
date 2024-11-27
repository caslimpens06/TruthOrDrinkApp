using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink.Model
{
	public class Participant
	{
		private int _participantid;
		private string _name;
		private string _gender;
		private int _sessioncode;
		private SupabaseService _supabaseService = new SupabaseService();

		public int ParticipantId
		{
			get { return _participantid; }
			set { _participantid = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		public string Gender
		{
			get { return _gender; }
			set { _gender = value; }
		}

		public int SessionCode
		{
			get { return _sessioncode; }
			set { _sessioncode = value; }
		}

		public Participant(int participantid, int sessioncode)
		{
			_participantid = participantid;
			_sessioncode = sessioncode;
		}
		public Participant(int sessioncode)
		{
			_sessioncode = sessioncode;

		}
		public Participant(string name, string gender)
		{
			_name = name;
			_gender = gender;
		}
		public async Task<Game> GetGameBySessionId() 
		{
			return await _supabaseService.GetGameBySessionId(this);
		}
	}
}
