using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Api.Middleware.Models;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Controllers;

public class LeaveRequestsControllerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly Mock<IMediator> _mockMediator;
    private readonly LeaveRequestsController _leaveRequestsController;
    private readonly IMapper _mapper;

    public LeaveRequestsControllerTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockMediator = new Mock<IMediator>();
        _leaveRequestsController = new LeaveRequestsController(_mockMediator.Object);

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveRequestProfile>();
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        
    }

    [Fact]
    public async Task Get_ShouldReturnLeaveRequests()
    {
        // Arrange
   
        var mockLeaveRequests = await _mockRepo.Object.GetAsync();

        _mockMediator.Setup(m => m.Send(It.IsAny<GetLeaveRequestListQuery>(), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<List<LeaveRequestListDto>>(mockLeaveRequests));

        // Act
        var result = await _leaveRequestsController.Get();
        
        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        
        var leaveRequests = okResult.Value as List<LeaveRequestListDto>;
        leaveRequests.ShouldNotBeNull();
        leaveRequests.Count.ShouldBe(mockLeaveRequests.Count);
        
        // Verify Mediator call
        _mockMediator.Verify(m => m.Send(It.IsAny<GetLeaveRequestListQuery>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Get_WithId_ReturnsLeaveRequest()
    {
        // Arrange
        var leaveRequestId = 1;
        var mockLeaveRequest = await _mockRepo.Object.GetByIdAsync(leaveRequestId);

        _mockMediator.Setup(m => m.Send(It.IsAny<GetLeaveRequestDetailQuery>(), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<LeaveRequestDetailsDto>(mockLeaveRequest));

        // Act
        var result = await _leaveRequestsController.Get(leaveRequestId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var leaveRequest = okResult.Value as LeaveRequestDetailsDto;
        leaveRequest.ShouldNotBeNull();
        leaveRequest.Id.ShouldBe(leaveRequestId);
        
        // Verify Mediator call
        _mockMediator.Verify(m => m.Send(It.Is<GetLeaveRequestDetailQuery>(q => q.Id == leaveRequestId), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Get_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = 100;

        _mockMediator.Setup(m => m.Send(It.Is<GetLeaveRequestDetailQuery>(q => q.Id == nonExistingId), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException(nameof(LeaveRequest), nonExistingId));

        // Act
        var result = await Should.ThrowAsync<NotFoundException>(() => _leaveRequestsController.Get(nonExistingId));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe($"LeaveRequest ({nonExistingId}) was not found");

        // Verify the mediator was called once
        _mockMediator.Verify(m => m.Send(It.Is<GetLeaveRequestDetailQuery>(q => q.Id == nonExistingId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Get_WithUnexpectedException_ReturnsIntervalServerError()
    {
        // Arrange
        var leaveReauestId = 1;
        _mockMediator.Setup(m => m.Send(It.IsAny<GetLeaveRequestDetailQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected error"));
        
        // Act
        var result = await Should.ThrowAsync<Exception>(() => _leaveRequestsController.Get(leaveReauestId));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Unexpected error");
        
        // Verify the mediator was called once
        _mockMediator.Verify(m => m.Send(It.IsAny<GetLeaveRequestDetailQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
}