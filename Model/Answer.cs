﻿using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
    public class Answer
    {
		private int _id;
		private int _questionid;
		private string _response;
		private int _participantid;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int QuestionId
		{
			get { return _questionid; }
			set { _questionid = value; }
		}

		public string Response
		{
			get { return _response; }
			set { _response = value; }
		}
		public int ParticipantId
		{
			get { return _participantid; }
			set { _participantid = value; }
		}

		private readonly SupabaseService _supabaseService = new SupabaseService();

		public Answer(int questionid, string response, int participantid) 
		{ 
			_questionid = questionid;
			_response = response;
			_participantid = participantid;
		}

		public async Task SaveAnswerAsync()
		{
			await _supabaseService.SaveAnswerAsync(this);
		}
	}
}
