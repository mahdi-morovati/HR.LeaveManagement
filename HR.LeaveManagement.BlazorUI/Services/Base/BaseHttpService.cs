using HR.LeaveManagement.BlazorUI.Pages;

namespace HR.LeaveManagement.BlazorUI.Services.Base;

public class BaseHttpService
{
    protected IClient Client;

    public BaseHttpService(IClient client)
    {
        Client = client;
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
}