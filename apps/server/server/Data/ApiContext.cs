using Microsoft.EntityFrameworkCore;
using zhmApi.Models;
using ZhmApi.Models;

namespace ZhmApi.Data{

public class ApiContext : DbContext
{
  public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

  public DbSet<Role> Roles { get; set; } = null!;
  public DbSet<User> Users { get; set; } = null!;
  public DbSet<TwoFactorCode> TwoFactorCodes {get; set;} = null!;
  public DbSet<AuditEvent> AuditEvents { get; set; }

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
  }

  #region Seed Data

  #endregion
}}
