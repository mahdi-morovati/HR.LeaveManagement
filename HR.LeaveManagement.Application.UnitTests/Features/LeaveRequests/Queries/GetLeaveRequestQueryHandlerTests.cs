using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestDetail;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestsByUser;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using Moq;
using Shouldly;
using Xunit;
using LeaveRequestListDto = HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList.LeaveRequestListDto;

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


    /// <summary>
    /// GetLeaveRequestList success
    /// </summary>
    [Fact]
    public async Task GetLeaveRequestsWithDetailsTest()
    {
        var handler = new GetLeaveRequestListQueryHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);

        result.ShouldBeOfType<List<LeaveRequestListDto>>();
        result.Count.ShouldBe(3);
    }

    /// <summary>
    /// GetLeaveRequestDetail success
    /// </summary>
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
        var result = await handler.Handle(new GetLeaveRequestDetailQuery { Id = leaveRequestId },
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(leaveRequestId);
        result.LeaveType.Name.ShouldBe(leaveRequest.LeaveType.Name);
        result.RequestingEmployeeId.ShouldBe(leaveRequest.RequestingEmployeeId);
    }

    /// <summary>
    /// GetLeaveRequestDetail not found exception
    /// </summary>
    [Fact]
    public async Task Handle_LeaveRequestNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var leaveRequestId = 10000;
        _mockRepo.Setup(r => r.GetLeaveRequestWithDetails(leaveRequestId))
            .ReturnsAsync(() => null);

        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
            {
                await handler.Handle(new GetLeaveRequestDetailQuery { Id = leaveRequestId },
                    CancellationToken.None);
            }
        );
    }
    
    /// <summary>
    /// LeaveRequestsByUser list success. (get details with RequestingEmployeeId)
    /// </summary>
    [Fact]
    public async Task Handle_LeaveRequestsByUser_Success()
    {
        // Arrange
        var requestingEmployeeId = "1";
        var handler = new GetLeaveRequestsByUserQueryHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(
            new GetLeaveRequestsByUserQuery { RequestingEmployeeId = requestingEmployeeId },
            CancellationToken.None);

        // Assert
        result.ShouldNotBeNull(); // Ensure the result is not null
        result.ShouldBeOfType<List<LeaveRequestsByUserDto>>(); // Ensure the correct type is returned
        result.Count.ShouldBe(2); // Ensure there are exactly 2 items
        result.All(r => r.RequestingEmployeeId == requestingEmployeeId).ShouldBeTrue(); // Validate all items belong to the given employee

        // Additional validation for fields
        foreach (var leaveRequest in result)
        {
            leaveRequest.RequestingEmployeeId.ShouldBe(requestingEmployeeId);
            leaveRequest.LeaveType.ShouldNotBeNull(); // Ensure LeaveType is not null
            leaveRequest.StartDate.ShouldBeLessThan(leaveRequest.EndDate); // Ensure StartDate is before EndDate
        }
    }

    
}