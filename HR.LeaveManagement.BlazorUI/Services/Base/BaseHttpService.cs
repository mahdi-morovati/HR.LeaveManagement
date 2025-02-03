using System.Net.Http.Headers;
using Blazored.LocalStorage;
using HR.LeaveManagement.BlazorUI.Pages;

namespace HR.LeaveManagement.BlazorUI.Services.Base;

public class BaseHttpService
{
    protected IClient Client;
    protected readonly ILocalStorageService LocalStorage;

    public BaseHttpService(IClient client, ILocalStorageService localStorage)
    {
        Client = client;
        LocalStorage = localStorage;
    }

    protected Response<Guid> ConvertApiException<Guid>(ApiException ex)
    {
        switch (ex.StatusCode)
        {
            case 400:
                return new Response<Guid>
                {
                    Message = "Invalid data was submitted",
                    ValidationErrors = ex.Response,
                    Success = false
                };

            case 404:
                return new Response<Guid>
                {
                    Message = "The record was not found",
                    Success = false
                };
            default:
                return new Response<Guid>
                {
                    Message = "Something went wrong, please try again later.",
                    Success = false
                };
        }
    }
    
    protected async Task AddBearerToken()
    {
        if (await LocalStorage.ContainKeyAsync("token"))
            Client.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await LocalStorage.GetItemAsync<string>("token"));
    }
    
}