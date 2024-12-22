using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.Model;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public partial class WaitOnHostViewModel : INotifyPropertyChanged
	{
		private Participant _participant;
		private double _waitingLabelOpacity;
		private bool _isFlickering;
		private bool _notStarted;

		public double WaitingLabelOpacity
		{
			get => _waitingLabelOpacity;
			set
			{
				if (_waitingLabelOpacity != value)
				{
					_waitingLabelOpacity = value;
					OnPropertyChanged(nameof(WaitingLabelOpacity));
				}
			}
		}

		public ICommand LeaveGameCommand { get; }

		public WaitOnHostViewModel(Participant participant)
		{
			_participant = participant;
			WaitingLabelOpacity = 1;
			LeaveGameCommand = new Command(async () => await LeaveGame());

			WaitOnHost();
		}

		private async void WaitOnHost()
		{
			_notStarted = true;

			// Start flickering the label
			StartFlickering();

			// Continuously check if the session has started
			while (_notStarted)
			{
				Session session = new Session(_participant.SessionCode);
				bool sessionStarted = await session.CheckIfSessionHasStarted();
				if (sessionStarted)
				{
					_notStarted = false;
					StopFlickering();
					await App.Current.MainPage.Navigation.PushAsync(new ParticipantGamePage(_participant));
				}

				await Task.Delay(2000);
			}
		}

		private async void StartFlickering()
		{
			_isFlickering = true;

			while (_isFlickering)
			{
				WaitingLabelOpacity = 0.1;
				await Task.Delay(500);
				WaitingLabelOpacity = 1;
				await Task.Delay(500);
			}
		}

		public void StopFlickering()
		{
			_isFlickering = false;
			WaitingLabelOpacity = 1;
		}

		private async Task LeaveGame()
		{
			await _participant.RemoveParticipantAsync();
			await App.Current.MainPage.Navigation.PopToRootAsync();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
