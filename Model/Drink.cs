using System.Collections.Generic;
using System.Text.Json.Serialization;
using SQLite;

namespace TruthOrDrink.Model
{
	[Table("Drink")]
	public class Drink
	{
		// The _name and _type fields can be null or empty now
		private string _name;
		private string _type;

		// Primary key for the Drink table
		[PrimaryKey]
		[JsonPropertyName("name")]
		public string Name
		{
			get { return _name; }
			set { _name = value ?? string.Empty; } // Ensure name isn't null
		}

		// Type property with no nullable constraints
		[JsonPropertyName("type")]
		public string Type
		{
			get { return _type; }
			set { _type = value ?? string.Empty; } // Default to empty if null
		}

		// Parameterized constructor
		public Drink(string name, string type)
		{
			_name = name ?? string.Empty;  // Ensure name isn't null
			_type = type ?? string.Empty;  // Ensure type isn't null
		}

		// Default constructor for SQLite and other usages
		public Drink() { }
	}
}
