using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class UpdateLeaveRequestCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;

    public UpdateLeaveRequestCommandTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveRequestProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }
    [Fact]
    public async Task UpdateLeaveRequest_ValidData()
    {
        // Arrange
        var handler = new UpdateLeaveRequestCommandHandler();
        // Act
        // Assert
    }
}