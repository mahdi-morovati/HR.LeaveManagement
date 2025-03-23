using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace HR.LeaveManagement.BlazorUI.Providers;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var isTokenPresent = await _localStorage.ContainKeyAsync("token");

        if (!isTokenPresent)
        {
            return new AuthenticationState(user);
        }

        var savedToken = await _localStorage.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(savedToken))
        {
            return new AuthenticationState(user);
        }

        var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

        // 🔹 Fix: Compare with UTC time
        if (tokenContent.ValidTo < DateTime.UtcNow)
        {
            await _localStorage.RemoveItemAsync("token");
            return new AuthenticationState(user);
        }

        var claims = ParseClaimsFromJwt(savedToken);
        user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        return new AuthenticationState(user);
    }

    public async Task LoggedIn()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(await GetClaims(), "jwt"));
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task LoggedOut()
    {
        await _localStorage.RemoveItemAsync("token");
        var nobody = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(nobody)));
    }

    private async Task<List<Claim>> GetClaims()
    {
        var savedToken = await _localStorage.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(savedToken)) return new List<Claim>();

        return ParseClaimsFromJwt(savedToken);
    }

    private List<Claim> ParseClaimsFromJwt(string token)
    {
        var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var claims = tokenContent.Claims.ToList();
        claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
        return claims;
    }
}
