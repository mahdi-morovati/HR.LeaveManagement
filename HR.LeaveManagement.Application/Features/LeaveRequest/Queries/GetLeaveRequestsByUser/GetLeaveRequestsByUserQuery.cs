using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestsByUser;

public class GetLeaveRequestsByUserQuery : IRequest<List<LeaveRequestsByUserDto>>
{
    public string RequestingEmployeeId { get; set; } = string.Empty;
}