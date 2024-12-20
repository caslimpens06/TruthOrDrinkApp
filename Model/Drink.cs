using System.Text.Json.Serialization;
using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TruthOrDrink.Model
{
	[Table("Drink")]
	public partial class Drink : ObservableObject
	{
		private string _name;
		private string _type;
		private bool _isSelected;
		public Color BackgroundColor => IsSelected ? Colors.LightGreen : Colors.White;

		[PrimaryKey]
		[JsonPropertyName("name")]
		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value ?? string.Empty);
		}

		[JsonPropertyName("type")]
		public string Type
		{
			get => _type;
			set => SetProperty(ref _type, value ?? string.Empty);
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				SetProperty(ref _isSelected, value);
				OnPropertyChanged(nameof(BackgroundColor));
			}
		}

		public Drink(string name, string type)
		{
			_name = name ?? string.Empty;
			_type = type ?? string.Empty;
		}

		public Drink() { }
	}
}
