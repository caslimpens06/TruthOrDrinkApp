using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	public class Participant
	{
		private int _participantid;
		private string _name;
		private string _gender;
		private int _sessioncode;
		private int _truthcount;
		private int _drinkcount;
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

		public int TruthCount
		{
			get { return _truthcount; }
			set { _truthcount = value; }
		}
		public int DrinkCount
		{
			get { return _drinkcount; }
			set { _drinkcount = value; }
		}

		public bool HasAnswered { get; set; }

		public Participant(int participantid, int sessioncode, string name)
		{
			_participantid = participantid;
			_sessioncode = sessioncode;
			_name = name;
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

		public Participant(string name, string gender, int truthcount)
		{
			_name = name;
			_gender = gender;
			_truthcount = truthcount;
		}

		public Participant(string name, string gender, int drinkcount, string placeholder)
		{
			_name = name;
			_gender = gender;
			_drinkcount = drinkcount;
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

		public async Task SetSessionToClose()
		{
			await _supabaseService.SetSessionToClose(this);
		}

		public async Task<bool> CheckIfSessionClosed()
		{
			return await _supabaseService.CheckIfGameClosed(this);
		}

		public async Task<string> GetDrinksBySession()
		{
			return await _supabaseService.GetDrinksBySession(this);
		}
	}
}
