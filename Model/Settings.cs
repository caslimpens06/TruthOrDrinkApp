using SQLite;
using TruthOrDrink.DataAccessLayer;

namespace TruthOrDrink.Model
{
	[SQLite.Table("Settings")]
	public class Settings
	{
		private int _primarykey;
		private int _maxplayercount;
		private string _country;
		private string _area;
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

		public string Country
		{
			get { return _country; }
			set { _country = value; }
		}

		public string Area
		{
			get { return _area; }
			set { _area = value; }
		}

		public Settings(int maxplayercount) 
		{
			_primarykey = 1;
			_maxplayercount = maxplayercount;
		}

		public Settings(string country, string area) 
		{
			_country = country;
			_area = area;
		}

		public Settings() { } // Local database needs parameterless constructor

		public async Task<bool> SaveSettingsAsync()
		{
			return await _sqliteService.SaveSettingsAsync(this);
		}

		public async Task SaveLocationLocallyAsync()
		{
			await _sqliteService.SaveLocationLocallyAsync(this);
		}

	}
}
