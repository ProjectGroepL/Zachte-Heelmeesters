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
  public DbSet<DoctorPatients> DoctorPatients {get; set;}= null!;
  public DbSet<Treatment> Treatments {get; set;}
  public DbSet<Referral> Referrals {get; set;}
  public DbSet<Appointment> Appointments {get; set;}


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

        modelBuilder.Entity<DoctorPatients>()
        .HasKey(dp => new { dp.DoctorId, dp.PatientId });
    //Many2Many relationdefinebyhand
    modelBuilder.Entity<DoctorPatients>()
        .HasOne(dp => dp.Doctor)
        .WithMany(u => u.DoctorPatients)
        .HasForeignKey(dp => dp.DoctorId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<DoctorPatients>()
        .HasOne(dp => dp.Patient)
        .WithMany()
        .HasForeignKey(dp => dp.PatientId)
        .OnDelete(DeleteBehavior.Restrict);

    // Ensure each patient can only have one doctor
    modelBuilder.Entity<DoctorPatients>()
        .HasIndex(dp => dp.PatientId)
        .IsUnique();
      // made sure that cascade wasnt being set on two of the tables
      modelBuilder.Entity<Referral>()
        .HasOne(r => r.Patient)
        .WithMany()
        .HasForeignKey(r => r.PatientId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Referral>()
        .HasOne(r => r.Doctor)
        .WithMany()
        .HasForeignKey(r => r.DoctorId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Referral>()
        .HasOne(r => r.Treatment)
        .WithMany()
        .HasForeignKey(r => r.TreatmentId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Referral>()
    .ToTable(tb => tb.HasTrigger("TR_Referrals_Expire"));

        
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
