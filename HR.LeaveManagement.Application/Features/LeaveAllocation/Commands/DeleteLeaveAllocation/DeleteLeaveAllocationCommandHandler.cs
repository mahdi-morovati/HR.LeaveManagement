using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;

public class DeleteLeaveAllocationCommandHandler : IRequestHandler<DeleteLeaveAllocationCommand, Unit>
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly IMapper _mapper;

    public DeleteLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository, IMapper mapper)
    {
        _leaveAllocationRepository = leaveAllocationRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteLeaveAllocationCommand request, CancellationToken none)
    {
        // Retrieve the entity
        var leaveRequest = await _leaveAllocationRepository.GetByIdAsync(request.Id);

        // Check if it exists
        if (leaveRequest == null)
        {
            throw new NotFoundException(nameof(Domain.LeaveAllocation), request.Id);
        }

        // Delete the entity
        await _leaveAllocationRepository.DeleteAsync(leaveRequest);

        return Unit.Value;
    }
}