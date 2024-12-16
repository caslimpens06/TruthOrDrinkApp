using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
namespace TruthOrDrink.Pages;

public partial class HostMainPage : FlyoutPage
{
    private readonly Host _host;
	private readonly SQLiteService _sqliteservice = new SQLiteService();

	public HostMainPage(Host host)
    {
        InitializeComponent();
        _host = host;
        IsPresented = false;
        LoadFlyoutData();
	}

    private void LoadFlyoutData()
    {
        if (_host.Name != null) {
            HostName.Text = _host.Name;
        }
    }
    protected override bool OnBackButtonPressed()
    {
        IsPresented = !IsPresented;
        return true;
    }

    private void OnHostChoosesGamePageClicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new HostChooseGamePage(_host));
        IsPresented = false;
    }

    private void OnProfilePageClicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new ProfilePage(_host));
        IsPresented = false;
    }

	private async void LogoutClicked(object sender, EventArgs e)
    {
        await _sqliteservice.ClearHostTableAsync();
        await _host.DeleteAllSessions();
        await Navigation.PopToRootAsync();
    }
}
