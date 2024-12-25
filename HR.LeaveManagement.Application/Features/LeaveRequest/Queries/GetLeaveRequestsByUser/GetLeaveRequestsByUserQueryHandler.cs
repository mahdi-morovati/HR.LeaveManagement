using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestsByUser;

public class GetLeaveRequestsByUserQueryHandler : IRequestHandler<GetLeaveRequestsByUserQuery, List<LeaveRequestsByUserDto>>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IMapper _mapper;

    public GetLeaveRequestsByUserQueryHandler(ILeaveRequestRepository leaveRequestRepository, IMapper mapper)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _mapper = mapper;
    }

    public async Task<List<LeaveRequestsByUserDto>> Handle(GetLeaveRequestsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var leaveRequests =
            _mapper.Map<List<LeaveRequestsByUserDto>>(
                await _leaveRequestRepository.GetLeaveRequestsWithDetails(request.RequestingEmployeeId));

        if (leaveRequests.Count == 0)
        {
            throw new NotFoundException(nameof(Domain.LeaveRequest), request.RequestingEmployeeId);
        }

        return leaveRequests;
    }
}