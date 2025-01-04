using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
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
    
    
}