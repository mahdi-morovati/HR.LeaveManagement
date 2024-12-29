using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Exceptions;
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
    private static readonly string LongRequestComment = new string('x', 501);


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

    /// <summary>
    /// check invalid data contains Id, LeaveTypeId, StartDate, EndDate, RequestComments
    /// </summary>
    /// <returns></returns>
    [Xunit.Theory]
    [MemberData(nameof(UpdateLeaveRequestTestData.InvalidLeaveRequestData), MemberType = typeof(UpdateLeaveRequestTestData))]
    public async Task Handle_InvalidLeaveRequest_ThrowsBadRequestException(
        int id,
        int leaveTypeId,
        string requestComments,
        string startDate,
        string endDate,
        string errorMessage,
        string expectedExceptionMessage
    )
    {
        
        // Arrange 
        var handler = new UpdateLeaveRequestCommandHandler(_mockRepo.Object, _mockLeaveTypeRepo.Object, _mapper, _appLogger.Object);

        var updateCommand = new UpdateLeaveRequestCommand
        {
            Id = id,
            LeaveTypeId = leaveTypeId,
            StartDate = DateTime.Parse(startDate),
            EndDate = DateTime.Parse(endDate),
            RequestComments = requestComments
        };

        // Act & Assert
        var exception = await Should.ThrowAsync<BadRequestException>(()
            => handler.Handle(updateCommand, CancellationToken.None));
        
        exception.Message.ShouldBe(expectedExceptionMessage);
        exception.ValidationErrors.ShouldContain(v => v.ErrorMessage.Contains(errorMessage));

    }
    
    
    public static class UpdateLeaveRequestTestData
    {
        public static IEnumerable<object[]> InvalidLeaveRequestData =>
            new List<object[]>
            {
                new object[] { 0, 1, "Valid Comment", DateTime.Parse("2023-12-01"), DateTime.Parse("2023-12-02"), "Id does not exist.", "Invalid Leave Request" },
                new object[] { 1, 0, "Valid Comment", DateTime.Parse("2023-12-01"), DateTime.Parse("2023-12-02"), "'Leave Type Id' must be greater than '0'.", "Invalid Leave Request" },
                new object[] { 1, 10, "Valid Comment", DateTime.Parse("2023-12-01"), DateTime.Parse("2023-12-02"), "Leave Type Id does not exist.", "Invalid Leave Request" },
                new object[] { 1, 2, null, DateTime.Parse("2023-12-01"), DateTime.Parse("2023-12-02"), "Request Comments is required", "Invalid Leave Request" },
                new object[] { 1, 1, new string('x', 501), DateTime.Parse("2023-12-01"), DateTime.Parse("2023-12-02"), "Request Comments must be fewer than 500 characters", "Invalid Leave Request" },
                new object[] { 1, 1, "Valid Comment", "2023-12-02", "2023-12-01", "Start Date must be before", "Invalid Leave Request" },
                new object[] { 1, 1, "Valid Comment", "2023-12-01", "2023-12-01", "End Date must be after", "Invalid Leave Request" }
            };
    }


}