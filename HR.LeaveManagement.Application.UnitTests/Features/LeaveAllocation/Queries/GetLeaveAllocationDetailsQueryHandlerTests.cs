using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocation.Queries;

public class GetLeaveAllocationDetailsQueryHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly IMapper _mapper;

    public GetLeaveAllocationDetailsQueryHandlerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveAllocationProfile>();
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ReturnsCorrectLeaveAllocationDetails()
    {
        // Arrange
        var leaveRequestId = 1;
        var handler = new GetLeaveAllocationDetailsQueryHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetLeaveAllocationDetailsQuery { Id = leaveRequestId },
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<LeaveAllocationDetailsDto>();
        result.Id.ShouldBe(leaveRequestId);
        result.LeaveType.ShouldNotBeNull();
        result.LeaveType.Name.ShouldNotBeEmpty();
        result.LeaveType.Name.ShouldBe("Normal Leave Type");
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException()
    {
        // Arrange
        var leaveRequestId = 100;
        var handler = new GetLeaveAllocationDetailsQueryHandler(_mockRepo.Object, _mapper);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(() => handler.Handle(new GetLeaveAllocationDetailsQuery { Id = leaveRequestId },
            CancellationToken.None));
    }
    
}