namespace TruthOrDrink;

public partial class Game4 : ContentPage
{
	public Game4()
	{
		InitializeComponent();
	}
	private void OnTruthClicked(object sender, EventArgs e)
	{
		TruthButton5.IsVisible = false;
		DrinkButton5.IsVisible = false;

		QuestionLabel5.Text = "You chose Truth!";
		QuestionLabel5.TextColor = Colors.Green;
	}

	private void OnDrinkClicked(object sender, EventArgs e)
	{
		TruthButton5.IsVisible = false;
		DrinkButton5.IsVisible = false;

		QuestionLabel5.Text = "You chose Drink!";
		QuestionLabel5.TextColor = Colors.Red;
	}

	private void OnNextClicked(object sender, EventArgs e)
	{
		TruthButton5.IsVisible = true;
		DrinkButton5.IsVisible = true;
		QuestionLabel5.Text = "Here comes the next question";
		QuestionLabel5.TextColor = Colors.Black;
	}
}