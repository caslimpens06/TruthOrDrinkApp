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
		private int _gamecode;

		public int ParticipantId
		{
			get { return _participantid; }
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

		public int Gamecode
		{
			get { return _gamecode; }
		}

		public Participant(int participantid, int gamecode)
		{
			_participantid = participantid;
			_gamecode = gamecode;
		}

		public Participant(string name, string gender)
		{
			_name = name;
			_gender = gender;
		}
	}
}
