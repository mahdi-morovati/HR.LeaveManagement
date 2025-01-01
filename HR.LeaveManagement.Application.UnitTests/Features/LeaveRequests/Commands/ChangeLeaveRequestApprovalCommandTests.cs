using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.UpdateLeaveRequest;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class ChangeLeaveRequestApprovalCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IAppLogger<ChangeLeaveRequestApprovalCommandValidator>> _appLogger;


    public ChangeLeaveRequestApprovalCommandTests()
    {
        _appLogger = new Mock<IAppLogger<ChangeLeaveRequestApprovalCommandValidator>>();
        _appLogger.Setup(logger => logger.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()));

        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidRequest_ChangeLeaveRequestApprovalStatus()
    {
        // Arrange
        var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _appLogger.Object);

        var changeLeaveRequestApprovalStatusCommand = new ChangeLeaveRequestApprovalCommand
        {
            Id = 1,
            Approved = true
        };
        
        // Act
        var result = await handler.Handle(changeLeaveRequestApprovalStatusCommand, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(
            r => r.UpdateAsync(It.Is<Domain.LeaveRequest>(lr =>
                lr.Id == changeLeaveRequestApprovalStatusCommand.Id &&
                lr.Approved == changeLeaveRequestApprovalStatusCommand.Approved)), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = 0;
        var handler = new ChangeLeaveRequestApprovalCommandHandler(_mockRepo.Object, _appLogger.Object);
        var command = new ChangeLeaveRequestApprovalCommand
        {
            Id = invalidId,
            Approved = true
        };
        
        // Act && Assert
        
        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        
        // exception.Message.ShouldBe($"{nameof(Domain.LeaveRequest)}\" ({invalidId}) was not found.");
        exception.Message.ShouldBe($"{nameof(Domain.LeaveRequest)} ({invalidId}) was not found");
        
    }
    
}