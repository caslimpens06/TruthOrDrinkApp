using TruthOrDrink.Model;

namespace TruthOrDrink.Pages
{
	public partial class WaitOnHostPage : ContentPage
	{
		private Participant _participant;
		private SupabaseService _supabaseService;

		public WaitOnHostPage(Participant participant)
		{
			InitializeComponent();

			_participant = participant;
			_supabaseService = new SupabaseService();
			WaitOnHost();
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}

		public async void WaitOnHost()
		{
			bool notStarted = true;

			// Start the flickering label effect
			StartFlickering(notStarted);

			// Keep checking if the session has started every 5 seconds
			while (notStarted)
			{
				bool sessionStarted = await _supabaseService.CheckIfSessionHasStartedAsync(_participant);

				if (sessionStarted)
				{
					notStarted = false;
					StopFlickering();

					await Navigation.PushAsync(new ParticipantGamePage(_participant));
				}

				await Task.Delay(5000);
			}
		}


		public async void StartFlickering(bool flicker)
		{
			WaitingLabel.Opacity = 1;

			while (flicker)
			{
				WaitingLabel.Opacity = 0.1;
				await Task.Delay(500);

				WaitingLabel.Opacity = 1;
				await Task.Delay(500);
			}
		}


		public void StopFlickering()
		{
			WaitingLabel.Opacity = 1;
		}

		private async void LeaveGameClicked(object sender, EventArgs e)
		{ 
			await Navigation.PopToRootAsync();
		}

	}
}
