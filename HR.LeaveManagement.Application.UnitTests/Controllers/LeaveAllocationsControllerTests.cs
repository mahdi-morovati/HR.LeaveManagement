using AutoMapper;
using FluentValidation.Results;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Api.Middleware.Models;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.CreateLeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocationDetails;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Queries.GetLeaveAllocations;
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

        _mockRepo.Setup(r => r.GetByIdAsync(It.Is<int>(id => id <= 0)))
            .ReturnsAsync((LeaveAllocation?)null);
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
        _mockMediator.Verify(
            m => m.Send(It.Is<GetLeaveAllocationDetailsQuery>(q => q.Id == leaveAllocationId), CancellationToken.None),
            Times.Once);
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

    [Fact]
    public async Task Put_ValidCommand_ReturnsNoContent()
    {
        // Arrange
        var updateCommand = new UpdateLeaveAllocationCommand
        {
            Id = 1,
            LeaveTypeId = 1,
            NumberOfDays = 2,
            Period = 2025
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _leaveAllocationsController.Put(updateCommand);

        // Assert

        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();
        noContentResult.StatusCode.ShouldBe(StatusCodes.Status204NoContent);

        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Put_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var invalidCommand = new UpdateLeaveAllocationCommand
        {
            Id = 0, // Invalid ID
            LeaveTypeId = -1, // Invalid LeaveTypeId
            NumberOfDays = -1, // Invalid NumberOfDays
            Period = 0 // Invalid Period
        };

        var validationFailures = new List<ValidationFailure>
        {
            new("Id", "Id must be greater than 0"),
            new("LeaveTypeId", "LeaveTypeId must be valid"),
            new("NumberOfDays", "NumberOfDays must be greater than 0"),
            new("Period", "Period must be valid")
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None))
            .ThrowsAsync(new BadRequestException("Invalid Leave Allocation", new ValidationResult(validationFailures)));

        // Act
        // var result = await Assert.ThrowsAsync<BadRequestException>(() => _leaveAllocationsController.Put(invalidCommand));
        var result =
            await Should.ThrowAsync<BadRequestException>(() => _leaveAllocationsController.Put(invalidCommand));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Invalid Leave Allocation");
        result.ValidationErrors.ShouldNotBeNull();
        result.ValidationErrors.Count.ShouldBe(4);
        result.ValidationErrors["Id"].ShouldContain("Id must be greater than 0");
        result.ValidationErrors["LeaveTypeId"].ShouldContain("LeaveTypeId must be valid");
        result.ValidationErrors["NumberOfDays"].ShouldContain("NumberOfDays must be greater than 0");
        result.ValidationErrors["Period"].ShouldContain("Period must be valid");

        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Put_NonExistingEntity_ReturnsNotFound()
    {
        // Arrange
        var nonExistingCommand = new UpdateLeaveAllocationCommand
        {
            Id = 99, // Non-existing ID
            LeaveTypeId = 1,
            NumberOfDays = 5,
            Period = 2025
        };

        // Simulate a NotFoundException for the given command
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None))
            .ThrowsAsync(new NotFoundException(nameof(LeaveAllocation), nonExistingCommand.Id));

        // Act
        var exception =
            await Should.ThrowAsync<NotFoundException>(() => _leaveAllocationsController.Put(nonExistingCommand));

        // Assert
        exception.ShouldNotBeNull();
        exception.Message.ShouldBe($"{nameof(LeaveAllocation)} ({nonExistingCommand.Id}) was not found");

        // Verify Mediator was called once
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None), Times.Once);
    }


    [Fact]
    public async Task Put_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var validCommand = new UpdateLeaveAllocationCommand
        {
            Id = 1,
            LeaveTypeId = 1,
            NumberOfDays = 5,
            Period = 2025
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await Should.ThrowAsync<Exception>(() => _leaveAllocationsController.Put(validCommand));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Unexpected error");

        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateLeaveAllocationCommand>(), CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        // Arrange
        var validId = 1;

        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLeaveAllocationCommand>(), CancellationToken.None))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _leaveAllocationsController.Delete(validId);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();
        noContentResult.StatusCode.ShouldBe(StatusCodes.Status204NoContent);

        _mockMediator.Verify(m => m.Send(It.Is<DeleteLeaveAllocationCommand>(cmd => cmd.Id == validId), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = 99;

        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteLeaveAllocationCommand>(), CancellationToken.None))
            .ThrowsAsync(new NotFoundException(nameof(LeaveAllocation), invalidId));

        // Act
        var result = await Should.ThrowAsync<NotFoundException>(() => _leaveAllocationsController.Delete(invalidId));

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe($"{nameof(LeaveAllocation)} ({invalidId}) was not found");

        _mockMediator.Verify(m => m.Send(It.Is<DeleteLeaveAllocationCommand>(cmd => cmd.Id == invalidId), CancellationToken.None), Times.Once);
    }

}