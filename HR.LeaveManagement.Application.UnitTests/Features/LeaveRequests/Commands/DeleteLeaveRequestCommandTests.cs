using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveRequest.Commands.DeleteLeaveRequest;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;
using MediatR;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveRequests.Commands;

public class DeleteLeaveRequestCommandTests
{
    private readonly Mock<ILeaveRequestRepository> _mockRepo;

    public DeleteLeaveRequestCommandTests()
    {
        _mockRepo = MockLeaveRequestRepository.GetMockLeaveRequestRepository();
    }

    [Fact]
    public async Task Handle_ValidRequest_DeletesLeaveRequest()
    {
        // Arrange
        var handler = new DeleteLeaveRequestCommandHandler(_mockRepo.Object);

        var deleteCommand = new DeleteLeaveRequestCommand
        {
            Id = 1 // Valid LeaveRequest ID
        };

        // Act
        var result = await handler.Handle(deleteCommand, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(r => r.DeleteAsync(It.Is<Domain.LeaveRequest>(lr => lr.Id == deleteCommand.Id)), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsNotFoundException()
    {
        // Arrange
        var handler = new DeleteLeaveRequestCommandHandler(_mockRepo.Object);

        var deleteCommand = new DeleteLeaveRequestCommand
        {
            Id = 100 // Non-existent LeaveRequest ID
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<NotFoundException>(async () =>
            await handler.Handle(deleteCommand, CancellationToken.None));

        exception.Message.ShouldBe("LeaveRequest (100) was not found");
    }
}