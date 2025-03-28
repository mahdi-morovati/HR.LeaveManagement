using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveType.Commands.CreateLeaveType;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Application.UnitTests.Features.LeaveTypes.Commands;

public class CreateLeaveTypeCommandTests
{
    private Mock<ILeaveTypeRepository> _mockRepo;
    private readonly IMapper _mapper;

    public CreateLeaveTypeCommandTests()
    {
        _mockRepo = MockLeaveTypeRepository.GetMockLeaveTypeRepository();

        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<LeaveTypeProfile>();
        });

        _mapper = mapperConfig.CreateMapper();
    }
    
    [Fact]
    public async Task Handle_ValidLeaveType()
    {
        var handler = new CreateLeaveTypeCommandHandler(_mapper, _mockRepo.Object);

        await handler.Handle(new CreateLeaveTypeCommand() { Name = "Test1", DefaultDays = 1
        }, CancellationToken.None);

        var leaveTypes = await _mockRepo.Object.GetAsync();
        leaveTypes.Count.ShouldBe(4);
    }
    
}