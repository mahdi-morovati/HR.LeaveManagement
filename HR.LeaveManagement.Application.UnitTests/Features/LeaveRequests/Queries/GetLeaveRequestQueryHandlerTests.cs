using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Logging;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Features.LeaveRequest.Queries.GetLeaveRequestList;
using HR.LeaveManagement.Application.MappingProfiles;
using HR.LeaveManagement.Application.UnitTests.Mocks;
using Moq;
using Shouldly;
using Xunit;

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


    [Fact]
    public async Task GetLeaveRequestListTest()
    {
        var handler = new GetLeaveRequestListQueryHandler(_mockRepo.Object, _mapper);

        var result = await handler.Handle(new GetLeaveRequestListQuery(), CancellationToken.None);
        
        result.ShouldBeOfType<List<LeaveRequestListDto>>();
        result.Count.ShouldBe(3);
    }
    
}