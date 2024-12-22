using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TruthOrDrink.ViewModels
{
	public partial class SettingsPageViewModel : ObservableObject
	{
		[ObservableProperty]
		private string _exampleSetting = "Default Setting Value";


		public ICommand RefreshDrinkDataCommand { get; }
		public ICommand OpenInstagramCommand { get; }
		public ICommand OpenFacebookCommand { get; }
		public ICommand OpenLinkedInCommand { get; }

		public SettingsPageViewModel()
		{
			RefreshDrinkDataCommand = new Command(RefreshDrinkData);
			OpenInstagramCommand = new Command(OpenInstagram);
			OpenFacebookCommand = new Command(OpenFacebook);
			OpenLinkedInCommand = new Command(OpenLinkedIn);
		}

		private void RefreshDrinkData() { }

		private void OpenInstagram()
		{
			Launcher.OpenAsync("https://www.instagram.com/cas_limpens");
		}

		private void OpenFacebook()
		{
			Launcher.OpenAsync("https://www.facebook.com/profile.php?id=100076414400998");
		}

		private void OpenLinkedIn()
		{
			Launcher.OpenAsync("https://www.linkedin.com/in/cas-limpens-a091742b1/");
		}
	}
}
