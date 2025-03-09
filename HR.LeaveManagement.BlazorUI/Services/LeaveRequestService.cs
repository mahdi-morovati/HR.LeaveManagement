using AutoMapper;
using Blazored.LocalStorage;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.LeaveManagement.BlazorUI.Contracts;
using HR.LeaveManagement.BlazorUI.Models.LeaveAllocations;
using HR.LeaveManagement.BlazorUI.Models.LeaveRequests;
using HR.LeaveManagement.BlazorUI.Pages;
using HR.LeaveManagement.BlazorUI.Services.Base;

namespace HR.LeaveManagement.BlazorUI.Services;

public class LeaveRequestService : BaseHttpService, ILeaveRequestService
{
    public LeaveRequestService(IClient client, ILocalStorageService localStorageService) : base(client, localStorageService)
    {
    }

    public Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
    {
        throw new NotImplementedException();
        // var leaveRequests = await Client.LeaveRequestsAllAsync(isLoggedInUser: false);
        //
        // var model = new AdminLeaveRequestViewVM
        // {
        //     TotalRequests = leaveRequests.Count,
        //     ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
        //     PendingRequests = leaveRequests.Count(q => q.Approved == null),
        //     RejectedRequests = leaveRequests.Count(q => q.Approved == false),
        //     LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
        // };
        // return model;
    }

    public Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
    {
        throw new NotImplementedException();
        // var leaveRequests = await Client.LeaveRequestsAllAsync(isLoggedInUser: true);
        // var allocations = await Client.LeaveAllocationsAllAsync(isLoggedInUser: true);
        // var model = new EmployeeLeaveRequestViewVM
        // {
        //     LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(allocations),
        //     LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
        // };
        //
        // return model;
    }

    public Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest)
    {
        throw new NotImplementedException();
        // try
        // {
        //     var response = new Response<Guid>();
        //     CreateLeaveRequestCommand createLeaveRequest = _mapper.Map<CreateLeaveRequestCommand>(leaveRequest);
        //
        //     await Client.LeaveRequestsPOSTAsync(createLeaveRequest);
        //     return response;
        // }
        // catch (ApiException ex)
        // {
        //     return ConvertApiException<Guid>(ex);
        // }
    }

    public Task<LeaveRequestVM> GetLeaveRequest(int id)
    {
        throw new NotImplementedException();
        // var leaveRequest = await Client.LeaveRequestsGETAsync(id);
        // return _mapper.Map<LeaveRequestVM>(leaveRequest);
    }

    public Task DeleteLeaveRequest(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> ApproveLeaveRequest(int id, bool approved)
    {
        throw new NotImplementedException();
        // try
        // {
        //     var response = new Response<Guid>();
        //     var request = new ChangeLeaveRequestApprovalCommand { Approved = approved, Id = id };
        //     await Client.UpdateApprovalAsync(request);
        //     return response;
        // }
        // catch (ApiException ex)
        // {
        //     return ConvertApiException<Guid>(ex);
        // }
    }

    public Task<Response<Guid>> CancelLeaveRequest(int id)
    {
        throw new NotImplementedException();
        // try
        // {
        //     var response = new Response<Guid>();
        //     var request = new CancelLeaveRequestCommand { Id = id };
        //     await Client.CancelRequestAsync(request);
        //     return response;
        // }
        // catch (ApiException ex)
        // {
        //     return ConvertApiException<Guid>(ex);
        // }
    }
} 