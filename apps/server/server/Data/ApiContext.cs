using Microsoft.EntityFrameworkCore;
using ZhmApi.Models;

namespace ZhmApi.Data;

public class ApiContext : DbContext
{
  public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

  public DbSet<Role> Roles { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;

  #region UpdatedAt timestamp handling
  public override int SaveChanges()
  {
    UpdateTimestamps();
    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    UpdateTimestamps();
    return base.SaveChangesAsync(cancellationToken);
  }

  private void UpdateTimestamps()
  {
    var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Modified &&
                   e.Entity.GetType().GetProperty("UpdatedAt") != null);

    foreach (var entry in entries)
    {
      entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
    }
  }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Self-referencing GP relationship
        modelBuilder.Entity<User>()
            .HasOne(u => u.GeneralPractitioner)
            .WithMany(gp => gp.Patients)
            .HasForeignKey(u => u.GeneralPractitionerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Patient",
                Description = "Regular patient with access to personal medical records."
            },
            new Role
            {
                Id = 2,
                Name = "General Practitioner",
                Description = "Medical doctor responsible for patient care."
            }
        );

        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "1234567890",
                Street = "Main Street",
                HouseNumber = "12",
                PostalCode = "12345",
                City = "Sampletown",
                Province = "Province",
                Country = "Country",
                ZipCode = "12345",
                PasswordHash = "hashedpassword123",
                RoleId = 2, // GP
                TwoFactorEnabled = false,
            },
            new User
            {
                Id = 2,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                PhoneNumber = "0987654321",
                Street = "Second Street",
                HouseNumber = "34",
                PostalCode = "54321",
                City = "Examplecity",
                Province = "Province",
                Country = "Country",
                ZipCode = "54321",
                PasswordHash = "hashedpassword456",
                RoleId = 1, // Patient
                TwoFactorEnabled = false,
                GeneralPractitionerId = 1 // Assigned to GP
            }
        );
    }


    #region Seed Data

    #endregion
}
