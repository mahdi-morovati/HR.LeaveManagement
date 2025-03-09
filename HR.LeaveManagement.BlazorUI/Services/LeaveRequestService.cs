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
    }

    public Task<EmployeeLeaveRequestViewVM> GetUserLeaveRequests()
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> CreateLeaveRequest(LeaveRequestVM leaveRequest)
    {
        throw new NotImplementedException();
    }

    public Task<LeaveRequestVM> GetLeaveRequest(int id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLeaveRequest(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> ApproveLeaveRequest(int id, bool approved)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> CancelLeaveRequest(int id)
    {
        throw new NotImplementedException();
    }
} 