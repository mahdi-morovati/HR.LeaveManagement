using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks;

public class MockLeaveAllocationRepository
{
    public static Mock<ILeaveAllocationRepository> GetMockLeaveAllocationRepository()
    {
        var leaveAllocations = new List<LeaveAllocation>
        {
            new LeaveAllocation
            {
                Id = 1,
                LeaveTypeId = 1,
                LeaveType = new LeaveType
                {
                    Id = 1,
                    DefaultDays = 10,
                    Name = "Normal Leave Type"
                },
                Period = 2023,
                EmployeeId = "1",
                NumberOfDays = 10
            },
            new LeaveAllocation
            {
                Id = 2,
                LeaveTypeId = 2,
                LeaveType = new LeaveType
                {
                    Id = 2,
                    DefaultDays = 15,
                    Name = "Sick"
                },
                Period = 2023,
                EmployeeId = "2",
                NumberOfDays = 15
            },
        };
        
        // create mock
        var mockRepo = new Mock<ILeaveAllocationRepository>();
        
        // setup mock methods
        // Setup GetLeaveAllocationWithDetails(int id)
        mockRepo.Setup(r => r.GetLeaveAllocationWithDetails(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveAllocations.FirstOrDefault(la => la.Id == id));

        // Setup GetLeaveAllocationsWithDetails()
        mockRepo.Setup(r => r.GetLeaveAllocationsWithDetails())
            .ReturnsAsync(leaveAllocations);

        // Setup GetLeaveAllocationsWithDetails(string userId)
        mockRepo.Setup(r => r.GetLeaveAllocationsWithDetails(It.IsAny<string>()))
            .ReturnsAsync((string userId) => leaveAllocations.Where(la => la.EmployeeId == userId).ToList());

        // Setup AllocationExists(string userId, int leaveTypeId, int period)
        mockRepo.Setup(r => r.AllocationExists(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((string userId, int leaveTypeId, int period) =>
                leaveAllocations.Any(la => la.EmployeeId == userId && la.LeaveTypeId == leaveTypeId && la.Period == period));

        // Setup AddAllocations(List<LeaveAllocation> allocations)
        mockRepo.Setup(r => r.AddAllocations(It.IsAny<List<LeaveAllocation>>()))
            .Callback((List<LeaveAllocation> newAllocations) =>
            {
                leaveAllocations.AddRange(newAllocations);
            })
            .Returns(Task.CompletedTask);

        // Setup GetUserAllocations(string userId, int leaveTypeId)
        mockRepo.Setup(r => r.GetUserAllocations(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((string userId, int leaveTypeId) =>
                leaveAllocations.FirstOrDefault(la => la.EmployeeId == userId && la.LeaveTypeId == leaveTypeId));
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => leaveAllocations.FirstOrDefault(la => la.Id == id));

        return mockRepo;
    }
}