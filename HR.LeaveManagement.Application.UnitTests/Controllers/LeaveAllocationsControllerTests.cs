using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocations;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Controllers;

public class LeaveAllocationsControllerTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly LeaveAllocationsController _leaveAllocationsController;
    private readonly IMapper _mapper;

    public LeaveAllocationsControllerTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockMediator = new Mock<IMediator>();
        _leaveAllocationsController = new LeaveAllocationsController(_mockMediator.Object);

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveAllocationProfile>();
        });
    }

    [Fact]
    public async Task Get_ShouldReturnLeaveAllocations()
    {
        // Arrange
        var mockedLeaveAllocations = await _mockRepo.Object.GetAsync();

        _mockMediator.Setup(m => m.Send(It.IsAny<GetLeaveAllocationListQuery>(), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<List<LeaveAllocationDto>>(mockedLeaveAllocations));

        // Act
        var result = await _leaveAllocationsController.Get();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var leaveAllocations = okResult.Value as List<LeaveAllocationDto>;
        leaveAllocations.ShouldNotBeNull();
        leaveAllocations.Count.ShouldBe(mockedLeaveAllocations.Count);

        // Verify Mediator call
        _mockMediator.Verify(m => m.Send(It.IsAny<GetLeaveAllocationListQuery>(), CancellationToken.None), Times.Once);
    }

}