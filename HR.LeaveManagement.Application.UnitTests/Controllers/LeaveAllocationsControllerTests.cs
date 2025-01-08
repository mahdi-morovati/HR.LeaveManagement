using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
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
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
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

    [Fact]
    public async Task Get_WithId_ReturnsLeaveAllocation()
    {
        // Arrange
        var leaveAllocationId = 1;
        var mockedLeaveAllocation = await _mockRepo.Object.GetByIdAsync(leaveAllocationId);
        
        _mockMediator.Setup(m =>
                m.Send(It.Is<GetLeaveAllocationDetailsQuery>(q => q.Id == leaveAllocationId), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<LeaveAllocationDetailsDto>(mockedLeaveAllocation));

        // Act
        var result = await _leaveAllocationsController.Get(leaveAllocationId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var leaveAllocation = okResult.Value as LeaveAllocationDetailsDto;
        leaveAllocation.ShouldNotBeNull();
        leaveAllocation.Id.ShouldBe(leaveAllocationId);

        // Verify Mediator call
        _mockMediator.Verify(m => m.Send(It.Is<GetLeaveAllocationDetailsQuery>(q => q.Id == leaveAllocationId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Post_ValidCommand_ReturnsCreateResult()
    {
        // Arrange
        var createCommand = new CreateLeaveAllocationCommand
        {
            LeaveTypeId = 1
        };
        _mockMediator.Setup(m => m.Send(It.IsAny<CreateLeaveAllocationCommand>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value);
        
        // Act
        var result = await _leaveAllocationsController.Post(createCommand);

        // Assert
        var createResult = result as CreatedAtActionResult;
        createResult.ShouldNotBeNull();
        createResult.StatusCode.ShouldBe(StatusCodes.Status201Created);
        createResult.ActionName.ShouldBe("Get");
        createResult.RouteValues["id"].ShouldNotBeNull();
        createResult.RouteValues["id"].ShouldBe(Unit.Value);
        
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateLeaveAllocationCommand>(), CancellationToken.None), Times.Once);

    }

}