using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDrink.DataAccessLayer;
using SQLite;

namespace TruthOrDrink.Model
{
	[SQLite.Table("Question")]
	public class Question
	{
		private int _questionid;
		private string _text;
		private int _gameId;
		private bool _isenabled = true;

		[PrimaryKey]
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
		public bool IsEnabled 
		{  
			get { return _isenabled; }
			set { _isenabled = value; }
		}


		private readonly SupabaseService _supabaseService = new SupabaseService();

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

		public Question(string text)
		{
			_text = text;
		}

		public Question() { } // Local database needs parameterless constructor
		public async Task AddQuestionByParticipant()
		{
			_supabaseService.AddQuestionByParticipant(this);
		}


		public async Task SetCurrentQuestion(Session session)
		{
			_supabaseService.SetCurrentQuestion(this, session);
		}

		public async Task<bool> CheckIfAnswerHasBeenGiven(Session session)
		{
			return await _supabaseService.CheckIfAnswerHasBeenGiven(this, session);
		}
	}
}
