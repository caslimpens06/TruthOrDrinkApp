using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using System.Collections.ObjectModel;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.View;
using System.Text.Json;

namespace TruthOrDrink.ViewModels
{
	public partial class ChooseDrinksViewModel : ObservableObject
	{
		private readonly SQLiteService _sqliteService = new SQLiteService();
		private ObservableCollection<Drink> _availableDrinks = new ObservableCollection<Drink>();
		private ObservableCollection<Drink> _selectedDrinks = new ObservableCollection<Drink>();
		private readonly Session _session;
		private readonly List<Participant> _participants;
		private bool _mode; // true is online - false is offline

		public ChooseDrinksViewModel(Session session, List<Participant> participants)
		{
			_session = session;
			_participants = participants;
			_mode = false;
			ContinueCommand = new AsyncRelayCommand(Continue);
			ToggleDrinkSelectionCommand = new RelayCommand<Drink>(ToggleDrinkSelection);
			LoadDrinksAsync();
		}
		public ChooseDrinksViewModel(Session session)
		{
			_session = session;
			_mode = true;
			ContinueCommand = new AsyncRelayCommand(Continue);
			ToggleDrinkSelectionCommand = new RelayCommand<Drink>(ToggleDrinkSelection);
			LoadDrinksAsync();
		}

		public ChooseDrinksViewModel() { }

		public ObservableCollection<Drink> AvailableDrinks
		{
			get => _availableDrinks;
			set => SetProperty(ref _availableDrinks, value);
		}

		public ObservableCollection<Drink> SelectedDrinks
		{
			get => _selectedDrinks;
			set => SetProperty(ref _selectedDrinks, value);
		}

		public string SelectedDrinksCountDisplay => $"Geselecteerde drankjes ({SelectedDrinks.Count})";

		public IAsyncRelayCommand ContinueCommand { get; }
		public IRelayCommand<Drink> ToggleDrinkSelectionCommand { get; }

		private async Task LoadDrinksAsync()
		{
			var drinks = await _sqliteService.GetDrinksFromLocalDatabase();
			AvailableDrinks = new ObservableCollection<Drink>(drinks);
		}

		private void ToggleDrinkSelection(Drink drink)
		{
			if (drink == null) return;

			drink.IsSelected = !drink.IsSelected;

			if (drink.IsSelected)
			{
				SelectedDrinks.Add(drink);
			}
			else
			{
				SelectedDrinks.Remove(drink);
			}

			OnPropertyChanged(nameof(SelectedDrinksCountDisplay));
		}



		public bool IsDrinkSelected(Drink drink) => SelectedDrinks.Contains(drink);

		private async Task Continue()
		{
			if (SelectedDrinks.Count > 0)
			{
				if (_mode)
				{
					string json = JsonSerializer.Serialize(SelectedDrinks.ToList());
					Session drinksinjson = new Session(_session.SessionCode, json);
					await drinksinjson.AddDrinksToSession();
					await _session.StartGame();
					await Application.Current.MainPage.Navigation.PushAsync(new HostControlsGamePage(_session));
					
				}
				else 
				{
					await Application.Current.MainPage.Navigation.PushAsync(new ControlOfflineGamePage(_session, _participants, SelectedDrinks.ToList()));
				}
			}
			else
			{
				await Application.Current.MainPage.DisplayAlert("Waarschuwing", "Je moet minstens één drankje selecteren.", "OK");
			}
		}
	}
}
