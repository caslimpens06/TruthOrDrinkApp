using System.ComponentModel;
using System.Windows.Input;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink.ViewModels
{
	public partial class WaitOnHostViewModel : INotifyPropertyChanged
	{
		private readonly SupabaseService _supabaseService;
		private readonly INavigation _navigation;
		private Participant _participant;
		private double _waitingLabelOpacity;
		private bool _isFlickering;

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

		public WaitOnHostViewModel(Participant participant, INavigation navigation)
		{
			_participant = participant;
			_supabaseService = new SupabaseService();
			_navigation = navigation;
			WaitingLabelOpacity = 1;
			LeaveGameCommand = new Command(async () => await LeaveGame());
			WaitOnHost();
		}

		private async void WaitOnHost()
		{
			bool notStarted = true;
			StartFlickering();

			while (notStarted)
			{
				bool sessionStarted = await _supabaseService.CheckIfSessionHasStartedAsync(_participant);

				if (sessionStarted)
				{
					notStarted = false;
					StopFlickering();
					await _navigation.PushAsync(new ParticipantGamePage(_participant));
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
			await _navigation.PopToRootAsync();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}