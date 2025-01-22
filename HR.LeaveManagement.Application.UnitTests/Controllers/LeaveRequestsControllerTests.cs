using AutoMapper;
using HR.LeaveManagement.Api.Controllers;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
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
    
    
    
}