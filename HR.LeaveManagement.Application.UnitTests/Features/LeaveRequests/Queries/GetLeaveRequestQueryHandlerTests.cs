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
    /// Test retrieving a list of leave requests with details.
    /// </summary>
    [Fact]
    public async Task GetLeaveRequestsWithDetailsTest()
    {
        // Arrange
        var handler = new GetLeaveRequestListQueryHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);

        // Assert
        result.ShouldNotBeNull(); // Ensure result is not null
        result.ShouldBeOfType<List<LeaveRequestListDto>>(); // Ensure correct type is returned
        result.Count.ShouldBe(3); // Ensure all 3 mock requests are returned
    }

    /// <summary>
    /// Test retrieving leave request details successfully.
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
        var result = await handler.Handle(new GetLeaveRequestDetailQuery { Id = leaveRequestId }, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull(); // Ensure result is not null
        result.Id.ShouldBe(leaveRequestId); // Ensure correct ID is returned
        result.LeaveType.Name.ShouldBe(leaveRequest.LeaveType.Name); // Validate LeaveType
        result.RequestingEmployeeId.ShouldBe(leaveRequest.RequestingEmployeeId); // Validate RequestingEmployeeId
    }
    
    /// <summary>
    /// this is a refactor of Handle_LeaveRequestFound_ReturnsLeaveRequestDetailsDto method.
    /// in this method used the _mockRepo default data. (_mockRepo leaveRequest)
    /// </summary>
    [Fact]
    public async Task Handle_LeaveRequestFound_ReturnsLeaveRequestDetailsDto_FromMockRepoData()
    {
        // Arrange
        // ما می‌دانیم در MockLeaveRequestRepository یک LeaveRequest با Id=1 داریم
        var leaveRequestId = 1;

        // Since Mock default data is enough،
        // No need _mockRepo.setup (...) again.
        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetLeaveRequestDetailQuery { Id = leaveRequestId }, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(leaveRequestId);
        result.LeaveType.ShouldNotBeNull();
        result.LeaveType.Name.ShouldBe("Vacation");
        result.RequestingEmployeeId.ShouldBe("1");
    }


    /// <summary>
    /// Test NotFoundException for non-existing leave request.
    /// </summary>
    [Fact]
    public async Task Handle_LeaveRequestNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var leaveRequestId = 999; // Non-existing ID
        _mockRepo.Setup(r => r.GetLeaveRequestWithDetails(leaveRequestId))
            .ReturnsAsync(() => null);

        var handler = new GetLeaveRequestDetailQueryHandler(_mockRepo.Object, _mapper);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
        {
            await handler.Handle(new GetLeaveRequestDetailQuery { Id = leaveRequestId }, CancellationToken.None);
        });
    }

    /// <summary>
    /// Test retrieving leave requests by user successfully.
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
        result.ShouldNotBeNull(); // Ensure result is not null
        result.ShouldBeOfType<List<LeaveRequestsByUserDto>>(); // Ensure correct type is returned
        result.Count.ShouldBe(2); // Ensure exactly 2 requests are returned
        result.All(r => r.RequestingEmployeeId == requestingEmployeeId).ShouldBeTrue(); // Validate employee ID for all items

        // Additional field validation
        foreach (var leaveRequest in result)
        {
            leaveRequest.RequestingEmployeeId.ShouldBe(requestingEmployeeId); // Validate employee ID
            leaveRequest.LeaveType.ShouldNotBeNull(); // Ensure LeaveType is not null
            leaveRequest.StartDate.ShouldBeLessThan(leaveRequest.EndDate); // Validate date range
        }
    }

    /// <summary>
    /// Test NotFoundException when no leave requests found for a user.
    /// </summary>
    [Fact]
    public async Task Handle_LeaveRequestsByUser_NotFound_ThrowsNotFoundException()
    {
        // Arrange
        var requestingEmployeeId = "999"; // Non-existing employee ID
        _mockRepo.Setup(r => r.GetLeaveRequestsWithDetails(requestingEmployeeId))
            .ReturnsAsync(new List<LeaveRequest>()); // Return an empty list

        var handler = new GetLeaveRequestsByUserQueryHandler(_mockRepo.Object, _mapper);

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
        {
            await handler.Handle(new GetLeaveRequestsByUserQuery { RequestingEmployeeId = requestingEmployeeId },
                CancellationToken.None);
        });
    }
}
