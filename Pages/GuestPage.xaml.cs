using System.Runtime.CompilerServices;
using TruthOrDrink.Model;
using TruthOrDrink.Pages;

namespace TruthOrDrink;

public partial class GuestPage : ContentPage
{
	private int _participantid;
	public GuestPage(int participantid)
	{
		InitializeComponent();
		_participantid = participantid;
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
			SupabaseService supabaseService = new SupabaseService();

			if (await supabaseService.CheckIfSessionExistsAsync(parsedSessionCode))
			{	
				bool hasStarted = await supabaseService.CheckIfSessionHasStarted(parsedSessionCode);
				if (!hasStarted)
				{
					Participant participant = new Participant(_participantid, parsedSessionCode);
					Session session = new Session(parsedSessionCode);
					await supabaseService.JoinParticipantToSession(participant);

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
				await DisplayAlert("Ongeldige gamecode", "VerifiŽer of de host al een spel heeft gemaakt.", "OK");
			}
		}
	}

	private void QR_Clicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new QRScannerPage(_participantid));
	}

	private async void LeaveGameClicked(object sender, EventArgs e)
	{
		await Navigation.PopToRootAsync();
	}
}