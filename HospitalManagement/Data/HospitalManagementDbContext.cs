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
        }


        public DbSet<HospitalManagement.Models.ActivityLog> ActivityLogs { get; set; }

        public DbSet<HospitalManagement.Models.DeviceInfo> DeviceInfos { get; set; }

        public DbSet<HospitalManagement.Models.Payment> Payments { get; set; }

        public DbSet<HospitalManagement.Models.SystemAdministrator> SystemAdministrators { get; set; }

        public DbSet<HospitalManagement.Models.UserBaseModel> Users { get; set; }

        public DbSet<HospitalManagement.Models.Booking> Bookings { get; set; }

        public DbSet<HospitalManagement.Models.Patient> Patients { get; set; }

        public DbSet<HospitalManagement.Models.Doctor> Doctors { get; set; }

        public DbSet<HospitalManagement.Models.Admission> Admissions { get; set; }

        public DbSet<HospitalManagement.Models.MedicalHistory> MedicalHistorys { get; set; }

        public DbSet<HospitalManagement.Models.Medication> Medications { get; set; }

        public DbSet<HospitalManagement.Models.MedicationInventory> MedicationInventory { get; set; }

        public DbSet<HospitalManagement.Models.MenuItem> MenuItems { get; set; }

        public DbSet<HospitalManagement.Models.MenuInventory> MenuInventory { get; set; }

        public DbSet<HospitalManagement.Models.Cart> Carts { get; set; }

        public DbSet<HospitalManagement.Models.CartItem> CartItems { get; set; }

        public DbSet<HospitalManagement.Models.Order> Orders { get; set; }

        public DbSet<HospitalManagement.Models.OrderItem> OrderItems { get; set; }
    }
}
