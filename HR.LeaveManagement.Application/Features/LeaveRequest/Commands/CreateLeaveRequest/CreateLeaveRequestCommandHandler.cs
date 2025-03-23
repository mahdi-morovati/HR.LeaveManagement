using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Email;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Models.Email;
using MediatR;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Unit>
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IMapper _mapper;
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly IUserService _userService;
    private readonly ILeaveAllocationRepository _leaveAllocationRepository;
    private readonly IEmailSender _emailSender;
    public CreateLeaveRequestCommandHandler(ILeaveRequestRepository leaveRequestRepository, IMapper mapper, ILeaveTypeRepository leaveTypeRepository, IUserService userService, ILeaveAllocationRepository leaveAllocationRepository, IEmailSender emailSender)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _mapper = mapper;
        _leaveTypeRepository = leaveTypeRepository;
        _userService = userService;
        _leaveAllocationRepository = leaveAllocationRepository;
        _emailSender = emailSender;
    }
    public async Task<Unit> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // validate
        var validator = new CreateLeaveRequestCommandValidator(_leaveTypeRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (validationResult.Errors.Any())
        {
            throw new BadRequestException("Invalid Leave Request", validationResult);
        }

        // get requesting employee's id
        var employeeId = _userService.UserId;
        // check on employee's allocation
        var allocation = await _leaveAllocationRepository.GetUserAllocations(employeeId, request.LeaveTypeId);
        
        // if allocations aren't enough, return validation error with message
        if (allocation is null)
        {
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(nameof(request.LeaveTypeId),
                "You do not have any allocations for this leave type."));
            throw new BadRequestException("Invalid Leave Request", validationResult);
        }
        
        int daysRequested = (int)(request.EndDate - request.StartDate).TotalDays;
        if (daysRequested > allocation.NumberOfDays)
        {
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                nameof(request.EndDate), "You do not have enough days for this request"));
            throw new BadRequestException("Invalid Leave Request", validationResult);
        }
        
        // create leave request
        var leaveRequest = _mapper.Map<Domain.LeaveRequest>(request);
        leaveRequest.RequestingEmployeeId = employeeId;
        leaveRequest.DateRequested = DateTime.Now;
        await _leaveRequestRepository.CreateAsync(leaveRequest);
        
        // send confirmation email
        
        try
        {
            var email = new EmailMessage
            {
                To = string.Empty, /* Get email from employee record */
                Body = $"Your leave request for {request.StartDate:D} to {request.EndDate:D} " +
                       $"has been submitted successfully.",
                Subject = "Leave Request Submitted"
            };

            await _emailSender.SendEmail(email);
        }
        catch (Exception)
        {
            //// Log or handle error,
        }

        return Unit.Value;
    }
}