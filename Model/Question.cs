using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	[Table("Question")]
	public class Question
	{
		private int _questionId;
		private string _text;
		private int _gameId;
		private bool _isEnabled = true;
		private bool _isTapped;

		[PrimaryKey]
		public int QuestionId
		{
			get { return _questionId; }
			set { _questionId = value; }
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
			get { return _isEnabled; }
			set { _isEnabled = value; }
		}

		public bool IsTapped
		{
			get { return _isTapped; }
			set { _isTapped = value; }
		}

		private readonly SupabaseService _supabaseService = new SupabaseService();

		public Question(int questionId, string text, int gameId)
		{
			_questionId = questionId;
			_text = text;
			_gameId = gameId;
		}

		public Question(int questionId, string text)
		{
			_questionId = questionId;
			_text = text;
		}

		public Question(int questionId)
		{
			_questionId = questionId;
		}

		public Question(string text)
		{
			_text = text;
		}

		public Question() { }

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
