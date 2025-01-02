using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CreateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class CreateLeaveRequestCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;

    public CreateLeaveRequestCommandTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveRequestProfile>();
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidLeaveRequest_CreatesLeaveRequest()    
    {
        // Arrange
        var handler = new CreateLeaveRequestCommandHandler(_mockRepo.Object, _mapper, _mockLeaveTypeRepo.Object);
        
        // Ensure the LeaveTypeId exists in the mock repository
        _mockLeaveTypeRepo.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new LeaveType
            {
                Id = 1,
                Name = "Vacation",
                DefaultDays = 10
            });
        
        var command = new CreateLeaveRequestCommand
        {
            LeaveTypeId = 1, // Valid leave type
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5),
            RequestComments = "Vacation leave request."
        };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
    
        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<LeaveRequest>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidLeaveType_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new CreateLeaveRequestCommandHandler(_mockRepo.Object, _mapper, _mockLeaveTypeRepo.Object);
        
        var command = new CreateLeaveRequestCommand
        {
            LeaveTypeId = 999, // Non-existing leave type
            StartDate = DateTime.Now.AddDays(5), // Invalid StartDate (StartDate must be before the EndDate)
            EndDate = DateTime.Now,
            RequestComments = "Vacation leave request."
        };
        
        // Act & Assert
        var exception = await Should.ThrowAsync<BadRequestException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
        
        exception.Message.ShouldBe("Invalid Leave Request");
        exception.ValidationErrors.ShouldNotBeNull(); // Ensure validation errors exist

        exception.ValidationErrors.Any(error => error.PropertyName == "StartDate" && 
                                                error.ErrorMessage.Contains("must be before")).ShouldBeTrue();
        
    }

    [Fact]
    public async Task Handle_InvalidCommandValidation_ThrowsBadRequestException()
    {
        // Arrange 
        var handler = new CreateLeaveRequestCommandHandler(_mockRepo.Object, _mapper, _mockLeaveTypeRepo.Object);
        var command = new CreateLeaveRequestCommand
        {
            LeaveTypeId = 1,
            StartDate = DateTime.Now.AddDays(5), // End date is before start date
            EndDate = DateTime.Now,
            RequestComments = null
        };
        
        // Act && Assert
        var exception = await Should.ThrowAsync<BadRequestException>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });

        exception.ValidationErrors.Any(error =>
                error.PropertyName == "RequestComments" &&
                error.ErrorMessage.Contains("is required"))
            .ShouldBeTrue();

    }
    
    
}