using AutoMapper;
using Blazored.LocalStorage;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveAllocations;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Pages;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Services;

public class LeaveRequestService : BaseHttpService, ILeaveRequestService
{
    private readonly IMapper _mapper;
    public LeaveRequestService(IClient client, ILocalStorageService localStorageService, IMapper mapper) : base(client, localStorageService)
    {
        _mapper = mapper;
    }

    public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
    {
        var leaveRequests = await Client.LeaveRequestsAllAsync(isLoggedInUser: false);
        
        var model = new AdminLeaveRequestViewVM
        {
            TotalRequests = leaveRequests.Count,
            ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
            PendingRequests = leaveRequests.Count(q => q.Approved == null),
            RejectedRequests = leaveRequests.Count(q => q.Approved == false),
            LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
        };
        return model;
    }

    public async Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
    {
        var leaveRequests = await Client.LeaveRequestsAllAsync(isLoggedInUser: true);
        var allocations = await Client.LeaveAllocationsAllAsync(isLoggedInUser: true);
        var model = new EmployeeLeaveRequestViewVM
        {
            LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(allocations),
            LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
        };
        
        return model;
    }

    public async Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest)
    {
        try
        {
            var response = new Response<Guid>();
            CreateLeaveRequestCommand createLeaveRequest = _mapper.Map<CreateLeaveRequestCommand>(leaveRequest);

            await Client.LeaveRequestsPOSTAsync(createLeaveRequest);
            return response;
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }

    public async Task<LeaveRequestVM> GetLeaveRequest(int id)
    {
        var leaveRequest = await Client.LeaveRequestsGETAsync(id);
        return _mapper.Map<LeaveRequestVM>(leaveRequest);
    }

    public Task DeleteLeaveRequest(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Guid>> ApproveLeaveRequest(int id, bool approved)
    {
        try
        {
            var response = new Response<Guid>();
            var request = new ChangeLeaveRequestApprovalCommand { Approved = approved, Id = id };
            await Client.UpdateApprovalAsync(request);
            return response;
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }

    public async Task<Response<Guid>> CancelLeaveRequest(int id)
    {
        try
        {
            var response = new Response<Guid>();
            var request = new CancelLeaveRequestCommand { Id = id };
            await Client.CancelRequestAsync(request);
            return response;
        }
        catch (ApiException ex)
        {
            return ConvertApiException<Guid>(ex);
        }
    }
} 