using System;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace TruthOrDrink
{
	public class GenderToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
			{
				return "blankimage.png";
			}

			string gender = value.ToString().ToLower();
			Console.WriteLine($"Gender value is: {gender}");

			if (gender == "vrouw")
			{
				return "female.png";
			}
			else if (gender == "man")
			{
				return "male.png";
			}
			else
			{
				return "blankimage.png";  // Default image
			}
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
