using HR.LeaveManagement.BlazorUI.Pages;

namespace HR.LeaveManagement.BlazorUI.Contracts;

public interface ILeaveAllocationService
{
    Task<Response<Guid>> CreateLeaveAllocations(int leaveTypeId);
}