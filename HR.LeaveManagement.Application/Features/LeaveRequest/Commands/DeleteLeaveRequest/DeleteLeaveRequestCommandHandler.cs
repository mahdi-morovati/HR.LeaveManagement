using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;

public class DeleteLeaveRequestCommandHandler
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;

    public DeleteLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
    }

    public async Task<object> Handle(DeleteLeaveRequestCommand request, CancellationToken none)
    {
        // Retrieve the entity
        var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);

        // Check if it exists
        if (leaveRequest == null)
        {
            throw new NotFoundException(nameof(LeaveRequest), request.Id);
        }

        // Delete the entity
        await _leaveRequestRepository.DeleteAsync(leaveRequest);

        return Unit.Value;
    }
}