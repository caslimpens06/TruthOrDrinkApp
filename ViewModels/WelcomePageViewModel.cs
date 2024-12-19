﻿using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class WelcomePageViewModel : INotifyPropertyChanged
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private Host _host;

		private string _hostButtonText = "Speel als Host";
		private Color _hostButtonBackgroundColor = Colors.Black;

		public string HostButtonText
		{
			get => _hostButtonText;
			set
			{
				_hostButtonText = value;
				OnPropertyChanged(nameof(HostButtonText));
			}
		}

		public Color HostButtonBackgroundColor
		{
			get => _hostButtonBackgroundColor;
			set
			{
				_hostButtonBackgroundColor = value;
				OnPropertyChanged(nameof(HostButtonBackgroundColor));
			}
		}

		public ICommand NavigateToSignupCommand { get; }
		public ICommand NavigateToHostCommand { get; }
		public ICommand NavigateToParticipantCommand { get; }

		public WelcomePageViewModel()
		{
			NavigateToSignupCommand = new Command(async () => await NavigateToSignup());
			NavigateToHostCommand = new Command(async () => await NavigateToHost());
			NavigateToParticipantCommand = new Command(async () => await NavigateToParticipant());

			CheckLoginStatus();
		}

		public async void CheckLoginStatus()
		{
			_host = await _sqliteService.GetHostAsync();

			if (_host != null)
			{
				HostButtonText = $"Schud om in te loggen als {_host.Name}";
				HostButtonBackgroundColor = Colors.Green;

				StartShakeDetection();
			}
		}

		private void StartShakeDetection()
		{
			if (Accelerometer.Default.IsSupported)
			{
				if (!Accelerometer.Default.IsMonitoring)
				{
					Accelerometer.Default.ReadingChanged += DetectShake;
					Accelerometer.Default.Start(SensorSpeed.Game); // Use high responsiveness
				}
			}
			else
			{
				Console.WriteLine("Accelerometer not supported.");
			}
		}

		private async void DetectShake(object sender, AccelerometerChangedEventArgs e)
		{
			const double shakeThreshold = 2.5;

			double x = e.Reading.Acceleration.X;
			double y = e.Reading.Acceleration.Y;
			double z = e.Reading.Acceleration.Z;

			double magnitude = Math.Sqrt(x * x + y * y + z * z);
			Console.WriteLine(magnitude);

			if (magnitude > shakeThreshold)
			{
				Accelerometer.Default.Stop();
				Accelerometer.Default.ReadingChanged -= DetectShake;

				// Navigate to HostMainPage after shake detection
				await App.Current.MainPage.Navigation.PushAsync(new HostMainPage(_host));
			}
		}

		private async Task NavigateToSignup()
		{
			await App.Current.MainPage.Navigation.PushAsync(new SignUpPage());
		}

		private async Task NavigateToHost()
		{
			await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
		}

		private async Task NavigateToParticipant()
		{
			await App.Current.MainPage.Navigation.PushAsync(new GuestIdentifierPage());
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
