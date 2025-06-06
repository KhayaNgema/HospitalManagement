using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Data
{
    public class HospitalManagementDbContext : IdentityDbContext
    {
        public HospitalManagementDbContext(DbContextOptions<HospitalManagementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.OwnsOne(d => d.AvailableTimings);
            });

            modelBuilder.Entity<Booking>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Booking>("Booking")
                .HasValue<X_RayAppointment>("X_RayAppointment");

            modelBuilder.Entity<MedicationPescription>()
                   .HasMany(mp => mp.PrescribedMedication)
                   .WithMany(m => m.MedicationPescriptions)
                   .UsingEntity<Dictionary<string, object>>(
                       "MedicationPescription_Medication",
                       j => j
                           .HasOne<Medication>()
                           .WithMany()
                           .HasForeignKey("MedicationId")
                           .OnDelete(DeleteBehavior.Cascade),
                       j => j
                           .HasOne<MedicationPescription>()
                           .WithMany()
                           .HasForeignKey("MedicationPescriptionId")
                           .OnDelete(DeleteBehavior.Cascade),
                       j =>
                       {
                           j.HasKey("MedicationPescriptionId", "MedicationId");
                           j.ToTable("MedicationPescription_Medication");
                       });

        }


        public DbSet<HospitalManagement.Models.ActivityLog> ActivityLogs { get; set; }

        public DbSet<HospitalManagement.Models.DeviceInfo> DeviceInfos { get; set; }

        public DbSet<HospitalManagement.Models.Payment> Payments { get; set; }

        public DbSet<HospitalManagement.Models.SystemAdministrator> SystemAdministrators { get; set; }

        public DbSet<HospitalManagement.Models.UserBaseModel> Users { get; set; }

        public DbSet<HospitalManagement.Models.Booking> Bookings { get; set; }

        public DbSet<HospitalManagement.Models.Patient> Patients { get; set; }

        public DbSet<HospitalManagement.Models.KitchenStaff> KitchenStaff { get; set; }

        public DbSet<HospitalManagement.Models.Doctor> Doctors { get; set; }

        public DbSet<HospitalManagement.Models.Admission> Admissions { get; set; }

        public DbSet<HospitalManagement.Models.MedicalHistory> MedicalHistorys { get; set; }

        public DbSet<HospitalManagement.Models.Medication> Medications { get; set; }

        public DbSet<HospitalManagement.Models.MedicationInventory> MedicationInventory { get; set; }

        public DbSet<HospitalManagement.Models.MenuItem> MenuItems { get; set; }

        public DbSet<HospitalManagement.Models.MenuInventory> MenuInventory { get; set; }

        public DbSet<HospitalManagement.Models.Cart> Carts { get; set; }

        public DbSet<HospitalManagement.Models.CartItem> CartItems { get; set; }

        public DbSet<HospitalManagement.Models.Category> Categories { get; set; }

        public DbSet<HospitalManagement.Models.Order> Orders { get; set; }

        public DbSet<HospitalManagement.Models.OrderItem> OrderItems { get; set; }

        public DbSet<HospitalManagement.Models.PatientMedicalHistory> PatientMedicalHistories { get; set; }

        public DbSet<HospitalManagement.Models.X_RayAppointment> X_RayAppointments { get; set; }

        public DbSet<HospitalManagement.Models.Pharmacist> Pharmacists { get; set; }


        public DbSet<HospitalManagement.Models.PatientBill> PatientBills { get; set; }

        public DbSet<HospitalManagement.Models.PatientBillServices> PatientBillServices { get; set; }

        public DbSet<HospitalManagement.Models.MedicationPescription> MedicationPescription { get; set; }

        public DbSet<HospitalManagement.Models.Receptionist> Receptionists { get; set; }
    }
}
