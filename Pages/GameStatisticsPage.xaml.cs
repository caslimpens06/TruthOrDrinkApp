using Supabase.Gotrue;
using TruthOrDrink.Model;

namespace TruthOrDrink.Pages;

public partial class GameStatisticsPage : ContentPage
{

	private Participant _toptruthparticipant;
	private Participant _topdrinkparticipant;
	private Participant _participant;
	public Participant TopTruthParticipant
	{
		get { return _toptruthparticipant; }
		set { _toptruthparticipant = value; OnPropertyChanged(); }
	}

	public Participant TopDrinkParticipant
	{
		get { return _topdrinkparticipant; }
		set { _topdrinkparticipant = value; OnPropertyChanged(); }
	}

	public GameStatisticsPage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
		_ = InitializeData();
		BindingContext = this;
	}

	private async Task InitializeData()
	{
		_toptruthparticipant = await GetParticipantWithMostTruthAnswers();
		_topdrinkparticipant = await GetParticipantWithMostDrinkAnswers();

		if (_toptruthparticipant?.Name == _topdrinkparticipant?.Name)
		{
			TruthLabel.Text = _toptruthparticipant?.Name ?? "Geen Data";
			DrinkLabel.Text = _topdrinkparticipant?.Name ?? "Geen Data";
			TopTruthCount.Text = $"Je vulde {_toptruthparticipant.TruthOrDrinkCount} keer Truth in.";
			TopDrinkCount.Text = $"Je vulde {_topdrinkparticipant.TruthOrDrinkCount} keer Drink in.";
			TopTruthImage.Source = _toptruthparticipant.GenderImage;
			TopDrinkImage.Source = _topdrinkparticipant.GenderImage;
		}
		else
		{
			TopTruthImage.Source = _toptruthparticipant.GenderImage;
			TopDrinkImage.Source = _topdrinkparticipant.GenderImage;
			TruthLabel.Text = _toptruthparticipant?.Name ?? "Geen Data";
			DrinkLabel.Text = _topdrinkparticipant?.Name ?? "Geen Data";
			TopTruthCount.Text = $"Meeste keren Truth: {_toptruthparticipant?.TruthOrDrinkCount ?? 0}";
			TopDrinkCount.Text = $"Meeste keren Drink: {_topdrinkparticipant?.TruthOrDrinkCount ?? 0}";
		}
	}


	private async Task<Participant> GetParticipantWithMostTruthAnswers()
	{
		return await _participant.GetMostTruths();
	}

	private async Task<Participant> GetParticipantWithMostDrinkAnswers()
	{
		return await _participant.GetMostDrinks();
	}

}