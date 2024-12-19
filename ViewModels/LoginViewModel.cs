﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TruthOrDrink.Model;
using TruthOrDrink.DataAccessLayer;
using TruthOrDrink.Pages;
using TruthOrDrink.View;

namespace TruthOrDrink.ViewModels
{
	public class LoginViewModel : ObservableObject
	{
		private string _email;
		private string _password;
		private readonly SQLiteService _sqliteservice;
		private readonly INavigation _navigation;

		public LoginViewModel(INavigation navigation)
		{
			_navigation = navigation;
			_sqliteservice = new SQLiteService();
			LoginCommand = new AsyncRelayCommand(Login);
		}

		public string Email
		{
			get => _email;
			set => SetProperty(ref _email, value);
		}

		public string Password
		{
			get => _password;
			set => SetProperty(ref _password, value);
		}

		public IAsyncRelayCommand LoginCommand { get; }

		private async Task Login()
		{
			if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
			{
				await App.Current.MainPage.DisplayAlert("Ongeldige invoer", "Voer een geldig e-mailadres in.", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				await App.Current.MainPage.DisplayAlert("Ongeldig wachtwoord", "Voer je wachtwoord in.", "OK");
				return;
			}

			Host host = new Host(Email);
			string storedOnlineHash = await host.ValidateCredentialsAsync();

			PasswordHasher passwordHasher = new PasswordHasher(Password, storedOnlineHash);
			bool correctCredentials = passwordHasher.VerifyPassword();

			if (correctCredentials)
			{
				int hostId = await host.GetHostPrimaryKey();
				string name = await host.GetHostName();
				Host newHost = new Host(hostId, name, Email, Password);
				await _sqliteservice.SaveHostAsync(newHost);
				await _navigation.PushAsync(new HostMainPage(newHost));
			}
			else
			{
				await App.Current.MainPage.DisplayAlert("Login Mislukt", "Email of wachtwoord is incorrect.", "OK");
			}
		}

		private static bool IsValidEmail(string email)
		{
			try
			{
				email = email.Trim();
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}
}