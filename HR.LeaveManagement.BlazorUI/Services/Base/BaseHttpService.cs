namespace HR.LeaveManagement.BlazorUI.Services.Base;

public class BaseHttpService
{
    protected IClient Client;

    public BaseHttpService(IClient client)
    {
        Client = client;
    }
}