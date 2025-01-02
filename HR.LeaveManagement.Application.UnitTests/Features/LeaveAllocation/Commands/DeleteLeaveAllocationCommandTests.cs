using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.DeleteLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocation.Commands;

public class DeleteLeaveAllocationCommandTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockRepo;
    private readonly IMapper _mapper;

    public DeleteLeaveAllocationCommandTests()
    {
        _mockRepo = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveAllocationProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidLeaveAllocationId_DeletesLeaveAllocation()
    {
        // Arrange
        var handler = new DeleteLeaveAllocationCommandHandler(_mockRepo.Object, _mapper);
        var command = new DeleteLeaveAllocationCommand { Id = 1 };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        _mockRepo.Verify(r => r.DeleteAsync(It.Is<Domain.LeaveAllocation>(la => la.Id == command.Id)), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidLeaveAllocationId_ThrowsNotFoundException()
    {
        // Arrange
        var handler = new DeleteLeaveAllocationCommandHandler(_mockRepo.Object, _mapper);
        var command = new DeleteLeaveAllocationCommand { Id = 1000 }; // Non-existent ID

        // Act & Assert
        var exception =
            await Should.ThrowAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        exception.Message.ShouldContain(nameof(Domain.LeaveAllocation));
        exception.Message.ShouldContain(command.Id.ToString());
        exception.Message.ShouldBe($"LeaveAllocation ({command.Id}) was not found");
        _mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Domain.LeaveAllocation>()), Times.Never);
    }
}