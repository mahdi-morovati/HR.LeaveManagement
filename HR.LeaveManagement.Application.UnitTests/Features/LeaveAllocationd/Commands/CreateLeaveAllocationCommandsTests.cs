using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocationd.Commands;

public class CreateLeaveAllocationCommandsTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepository;


    public CreateLeaveAllocationCommandsTests()
    {
        _mockLeaveTypeRepository = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveAllocationProfile>();
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidLeaveRequest_CreateLeaveAllocation()
    {
        // Arrange
        var handle = new CreateLeaveAllocationCommandHandler(_mockRepo.Object, _mockLeaveTypeRepository.Object, _mapper);

        var command = new CreateLeaveAllocationCommand
        {
            LeaveTypeId = 1
        };

        // Act
        var result = await handle.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(
            r => r.CreateAsync(It.Is<Domain.LeaveAllocation>(la =>
                la.LeaveTypeId == command.LeaveTypeId)), Times.Once);
        
        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<LeaveAllocation>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidLeaveRequest_ThrowsBadRequestException()
    {
        // Arrange
        var handle = new CreateLeaveAllocationCommandHandler(_mockRepo.Object, _mockLeaveTypeRepository.Object, _mapper);

        var command = new CreateLeaveAllocationCommand
        {
            LeaveTypeId = 0
        };

        // Act
        var exception = await Should.ThrowAsync<BadRequestException>(() => handle.Handle(command, CancellationToken.None));

        // Assert
        exception.Message.ShouldBe("Invalid Leave Allocation");
        
        _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<LeaveAllocation>()), Times.Never);
        
        exception.ValidationErrors
            .Any(error => error.PropertyName == "LeaveTypeId" && error.ErrorMessage.Contains("does not exist."))
            .ShouldBeTrue();
        
        exception.ValidationErrors
            .Any(error => error.PropertyName == "LeaveTypeId" && error.ErrorMessage.Contains("'Leave Type Id' must be greater than '0'."))
            .ShouldBeTrue();

        
    }
}