using FluentValidation;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Shared;

namespace HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;

public class UpdateLeaveRequestCommandValidator : AbstractValidator<UpdateLeaveRequestCommand>
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    public UpdateLeaveRequestCommandValidator(ILeaveTypeRepository leaveTypeRepository, ILeaveRequestRepository leaveRequestRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _leaveRequestRepository = leaveRequestRepository;
        Include(new BaseLeaveRequestValidator(_leaveTypeRepository));
        
        RuleFor(p => p.Id)
            .GreaterThan(0)
            .MustAsync(async (id, token) => await LeaveRequestMustExist(id, token))
            .WithMessage("{PropertyName} does not exist.");
        
        RuleFor(p => p.RequestComments)
           .NotEmpty().WithMessage("{PropertyName} is required")
           .NotNull()
           .MaximumLength(500).WithMessage("{PropertyName} must be fewer than 500 characters");
        
    }

    private Task<bool> LeaveRequestMustExist(int id, CancellationToken token)
    {
        return _leaveRequestRepository.ExistsByIdAsync(id);
    }
}