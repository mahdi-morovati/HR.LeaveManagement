using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Queries;

public class GetLeaveRequestQueryHandlerTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;

    public GetLeaveRequestQueryHandlerTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveRequestProfile>();
            c.AddProfile<LeaveTypeProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

    
    }


    [Fact]
    public async Task GetLeaveRequestsWithDetailsTest()
    {
        var handler = new GetLeaveRequestListQueryHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);
        
        result.ShouldBeOfType<List<LeaveRequestListDto>>();
        result.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Handle_LeaveRequestFound_ReturnsLeaveRequestDetailsDto()
    {
        // Arrange
        var leaveRequestId = 1;
        var leaveRequest = new LeaveRequest
        {
            Id = leaveRequestId,
            RequestingEmployeeId = "123",
            LeaveTypeId = 1,
            LeaveType = new LeaveType
            {
                Id = 1,
                Name = "Vacation",
                DefaultDays = 10
            },
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5)
        };
        _mockRepo.Setup(r => r.GetLeaveRequestWithDetails(leaveRequestId))
            .ReturnsAsync(leaveRequest);
        
        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);
         
        
        // Act
        var result = await handler.Handle(new GetLeaveRequestDetailQuery {Id = leaveRequestId}, CancellationToken.None);
        
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(leaveRequestId);
        result.LeaveType.Name.ShouldBe(leaveRequest.LeaveType.Name);
        result.RequestingEmployeeId.ShouldBe(leaveRequest.RequestingEmployeeId);

    }

}