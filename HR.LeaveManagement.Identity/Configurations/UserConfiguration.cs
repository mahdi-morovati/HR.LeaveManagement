using HR.LeaveManagement.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.LeaveManagement.Identity.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(
            new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Email = "admin@localhost.com",
                NormalizedEmail = "ADMIN@LOCALHOST.COM",
                FirstName = "System",
                LastName = "Admin",
                UserName = "admin@localhost.com",
                NormalizedUserName = "ADMIN@LOCALHOST.COM",
                // PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                PasswordHash = "AQAAAAIAAYagAAAAEKzPsZdqEVbCasHFDmAjkERbLTPibDnXUQ79n7MdgMpfQFy7RPuT8VnsMpwJGpGnjg==", // ✅ Fixed hash
                SecurityStamp = "705d33d0-2ec6-48f0-9c52-9d365b916860", // Fixed value prevent error "dynamic values used in a 'HasData' call"
                ConcurrencyStamp = "11aeb9d8-fd56-4e5b-b9df-8baf2b8e7588", // Fixed value prevent error "dynamic values used in a 'HasData' call"
                EmailConfirmed = true
            },
            new ApplicationUser
            {
                Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                Email = "user@localhost.com",
                NormalizedEmail = "USER@LOCALHOST.COM",
                FirstName = "System",
                LastName = "User",
                UserName = "user@localhost.com",
                NormalizedUserName = "USER@LOCALHOST.COM",
                // PasswordHash = hasher.HashPassword(null, "P@ssword1"),
                PasswordHash = "AQAAAAIAAYagAAAAEKzPsZdqEVbCasHFDmAjkERbLTPibDnXUQ79n7MdgMpfQFy7RPuT8VnsMpwJGpGnjg==", // ✅ Fixed hash
                SecurityStamp = "54e6128c-6825-406b-987d-855ef244171a", // Fixed value prevent error "dynamic values used in a 'HasData' call"
                ConcurrencyStamp = "2c4f745e-996f-4462-ac7d-a6c921750a18", // Fixed value prevent error "dynamic values used in a 'HasData' call"
                EmailConfirmed = true
            }
        );
    }
}