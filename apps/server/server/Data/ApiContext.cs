using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Models;

namespace ZhmApi.Data;

public class ApiContext : IdentityDbContext<User, Role, int>
{
  public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

  // DbSets are inherited from IdentityDbContext
  public DbSet<Token> Tokens { get; set; } = null!;
  public DbSet<TwoFactorCode> TwoFactorCodes { get; set; } = null!;

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

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configure the User-Role relationship to avoid cascade delete conflicts
    modelBuilder.Entity<User>()
        .HasOne(u => u.Role)
        .WithMany()
        .HasForeignKey(u => u.RoleId)
        .OnDelete(DeleteBehavior.Restrict);

    // Configure the Token-User relationship
    modelBuilder.Entity<Token>()
        .HasOne(t => t.User)
        .WithMany()
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Configure Token indexes for better performance
    modelBuilder.Entity<Token>()
        .HasIndex(t => new { t.Value, t.Type })
        .IsUnique();

    modelBuilder.Entity<Token>()
        .HasIndex(t => t.UserId);

    modelBuilder.Entity<Token>()
        .HasIndex(t => t.ExpiresAt);

    // Configure TwoFactorCode-User relationship
    modelBuilder.Entity<TwoFactorCode>()
        .HasOne(tfc => tfc.User)
        .WithMany()
        .HasForeignKey(tfc => tfc.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Configure TwoFactorCode indexes
    modelBuilder.Entity<TwoFactorCode>()
        .HasIndex(tfc => tfc.UserId);

    modelBuilder.Entity<TwoFactorCode>()
        .HasIndex(tfc => tfc.ExpiresAt);

    // Seed initial data
    SeedData(modelBuilder);
  }

    #region Seed Data

  private void SeedData(ModelBuilder modelBuilder)
  {
    // Seed Roles
    modelBuilder.Entity<Role>().HasData(
      new Role { Id = 1, Name = "Patient", NormalizedName = "PATIENT", Description = "Patiënt die gebruik maakt van het systeem voor medische zorg en behandelingen", ConcurrencyStamp = Guid.NewGuid().ToString() },
      new Role { Id = 2, Name = "Specialist", NormalizedName = "SPECIALIST", Description = "Medisch specialist die gespecialiseerde zorg verleent in een specifiek vakgebied", ConcurrencyStamp = Guid.NewGuid().ToString() },
      new Role { Id = 3, Name = "Huisarts", NormalizedName = "HUISARTS", Description = "Huisarts die eerste lijn zorg verleent en patiënten doorverwijst naar specialisten", ConcurrencyStamp = Guid.NewGuid().ToString() },
      new Role { Id = 4, Name = "Zorgverzekeraar", NormalizedName = "ZORGVERZEKERAAR", Description = "Medewerker van zorgverzekeraar die verantwoordelijk is voor vergoedingen en polisbeheer", ConcurrencyStamp = Guid.NewGuid().ToString() },
      new Role { Id = 5, Name = "Systeembeheerder", NormalizedName = "SYSTEEMBEHEERDER", Description = "Systeembeheerder met volledige toegang tot alle functionaliteiten en gebruikersbeheer", ConcurrencyStamp = Guid.NewGuid().ToString() },
      new Role { Id = 6, Name = "Administratie", NormalizedName = "ADMINISTRATIE", Description = "Administratief medewerker in ziekenhuis die ondersteuning biedt bij balieservice en patiëntenzorg", ConcurrencyStamp = Guid.NewGuid().ToString() }
    );
  }

  #endregion
}
