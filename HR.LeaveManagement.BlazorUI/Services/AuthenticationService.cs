using Blazored.LocalStorage;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Providers;
using HR.LeaveManagement.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace HR.LeaveManagement.BlazorUI.Services;

public class AuthenticationService : BaseHttpService, IAuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    public AuthenticationService(IClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider) : base(client, localStorage)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        try
        {
            AuthRequest authenticationRequest = new AuthRequest() { Email = email, Password = password };
            var authenticationResponse = await Client.LoginAsync(authenticationRequest);
            if (authenticationResponse.Token != String.Empty)
            {
                await LocalStorage.SetItemAsync("token", authenticationResponse.Token);
                // Set claims in Blazor and login state
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string firstName, string lastName, string userName, string email,
        string password)
    {
        RegisterationRequest registrationRequest = new RegisterationRequest()
            { FirstName = firstName, LastName = lastName, UserName = userName, Email = email, Password = password };
        var response = await Client.RegisterAsync(registrationRequest);

        if (!string.IsNullOrEmpty(response.UserId))
        {
            return true;
        }

        return false;
    }

    public async Task Logout()
    {
        // remove claims in Blazor and invalidate login state
        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
    }
}