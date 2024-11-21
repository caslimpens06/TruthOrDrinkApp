namespace TruthOrDrink;

public partial class Game1 : ContentPage
{
	public Game1()
	{
		InitializeComponent();
	}

	private void OnTruthClicked(object sender, EventArgs e)
	{
		TruthButton2.IsVisible = false;
		DrinkButton2.IsVisible = false;

		QuestionLabel2.Text = "You chose Truth!";
		QuestionLabel2.TextColor = Colors.Green;
	}

	private void OnDrinkClicked(object sender, EventArgs e)
	{
		TruthButton2.IsVisible = false;
		DrinkButton2.IsVisible = false;

		QuestionLabel2.Text = "You chose Drink!";
		QuestionLabel2.TextColor = Colors.Red;
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton2.IsVisible = true;
		DrinkButton2.IsVisible = true;
		QuestionLabel2.Text = "Here comes the next question";
		QuestionLabel2.TextColor = Colors.Black;
	}
}