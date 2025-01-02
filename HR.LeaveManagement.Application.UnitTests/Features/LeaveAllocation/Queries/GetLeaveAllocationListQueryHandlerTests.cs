using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocations;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocation.Queries;

public class GetLeaveAllocationListQueryHandlerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly IMapper _mapper;

    public GetLeaveAllocationListQueryHandlerTests()
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
    public async Task Handle_ReturnsCorrectLeaveAllocationList()
    {
        // Arrange
        var handler = new GetLeaveAllocationListQueryHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetLeaveAllocationListQuery(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<List<LeaveAllocationDto>>();
        result.Count.ShouldBe(2);
    }

}