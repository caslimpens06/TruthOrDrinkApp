using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthOrDrink.Model
{
	public class Question
	{
		private int _id;
		private string _text;
		private int _gameId;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
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
		
		private readonly SupabaseService _supabaseService;

		// Constructor voor Question klasse
		public Question(SupabaseService supabaseService)
		{
			_supabaseService = supabaseService;
		}

		// Verkrijg alle vragen voor dit specifieke Game
		public async Task<List<Question>> GetQuestionsForGameAsync(Game game)
		{
			return await _supabaseService.GetQuestionsByGameIdAsync(game);
		}
	}
}
