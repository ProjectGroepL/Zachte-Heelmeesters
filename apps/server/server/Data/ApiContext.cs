using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.BiDi.Script;
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
  public DbSet<MedicalDocument> MedicalDocuments {get; set;}
  //this is already made a table so i couldnt delete the extra s so i guess we have to do with what we got
  public DbSet<AccessRequest> AccesssRequests {get; set;}
  public DbSet<Notification> Notifications {get; set;}
  public DbSet<AppointmentReport> AppointmentReports {get; set;}
  public DbSet<AppointmentReportItem> ApontmentReportItems {get; set;}
  public DbSet<AuditTrail> AuditTrails { get; set; }
 

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

      //accesrequest specification on what to do when it gets deleted
      modelBuilder.Entity<AccessRequest>()
      .HasOne(ac => ac.Patient)
      .WithMany()
      .HasForeignKey(ac => ac.PatientId)
      .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<AccessRequest>()
      .HasOne(ac => ac.Specialist )
      .WithMany()
      .HasForeignKey(ac => ac.SpecialistId)
      .OnDelete(DeleteBehavior.NoAction);  

      modelBuilder.Entity<AccessRequest>()
      .HasOne(a => a.Appointment)
      .WithMany()
      .HasForeignKey(a => a.AppointmentId)
      .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<MedicalDocument>()
        .HasOne(md => md.Patient)
        .WithMany()
        .HasForeignKey(md => md.PatientId)
        .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<MedicalDocument>()
    .HasOne(md => md.Appointment)
    .WithMany()
    .HasForeignKey(md => md.AppointmentId)
    .IsRequired(false) // ⭐ THIS IS THE KEY LINE
    .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<MedicalDocument>()
        .HasOne(md => md.Treatment)
        .WithMany()
        .HasForeignKey(md => md.TreatmentId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.NoAction);


        // Zorgt dat EF de tabel met de extra 's' gebruikt voor de notificatie-link
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.AccessRequest)
            .WithMany()
            .HasForeignKey(n => n.AccessRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Forceer de tabelnaam zodat EF niet stiekem naar 'AccessRequests' (2 s-en) zoekt
        modelBuilder.Entity<AccessRequest>().ToTable("AccesssRequests");
      
    // enforce required fields, add sensible limits and prevent bad data
    modelBuilder.Entity<AuditTrail>(entity =>
 {
    entity.HasKey(a => a.Id);

    entity.Property(a => a.Method)
          .IsRequired()
          .HasMaxLength(10);   // GET, POST, PUT, DELETE

    entity.Property(a => a.Path)
          .IsRequired()
          .HasMaxLength(500);  // /api/resource/123

    entity.Property(a => a.IpAddress)
          .IsRequired()
          .HasMaxLength(45);   // IPv4 + IPv6

    entity.Property(a => a.UserAgent)
          .HasMaxLength(512);

    entity.Property(a => a.StatusCode)
          .IsRequired();

    entity.Property(a => a.Timestamp)
          .IsRequired();

    entity.Property(a => a.Details)
          .HasMaxLength(2000);

    // Performance index for audit queries
    entity.HasIndex(a => a.Timestamp)
          .HasDatabaseName("IX_AuditTrails_Timestamp");
 });


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

    modelBuilder.Entity<Treatment>().HasData(
      new Treatment { Id = 1, Name = "Fysiotherapie", Description = "Behandeling voor lichamelijke klachten", Instructions = "Patiënt moet comfortabele kleding dragen" },
      new Treatment { Id = 2, Name = "Psychotherapie", Description = "Behandeling voor mentale gezondheidsproblemen", Instructions = "Patiënt moet wekelijks sessies bijwonen" },
      new Treatment { Id = 3, Name = "Artroscopie", Description = "Kijkoperatie van de mensicus", Instructions = "Patiënt moet nuchter zijn" },
      new Treatment { Id = 4, Name = "Ergotherapie", Description = "Behandeling die zich richt op het verbeteren van de dagelijkse vaardigheden en zelfstandigheid van patiënten", Instructions = "Patiënt moet wekelijks sessies bijwonen" }
    );
  }

  #endregion
}
