namespace TruthOrDrink;

public partial class Game3 : ContentPage
{
	public Game3()
	{
		InitializeComponent();
	}
	private void OnTruthClicked(object sender, EventArgs e)
	{
		TruthButton4.IsVisible = false;
		DrinkButton4.IsVisible = false;

		QuestionLabel4.Text = "You chose Truth!";
		QuestionLabel4.TextColor = Colors.Green;
	}

	private void OnDrinkClicked(object sender, EventArgs e)
	{
		TruthButton4.IsVisible = false;
		DrinkButton4.IsVisible = false;

		QuestionLabel4.Text = "You chose Drink!";
		QuestionLabel4.TextColor = Colors.Red;
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton4.IsVisible = true;
		DrinkButton4.IsVisible = true;
		QuestionLabel4.Text = "Here comes the next question";
		QuestionLabel4.TextColor = Colors.Black;
	}
}