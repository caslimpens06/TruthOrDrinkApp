using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink.Model
{
	public class Question
	{
		private int _questionid;
		private string _text;
		private int _gameId;

		public int QuestionId
		{
			get { return _questionid; }
			set { _questionid = value; }
		}

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public int GameId
		{
			get { return _gameId; }
			set { _gameId = value; }
		}
		
		private readonly SupabaseService _supabaseService = new SupabaseService();

		// Constructor voor Question klasse
		public Question(int questionid, string text, int gameid)
		{
			_questionid = questionid;
			_text = text;
			_gameId = gameid;
		}

		public Question(int questionid, string text)
		{
			_questionid = questionid;
			_text = text;
		}

		public Question(int questionid)
		{
			_questionid = questionid;
		}

		public async void SetCurrentQuestion(Session session)
		{
			_supabaseService.SetCurrentQuestion(this, session);
		}


	}
}
