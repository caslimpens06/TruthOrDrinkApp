namespace TruthOrDrink;

public partial class Game2 : ContentPage
{
	public Game2()
	{
		InitializeComponent();
	}
	private void OnTruthClicked(object sender, EventArgs e)
	{
		TruthButton3.IsVisible = false;
		DrinkButton3.IsVisible = false;

		QuestionLabel3.Text = "You chose Truth!";
		QuestionLabel3.TextColor = Colors.Green;
	}

	private void OnDrinkClicked(object sender, EventArgs e)
	{
		TruthButton3.IsVisible = false;
		DrinkButton3.IsVisible = false;

		QuestionLabel3.Text = "You chose Drink!";
		QuestionLabel3.TextColor = Colors.Red;
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton3.IsVisible = true;
		DrinkButton3.IsVisible = true;
		QuestionLabel3.Text = "Here comes the next question";
		QuestionLabel3.TextColor = Colors.Black;
	}
}