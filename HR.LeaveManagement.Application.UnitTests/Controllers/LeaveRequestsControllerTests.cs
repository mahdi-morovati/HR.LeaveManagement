using AutoMapper;
using FluentValidation.Results;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Api.Middleware.Models;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
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

    [Fact]
    public async Task Put_ValidCommand_ReturnsNoContent()
    {
        // Arrange
        var updateCommand = new UpdateLeaveRequestCommand
        {
            Id = 1,
            LeaveTypeId = 2,
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2022, 1, 7),
            RequestComments = "Test reason"
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value);
        
        // Act
        var result = await _leaveRequestsController.Put(updateCommand);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();
        noContentResult.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
        
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Put_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var invalidCommand = new UpdateLeaveRequestCommand
        {
            Id = 0, // Invalid ID
            LeaveTypeId = -1, // Invalid LeaveTypeId
            StartDate = DateTime.Now.AddDays(5),
            EndDate = DateTime.Now.AddDays(3), // EndDate is before StartDate
            RequestComments = new string('A', 501) // Exceeds max length
        };

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(UpdateLeaveRequestCommand.Id), "Id does not exist."),
            new(nameof(UpdateLeaveRequestCommand.LeaveTypeId), "LeaveTypeId does not exist."),
            new(nameof(UpdateLeaveRequestCommand.StartDate), "StartDate must be before EndDate."),
            new(nameof(UpdateLeaveRequestCommand.EndDate), "EndDate must be after StartDate."),
            new(nameof(UpdateLeaveRequestCommand.RequestComments), "RequestComments must be fewer than 500 characters.")
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None))
            .ThrowsAsync(new BadRequestException("Invalid Leave Request", new ValidationResult(validationFailures)));

        // Act
        var result = await Should.ThrowAsync<BadRequestException>(() => _leaveRequestsController.Put(invalidCommand));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Invalid Leave Request");
        result.ValidationErrors.ShouldNotBeNull();
        result.ValidationErrors.Count.ShouldBe(5);
        result.ValidationErrors["Id"].ShouldContain("Id does not exist.");
        result.ValidationErrors["LeaveTypeId"].ShouldContain("LeaveTypeId does not exist.");
        result.ValidationErrors["StartDate"].ShouldContain("StartDate must be before EndDate.");
        result.ValidationErrors["EndDate"].ShouldContain("EndDate must be after StartDate.");
        result.ValidationErrors["RequestComments"].ShouldContain("RequestComments must be fewer than 500 characters.");

        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None), Times.Once);

    }

    [Fact]
    public async Task NonExistentRequest_ReturnsNotFound()
    {
        // Arrange
        var nonExistingCommand = new UpdateLeaveRequestCommand
        {
            Id = 100, // Non-existent Id
            LeaveTypeId = 2,
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2022, 1, 7),
            RequestComments = "Test reason"
        };
        
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None))
            .ThrowsAsync(new NotFoundException(nameof(UpdateLeaveRequestCommand), nonExistingCommand.Id));
        
        // Act
        var result = await Should.ThrowAsync<NotFoundException>(() => _leaveRequestsController.Put(nonExistingCommand));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe($"UpdateLeaveRequestCommand ({nonExistingCommand.Id}) was not found");
        
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None));
    }
    
    [Fact]
    public async Task Put_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var validCommand = new UpdateLeaveRequestCommand
        {
            Id = 1,
            LeaveTypeId = 1,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            RequestComments = "Valid request"
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await Should.ThrowAsync<Exception>(() => _leaveRequestsController.Put(validCommand));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Unexpected error");

        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveRequestCommand>(), CancellationToken.None), Times.Once);
    }
    
}