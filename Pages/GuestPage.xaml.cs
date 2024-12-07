using System.Runtime.CompilerServices;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class GuestPage : ContentPage
{
	private readonly Participant _participant;

	public GuestPage(Participant participant)
	{
		InitializeComponent();
		_participant = participant;
	}

	protected override bool OnBackButtonPressed()
	{
		return true;
	}

	private async void Connect_Clicked(object sender, EventArgs e)
	{
		string sessionCode = SessionCodeEntry.Text;
		if (string.IsNullOrWhiteSpace(sessionCode) || sessionCode.Length != 6)
		{
			await DisplayAlert("Ongeldige gamecode", "Voer de gamecode in die op het hostapparaat staat. Dit is een 6-cijferige code.", "OK");
			return;
		}
		else 
		{
			int parsedSessionCode = Int32.Parse(sessionCode);
			Session session = new Session(parsedSessionCode);

			if (await session.CheckIfSessionExistsAsync())
			{	
				bool hasStarted = await session.CheckIfSessionHasStarted();
				if (!hasStarted)
				{
					Participant participant = new Participant(_participant.ParticipantId, parsedSessionCode);
					Console.WriteLine($"{_participant.ParticipantId} {parsedSessionCode}");
					await participant.JoinParticipantToSession();

					if (await session.CheckIfCustomGame())
					{
						await Navigation.PushAsync(new QuestionInputPage(participant));
					}

					else
					{
						await Navigation.PushAsync(new WaitOnHostPage(participant));
					}
				}
				else 
				{
					await DisplayAlert("Foutmelding", "Helaas, je kan niet meer deelnemen.", "Ok");
				}
			}
			else 
			{
				await DisplayAlert("Ongeldige gamecode", "Verifiëer of de host al een spel heeft gemaakt.", "OK");
			}
		}
	}

	private void QR_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new QRScannerPage(_participant));
	}

	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await _participant.RemoveParticipantAsync();
		await Navigation.PopToRootAsync();
	}
}