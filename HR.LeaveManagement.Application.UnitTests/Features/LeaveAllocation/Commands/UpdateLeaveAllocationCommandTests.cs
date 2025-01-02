using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Application.Features.LeaveAllocation.Commands.UpdateLeaveAllocation;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveAllocation.Commands;

public class UpdateLeaveAllocationCommandTests
{
    private readonly Mock<ILeaveAllocationRepository> _mockLeaveAllocationRepository;
    private readonly Mock<ILeaveTypeRepository> _mockLeaveTypeRepository;
    private readonly IMapper _mapper;

    public UpdateLeaveAllocationCommandTests()
    {
        _mockLeaveAllocationRepository = MockLeaveAllocationRepository.GetMockLeaveAllocationRepository();
        _mockLeaveTypeRepository = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c => { c.AddProfile<LeaveAllocationProfile>(); });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Handle_ValidRequest_UpdatesLeaveAllocation()
    {
        // Arrange
        var handler = new UpdateLeaveAllocationCommandHandler(
            _mapper, _mockLeaveTypeRepository.Object, _mockLeaveAllocationRepository.Object);

        var updateCommand = new UpdateLeaveAllocationCommand
        {
            Id = 1,
            NumberOfDays = 15,
            LeaveTypeId = 1,
            Period = DateTime.Now.Year
        };

        // Act
        var result = await handler.Handle(updateCommand, CancellationToken.None);

        // Assert
        result.ShouldBe(Unit.Value);
        _mockLeaveAllocationRepository.Verify(r =>
            r.UpdateAsync(It.Is<Domain.LeaveAllocation>(la =>
                la.Id == updateCommand.Id &&
                la.NumberOfDays == updateCommand.NumberOfDays &&
                la.LeaveTypeId == updateCommand.LeaveTypeId &&
                la.Period == updateCommand.Period)), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsBadRequestException()
    {
        // Arrange
        var handler = new UpdateLeaveAllocationCommandHandler(
            _mapper, _mockLeaveTypeRepository.Object, _mockLeaveAllocationRepository.Object);

        var invalidCommand = new UpdateLeaveAllocationCommand
        {
            Id = 1,
            NumberOfDays = 0, // Invalid number of days
            LeaveTypeId = 1,
            Period = DateTime.Now.Year
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<BadRequestException>(()
            => handler.Handle(invalidCommand, CancellationToken.None));
        exception.Message.ShouldBe("Invalid Leave Allocation");
        exception.ValidationErrors
            .Any(v => v.PropertyName == "NumberOfDays" && v.ErrorMessage.Contains("must greater than 0"))
            .ShouldBeTrue();
    }


}