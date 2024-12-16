using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	public class Participant
	{
		private int _participantid;
		private string _name;
		private string _gender;
		private int _sessioncode;
		private int _truthordrinkcount;
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

		public int TruthOrDrinkCount
		{
			get { return _truthordrinkcount; }
			set { _truthordrinkcount = value; }
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

		public Participant(int participantid, string name, string gender)
		{
			_participantid = participantid;
			_name = name;
			_gender = gender;
		}

		public Participant(string name, string gender, int truthordrinkcount)
		{
			_name = name;
			_gender = gender;
			_truthordrinkcount = truthordrinkcount;
		}

		public string GenderImage => GetImageSourceForGender(Gender);

		private string GetImageSourceForGender(string gender)
		{
			if (string.IsNullOrEmpty(gender)) return "blankimage.png";

			gender = gender.ToLower();
			return gender switch
			{
				"man" => "male.png",
				"vrouw" => "female.png",
				_ => "blankimage.png"
			};
		}


		public async Task<Game> GetGameBySessionId() 
		{
			return await _supabaseService.GetGameBySessionId(this);
		}

		public async Task SetDoneAddingQuestions()
		{
			await _supabaseService.SetDoneAddingQuestions(this);
		}

		public async Task<Question> GetCurrentQuestionAsync()
		{
			return await _supabaseService.GetCurrentQuestionAsync(this);
		}

		public async Task SetAllQuestionsToAnswered() 
		{
			await _supabaseService.SetAllQuestionsToAnswered(this);
		}

		public async Task<bool> CheckIfAllQuestionsAnswered() 
		{
			return await _supabaseService.CheckIfAllQuestionsAnswered(this);
		}

		public async Task<Participant> GetMostTruths()
		{
			return await _supabaseService.GetTopTruth(this);
		}

		public async Task<Participant> GetMostDrinks()
		{
			return await _supabaseService.GetTopDrinker(this);
		}

		public async Task RemoveParticipantAsync()
		{
			await _supabaseService.RemoveParticipantAsync(this);
		}

		public async Task<int> GetParticipantPrimarykey()
		{
			return await _supabaseService.GetParticipantPrimarykey(this);
		}

		public async Task<bool> AddParticipantIfNotExists()
		{
			return await _supabaseService.AddParticipantIfNotExists(this);
		}

		public async Task JoinParticipantToSession()
		{
			await _supabaseService.JoinParticipantToSession(this);
		}

		public async Task SetGameToClose()
		{
			await _supabaseService.SetSessionToClose(this);
		}

		public async Task<bool> CheckIfGameClosed()
		{
			return await _supabaseService.CheckIfGameClosed(this);
		}
	}
}
