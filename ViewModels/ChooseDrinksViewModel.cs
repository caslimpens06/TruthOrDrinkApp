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
		private bool _nodrinks = false;

		private string _buttonText = "Ga verder";
		public string ButtonText
		{
			get => _buttonText;
			set => SetProperty(ref _buttonText, value);
		}

		public IAsyncRelayCommand ContinueCommand { get; }

		public IRelayCommand BackToMainMenuCommand { get; }

		public IRelayCommand<Drink> ToggleDrinkSelectionCommand { get; }

		public ChooseDrinksViewModel(Session session, List<Participant> participants)
		{
			_session = session;
			_participants = participants;
			_mode = false;
			ContinueCommand = new AsyncRelayCommand(Continue);
			BackToMainMenuCommand = new RelayCommand(OnBackToMainMenuClicked);
			ToggleDrinkSelectionCommand = new RelayCommand<Drink>(ToggleDrinkSelection);
			LoadDrinksAsync();
		}

		public ChooseDrinksViewModel(Session session)
		{
			_session = session;
			_mode = true;
			ContinueCommand = new AsyncRelayCommand(Continue);
			BackToMainMenuCommand = new RelayCommand(OnBackToMainMenuClicked);
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

		private async Task LoadDrinksAsync()
		{
			var drinks = await _sqliteService.GetDrinksFromLocalDatabase();

			if (drinks == null || drinks.Count == 0)
			{
				AvailableDrinks = new ObservableCollection<Drink> { new Drink("Fout", "Geen dranken gevonden...") };
				ButtonText = "Terug naar hoofdmenu";
				_nodrinks = true;
			}
			else
			{
				AvailableDrinks = new ObservableCollection<Drink>(drinks);
				ButtonText = "Ga verder";
			}
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
			if (SelectedDrinks.Count > 0 || _nodrinks)
			{
				if (_mode)
				{
					string json = JsonSerializer.Serialize(SelectedDrinks.ToList());
					Session drinksinjson = new Session(_session.SessionCode, json);
					await drinksinjson.AddDrinksToSession();
					await _session.StartGame();
					await App.Current.MainPage.Navigation.PushAsync(new HostControlsGamePage(_session));
				}
				else
				{
					if (_nodrinks) 
					{
						await App.Current.MainPage.Navigation.PopToRootAsync();
					}
					else
					{
						await App.Current.MainPage.Navigation.PushAsync(new ControlOfflineGamePage(_session, _participants, SelectedDrinks.ToList()));
					}
				}
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Waarschuwing", "Je moet minstens één drankje selecteren.", "OK");
			}
		}

		private async void OnBackToMainMenuClicked()
		{
			await App.Current.MainPage.Navigation.PushAsync(new OfflineMode());
		}
	}
}
