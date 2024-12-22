using SQLite;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	[SQLite.Table("Settings")]
	public class Settings
	{
		private int _primarykey;
		private int _maxplayercount;
		private readonly SQLiteService _sqliteService = new SQLiteService();

		[PrimaryKey]
		public int PrimaryKey
		{
			get { return _primarykey; }
			set { _primarykey = value; }
		}

		public int MaxPlayerCount
		{
			get { return _maxplayercount; }
			set { _maxplayercount = value; }
		}

		public Settings(int maxplayercount) 
		{
			_primarykey = 1;
			_maxplayercount = maxplayercount;
		}

		public Settings() { } // Local database needs parameterless constructor

		public async Task<bool> SaveSettingsAsync()
		{
			return await _sqliteService.SaveSettingsAsync(this);
		}

	}
}
