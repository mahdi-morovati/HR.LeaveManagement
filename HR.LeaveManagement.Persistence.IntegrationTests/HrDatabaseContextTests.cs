using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace HR.LeaveManagement.Persistence.IntegrationTests;

public class HrDatabaseContextTests
{
    private readonly HrDatabaseContext _hrDatabaseContext;

    public HrDatabaseContextTests()
    {
        var dbOptions = new DbContextOptionsBuilder<HrDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _hrDatabaseContext = new HrDatabaseContext(dbOptions);
    }

    [Fact]
    public void Save_SetDataCreated()
    {
        // Arrange
        var leaveType = new LeaveType
        {
            Id = 1,
            DeafaultDays = 10,
            Name = "Test Vacation"
        };

        // Act
        _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
        _hrDatabaseContext.SaveChangesAsync();

        // Assert
        leaveType.DateCreated.ShouldNotBeNull();
    }

    [Fact]
    public void Save_SetDateModified()
    {
        // Arrange
        var leaveType = new LeaveType
        {
            Id = 1,
            DeafaultDays = 10,
            Name = "Test Vacation"
        };

        // Act
        _hrDatabaseContext.LeaveTypes.AddAsync(leaveType);
        _hrDatabaseContext.SaveChangesAsync();

        // Assert
        leaveType.DateModified.ShouldNotBeNull();
    }
}