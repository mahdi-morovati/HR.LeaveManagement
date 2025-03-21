using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;

public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateLeaveAllocationCommand, Unit>
{
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public CreateLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository,
        ILeaveTypeRepository leaveTypeRepository, IMapper mapper, IUserService userService)
    {
        _leaveAllocationRepository = leaveAllocationRepository;
        _leaveTypeRepository = leaveTypeRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<Unit> Handle(CreateLeaveAllocationCommand request, CancellationToken cancellationToken)
    {
        // validate
        var validator = new CreateLeaveAllocationCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Leave Allocation", validationResult);
        }

        // get Leave Type for allocations
        var leaveType = await _leaveTypeRepository.GetByIdAsync(request.LeaveTypeId);

        // get employees
        var employees = await _userService.GetEmployees();

        // get period
        var period = DateTime.Now.Year;

        // assign allocations if an allocation doesn't already exist for period and leave type
        var allocations = new List<Domain.LeaveAllocation>();
        foreach (var employee in employees)
        {
            var allocationExist =
                await _leaveAllocationRepository.AllocationExists(employee.Id, request.LeaveTypeId, period);
            if (allocationExist == false)
            {
                allocations.Add(new Domain.LeaveAllocation()
                {
                    EmployeeId = employee.Id,
                    LeaveTypeId = leaveType.Id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = period
                });
            }
        }

        if (allocations.Any())
        {
            await _leaveAllocationRepository.AddAllocations(allocations);
        }

        return Unit.Value;
    }
}