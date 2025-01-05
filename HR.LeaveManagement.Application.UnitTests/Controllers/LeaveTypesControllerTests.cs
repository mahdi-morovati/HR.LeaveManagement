using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
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

public class LeaveTypesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly LeaveTypesController _leaveTypesController;
    private readonly Mock<ILeaveTypeRepository> _mockRepo;
    private readonly IMapper _mapper;

    public LeaveTypesControllerTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        _mockMediator = new Mock<IMediator>();

        _leaveTypesController = new LeaveTypesController(_mockMediator.Object);

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveTypeProfile>(); });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Get_ShouldReturnListOfLeaveTypes()
    {
        // Arrange
        var mockLeaveTypes = await _mockRepo.Object.GetAsync();

        _mockMediator.Setup(m => m.Send(It.IsAny<GetLeaveTypesQuery>(), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<List<LeaveTypeDto>>(mockLeaveTypes));

        // Act
        var result = await _leaveTypesController.Get();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);

        var leaveTypes = okResult.Value as List<LeaveTypeDto>;
        leaveTypes.ShouldNotBeNull();
        leaveTypes.Count.ShouldBe(mockLeaveTypes.Count);

        // verify Mediator call
        _mockMediator.Verify(m => m.Send(It.IsAny<GetLeaveTypesQuery>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Get_WithId_ReturnsLeaveType()
    {
        // Arrange
        var leaveTypeId = 1; // Valid ID
        var mockLeaveType = await _mockRepo.Object.GetByIdAsync(leaveTypeId);
        
        // _mockMediator.Setup(m => m.Send(It.Is<GetLeaveTypeDetailsQuery>(q => q.Id == leaveTypeId), CancellationToken.None))
        //    .ReturnsAsync(_mapper.Map<LeaveTypeDetailsDto>(mockLeaveType));
        SetupMockMediatorForLeaveType(leaveTypeId, mockLeaveType);
        
        // Act
        var result = await _leaveTypesController.Get(leaveTypeId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        
        var leaveType = okResult.Value as LeaveTypeDetailsDto;
        leaveType.ShouldNotBeNull();
        leaveType.Id.ShouldBe(leaveTypeId);
        leaveType.Name.ShouldBe(mockLeaveType.Name);
        leaveType.DefaultDays.ShouldBe(mockLeaveType.DefaultDays);
        
        // verify Mediator is called
        _mockMediator.Verify(m => m.Send(It.IsAny<GetLeaveTypeDetailsQuery>(), CancellationToken.None));
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidLeaveTypeId = 0; // Invalid ID
        
        _mockMediator.Setup(m => m.Send(It.Is<GetLeaveTypeDetailsQuery>(q => q.Id == invalidLeaveTypeId), CancellationToken.None))
           .ThrowsAsync(new NotFoundException(nameof(LeaveType), invalidLeaveTypeId));
        
        // Act
        var result = await _leaveTypesController.Get(invalidLeaveTypeId);

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.ShouldNotBeNull();
        notFoundResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        
        // verify Mediator is called
        _mockMediator.Verify(m => m.Send(It.Is<GetLeaveTypeDetailsQuery>(q => q.Id == invalidLeaveTypeId), CancellationToken.None), Times.Once);
    }
    
    private void SetupMockMediatorForLeaveType(int leaveTypeId, LeaveType leaveType)
    {
        _mockMediator.Setup(m => m.Send(It.Is<GetLeaveTypeDetailsQuery>(q => q.Id == leaveTypeId), CancellationToken.None))
            .ReturnsAsync(_mapper.Map<LeaveTypeDetailsDto>(leaveType));
    }

    [Fact]
    public async Task Post_ValidCommand_ReturnsCreatedResult()
    {
        // Arrange
        var createCommand = new CreateLeaveTypeCommand
        {
            Name = "New Leave Type",
            DefaultDays = 10
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateLeaveTypeCommand>(), CancellationToken.None))
            .ReturnsAsync(1); // Assume the created ID is 1

        // Act
        var result = await _leaveTypesController.Post(createCommand);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.ShouldNotBeNull();
        createdResult.StatusCode.ShouldBe(StatusCodes.Status201Created);
        createdResult.ActionName.ShouldBe("Get");
        // Check Value if RouteValues is not populated
        createdResult.RouteValues["id"].ShouldNotBeNull();
        createdResult.RouteValues["id"].ShouldBe(1); // Check RouteValues
        
        
    }


}