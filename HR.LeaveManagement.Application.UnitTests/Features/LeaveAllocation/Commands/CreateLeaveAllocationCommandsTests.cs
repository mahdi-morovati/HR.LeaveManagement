using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Identity;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.Models.Identity;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocation.Commands;

public class CreateLeaveAllocationCommandsTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepository;
    private readonly Mock<IUserService> _mockUserService;


    public CreateLeaveAllocationCommandsTests()
    {
        _mockLeaveTypeRepository = MockLeaveTypeRepository.GetMockLeaveTypeRepository();
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockUserService = new Mock<IUserService>();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveAllocationProfile>();
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        
        _mockUserService.Setup(x => x.GetEmployees()).ReturnsAsync(new List<Employee>
        {
            new Employee { Id = "emp1", Email = "emp1@test.com" },
            new Employee { Id = "emp2", Email = "emp2@test.com" }
        });
    }

    [Fact]
    public async Task Handle_ValidLeaveRequest_CreateLeaveAllocation()
    {
        // Arrange
        var handle = new CreateLeaveAllocationCommandHandler(_mockRepo.Object, _mockLeaveTypeRepository.Object, _mapper, _mockUserService.Object);

        var command = new CreateLeaveAllocationCommand
        {
            LeaveTypeId = 1
        };

        // Act
        var result = await handle.Handle(command, CancellationToken.None);

        
        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(repo => repo.AddAllocations(It.IsAny<List<Domain.LeaveAllocation>>()), Times.Once);
    }
    
    [Xunit.Theory]
    [InlineData(0, "'Leave Type Id' must be greater than '0'.")]
    [InlineData(99, "Leave Type Id does not exist.")]
    public async Task Handle_InvalidLeaveRequest_ThrowsBadRequestException(int leaveTypeId, string errorMessage)
    {
        // Arrange
        var handle = new CreateLeaveAllocationCommandHandler(_mockRepo.Object, _mockLeaveTypeRepository.Object, _mapper, _mockUserService.Object);

        var command = new CreateLeaveAllocationCommand
        {
            LeaveTypeId = leaveTypeId
        };

        // Act
        var exception = await Should.ThrowAsync<BadRequestException>(() => handle.Handle(command, CancellationToken.None));

        // Assert
        exception.Message.ShouldBe("Invalid Leave Allocation");
        exception.ValidationErrors.ShouldContainKey("LeaveTypeId");
        exception.ValidationErrors["LeaveTypeId"].ShouldContain(errorMessage);

        _mockRepo.Verify(repo => repo.AddAllocations(It.IsAny<List<Domain.LeaveAllocation>>()), Times.Never);
    }
}