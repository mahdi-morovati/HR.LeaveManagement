using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.CancelLeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.ChangeLeaveRequestApproval;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using HR.LeaveManagement.Domain;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class CancelLeaveRequestCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;
    private readonly IMapper _mapper;


    public CancelLeaveRequestCommandTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveRequestProfile>(); });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidRequest_CancelLeaveRequest()
    {
        // Arrange
        var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object);

        var cancelLeaveRequestCommand = new CancelLeaveRequestCommand { Id = 1 };
        
        // Act
        var result = await handler.Handle(cancelLeaveRequestCommand, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(
            r => r.UpdateAsync(It.Is<Domain.LeaveRequest>(lr =>
                lr.Id == cancelLeaveRequestCommand.Id &&
                lr.Cancelled == true)), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = 0;
        var handler = new CancelLeaveRequestCommandHandler(_mockRepo.Object);
        var command = new CancelLeaveRequestCommand
        {
            Id = invalidId
        };
        
        // Act && Assert
        var exception = await Should.ThrowAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        exception.Message.ShouldBe($"{nameof(LeaveRequest)} ({invalidId}) was not found");
    }
    
}