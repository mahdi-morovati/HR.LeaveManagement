using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(HrDatabaseContext context) : base(context)
    {
    }

    public async Task<LeaveRequest?> GetLeaveRequestWithDetails(int id)
    {
        return await Context.LeaveRequests
            .Include(q => q.LeaveType)
            .FirstOrDefaultAsync(q => q.Id == id);

    }

    public async Task<List<LeaveRequest>> GetLeaveRequestsWithDetails()
    {
        return await Context.LeaveRequests
            .Include(q => q.LeaveType)
            .ToListAsync();
    }

    public async Task<List<LeaveRequest>> GetLeaveRequestsWithDetails(string userId)
    {
        return await Context.LeaveRequests
            .Where(q => q.RequestingEmployeeId == userId)
            .Include(q => q.LeaveType)
            .ToListAsync();
    }

    public Task<bool> ExistsByIdAsync(int id)
    {
        return Context.LeaveRequests.AnyAsync(q => q.Id == id);
    }
}