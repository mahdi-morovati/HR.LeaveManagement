using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class UpdateLeaveRequestCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepo;
    private readonly Mock<IAppLogger<UpdateLeaveRequestCommandValidator>> _appLogger;

    public UpdateLeaveRequestCommandTests()
    {
        _appLogger = new Mock<IAppLogger<UpdateLeaveRequestCommandValidator>>();
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
        _mockLeaveTypeRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveRequestProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesLeaveRequest()
    {
        // Arrange
        var handler = new UpdateLeaveRequestCommandHandler(_mockRepo.Object, _mockLeaveTypeRepo.Object, _mapper, _appLogger.Object);

        var updateCommand = new UpdateLeaveRequestCommand
        {
            Id = 1,
            LeaveTypeId = 2,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(2),
            RequestComments = "Updated leave request."

        };

        // Act
        var result = await handler.Handle(updateCommand, CancellationToken.None);
        
        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<Domain.LeaveRequest>(lr =>
        lr.Id == updateCommand.Id &&
        lr.LeaveTypeId == updateCommand.LeaveTypeId &&
        lr.StartDate == updateCommand.StartDate &&
        lr.EndDate == updateCommand.EndDate &&
        lr.RequestComments == updateCommand.RequestComments
    )), Times.Once);
        
    }
}