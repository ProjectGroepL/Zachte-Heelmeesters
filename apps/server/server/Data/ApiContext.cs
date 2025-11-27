using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZhmApi.Models;

namespace ZhmApi.Data;

public class ApiContext : IdentityDbContext<User, Role, int>
{
  public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

  // DbSets are inherited from IdentityDbContext
  public DbSet<Token> Tokens { get; set; }

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
