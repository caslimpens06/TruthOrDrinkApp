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
		private int _gamecode;

		public int ParticipantId
		{
			get { return _participantid; }
		}

		public string Name
		{
			get { return _name; }
		}

		public int Gamecode
		{
			get { return _gamecode; }
		}
		public Participant(int participantid, string name, int gamecode)
		{
			_participantid = participantid;
			_name = name;
			_gamecode = gamecode;
		}
		public Participant(string name)
		{
			_name = name;
		}
		public Participant() { }
	}
}
