using System.ComponentModel;
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
		private bool _hostButtonEnabled = true;

		private int _countdown = 10;
		private System.Timers.Timer _timer;

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

		public bool HostButtonEnabled
		{
			get => _hostButtonEnabled;
			set
			{
				if (_hostButtonEnabled == value) return;

				_hostButtonEnabled = value;
				OnPropertyChanged(nameof(HostButtonEnabled));

				(NavigateToHostCommand as Command)?.ChangeCanExecute();
			}
		}

		public ICommand NavigateToSignupCommand { get; }
		public ICommand NavigateToHostCommand { get; }
		public ICommand NavigateToParticipantCommand { get; }

		public WelcomePageViewModel()
		{
			NavigateToSignupCommand = new Command(async () => await NavigateToSignup());
			NavigateToHostCommand = new Command(async () => await ExecuteNavigateToHostCommand(), CanExecuteNavigateToHost);
			NavigateToParticipantCommand = new Command(async () => await NavigateToParticipant());

			CheckLoginStatus();
		}

		public async void CheckLoginStatus()
		{
			_host = await _sqliteService.GetHostAsync();

			if (_host != null)
			{
				_hostButtonEnabled = false;
				HostButtonText = $"Schud om in te loggen als {_host.Name} ({_countdown})";
				HostButtonBackgroundColor = Colors.Green;
				App.Vibrate();

				StartShakeDetection();
				StartCountdown();
			}
		}

		private async Task ExecuteNavigateToHostCommand()
		{
			if (_hostButtonEnabled)
			{
				await NavigateToHost();
			}
		}

		private bool CanExecuteNavigateToHost()
		{
			return _hostButtonEnabled;
		}

		private void StartShakeDetection()
		{
			if (Accelerometer.Default.IsSupported)
			{
				if (!Accelerometer.Default.IsMonitoring)
				{
					Accelerometer.Default.ReadingChanged += DetectShake;
					Accelerometer.Default.Start(SensorSpeed.Game);
				}
			}
			else
			{
				Console.WriteLine("Accelerometer not supported.");
			}
		}

		private void StartCountdown()
		{
			_timer = new System.Timers.Timer(1000);
			_timer.Elapsed += (sender, e) => OnCountdownTick();
			_timer.Start();
		}

		private async void OnCountdownTick()
		{
			if (_countdown > 0)
			{
				HostButtonEnabled = false;
				_countdown--;
				HostButtonText = $"Schud om in te loggen als {_host.Name} ({_countdown})";
			}
			else
			{
				ResetButton();
				_timer.Stop();

				// Stop shake detection
				if (Accelerometer.Default.IsMonitoring)
				{
					Accelerometer.Default.Stop();
					Accelerometer.Default.ReadingChanged -= DetectShake;
				}

				await _sqliteService.ClearHostTableAsync();
				HostButtonEnabled = true;
			}
		}

		private async void DetectShake(object sender, AccelerometerChangedEventArgs e)
		{
			// Shake sensitivity - how hard you have to shake to trigger the login
			const double shakeThreshold = 3.0;

			double x = e.Reading.Acceleration.X;
			double y = e.Reading.Acceleration.Y;
			double z = e.Reading.Acceleration.Z;

			double magnitude = Math.Sqrt(x * x + y * y + z * z);

			if (magnitude > shakeThreshold)
			{
				_timer.Stop();
				Accelerometer.Default.Stop();
				Accelerometer.Default.ReadingChanged -= DetectShake;
				App.Vibrate();
				await App.Current.MainPage.Navigation.PushAsync(new HostMainPage(_host));
			}
		}

		private void ResetButton()
		{
			HostButtonText = "Speel als Host";
			HostButtonBackgroundColor = Colors.Black;
			_countdown = 10;
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
