using Blazored.LocalStorage;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Pages;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Services;

public class LeaveAllocationService : BaseHttpService, ILeaveAllocationService
{
    public LeaveAllocationService(IClient client, ILocalStorageService localStorageService) : base(client, localStorageService)
    {
    }

    public async Task<Response<Guid>> CreateLeaveAllocations(int leaveTypeId)
    {
        try
        {
            var response = new Response<Guid>();
            CreateLeaveAllocationCommand createLeaveAllocation = new CreateLeaveAllocationCommand() { LeaveTypeId = leaveTypeId };
            await Client.LeaveAllocationsPOSTAsync(createLeaveAllocation);
            return response;
        }
        catch (ApiException e)
        {
            return ConvertApiException<Guid>(e);
        }
    }
}