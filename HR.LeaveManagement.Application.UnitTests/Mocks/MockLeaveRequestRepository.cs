using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks;

public class MockLeaveRequestRepository
{
    public static Mock<ILeaveRequestRepository> GetMockLeaveRequestRepository()
    {
        // sample data
        var leaveRequests = new List<LeaveRequest>
        {
            new LeaveRequest
            {
                Id = 1,
                RequestingEmployeeId = "1",
                LeaveTypeId = 1,
                LeaveType = new LeaveType
                {
                    Id = 1,
                    Name = "Vacation",
                    DefaultDays = 10
                },
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3)
            },
            new LeaveRequest
            {
                Id = 2,
                RequestingEmployeeId = "2",
                LeaveTypeId = 2,
                LeaveType = new LeaveType
                {
                    Id = 2,
                    Name = "Sick",
                    DefaultDays = 5
                },
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(4)
            },
            new LeaveRequest
            {
                Id = 3,
                RequestingEmployeeId = "1",
                LeaveTypeId = 3,
                LeaveType = new LeaveType
                {
                    Id = 3,
                    Name = "Maternity",
                    DefaultDays = 15
                },
                StartDate = DateTime.Now.AddDays(2),
                EndDate = DateTime.Now.AddDays(5)
            }
        };

        // create mock
        var mockRepo = new Mock<ILeaveRequestRepository>();

        // setup mock methods

        // setup GetLeaveRequestWithDetails(int id)
        mockRepo.Setup(r => r.GetLeaveRequestWithDetails(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveRequests.FirstOrDefault(lr => lr.Id == id));

        // setup GetLeaveRequestWithDetails()
        mockRepo.Setup(r => r.GetLeaveRequestsWithDetails())
            .ReturnsAsync(leaveRequests);

        // setup GetLeaveRequestsWithDetails(string userId)
        mockRepo.Setup(r => r.GetLeaveRequestsWithDetails(It.IsAny<string>()))
            .ReturnsAsync((string userId) => leaveRequests.Where(lr => lr.RequestingEmployeeId == userId).ToList());
        
        mockRepo.Setup(r => r.ExistsByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveRequests.Any(lr => lr.Id == id));
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveRequests.FirstOrDefault(lt => lt.Id == id));


        return mockRepo;
    }
}