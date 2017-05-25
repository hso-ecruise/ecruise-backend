using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ecruise.Database.Models
{
    public partial class EcruiseContext : DbContext
    {
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<CarChargingStation> CarChargingStations { get; set; }
        public virtual DbSet<CarMaintenance> CarMaintenances { get; set; }
        public virtual DbSet<ChargingStation> ChargingStations { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerToken> CustomerTokens { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }
        public virtual DbSet<Maintenance> Maintenances { get; set; }
        public virtual DbSet<Statistic> Statistics { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }

        public EcruiseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("booking_ibfk_1");

                entity.HasIndex(e => e.InvoiceItemId)
                    .HasName("booking_ibfk_3");

                entity.HasIndex(e => e.TripId)
                    .HasName("booking_ibfk_2");

                entity.Property(e => e.BookingId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CustomerId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.TripId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.InvoiceItemId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.BookedPositionLatitude).HasColumnType("double");

                entity.Property(e => e.BookedPositionLongitude).HasColumnType("double");

                entity.Property(e => e.BookingDate).HasColumnType("datetime");

                entity.Property(e => e.PlannedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("booking_ibfk_1");

                entity.HasOne(d => d.InvoiceItem)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.InvoiceItemId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("booking_ibfk_3");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.TripId)
                    .HasConstraintName("booking_ibfk_2");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("car");

                entity.Property(e => e.CarId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Kilowatts).HasColumnType("int(3) unsigned");

                entity.Property(e => e.LastKnownPositionDate).HasColumnType("datetime");

                entity.Property(e => e.LicensePlate)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.Manufacturer)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Milage).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnType("varchar(32)");
            });

            modelBuilder.Entity<CarChargingStation>(entity =>
            {
                entity.ToTable("car_charging_station");

                entity.HasIndex(e => e.CarId)
                    .HasName("CarId");

                entity.HasIndex(e => e.ChargingStationId)
                    .HasName("ChargingStationId");

                entity.Property(e => e.CarChargingStationId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ChargeEnd).HasColumnType("datetime");

                entity.Property(e => e.ChargeStart).HasColumnType("datetime");

                entity.Property(e => e.ChargingStationId).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarChargingStation)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_charging_station_ibfk_1");

                entity.HasOne(d => d.ChargingStation)
                    .WithMany(p => p.CarChargingStation)
                    .HasForeignKey(d => d.ChargingStationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_charging_station_ibfk_2");
            });

            modelBuilder.Entity<CarMaintenance>(entity =>
            {
                entity.ToTable("car_maintenance");

                entity.HasIndex(e => e.CarId)
                    .HasName("CarId");

                entity.HasIndex(e => e.InvoiceItemId)
                    .HasName("InvoiceItemId");

                entity.HasIndex(e => e.MaintenanceId)
                    .HasName("MaintenanceId");

                entity.Property(e => e.CarMaintenanceId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CompletedDate).HasColumnType("datetime");

                entity.Property(e => e.InvoiceItemId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.MaintenanceId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.PlannedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_maintenance_ibfk_1");

                entity.HasOne(d => d.InvoiceItem)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.InvoiceItemId)
                    .HasConstraintName("car_maintenance_ibfk_3");

                entity.HasOne(d => d.Maintenance)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.MaintenanceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_maintenance_ibfk_2");
            });

            modelBuilder.Entity<ChargingStation>(entity =>
            {
                entity.ToTable("charging_station");

                entity.Property(e => e.ChargingStationId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Slots).HasColumnType("int(3) unsigned");

                entity.Property(e => e.SlotsOccupied).HasColumnType("int(3) unsigned");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("configuration");

                entity.Property(e => e.ConfigurationId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AllowNewBookings).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.CustomerId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Activated).HasColumnType("bit(1)");

                entity.Property(e => e.AddressExtraLine).HasColumnType("varchar(64)");

                entity.Property(e => e.ChipCardUid).HasColumnType("varchar(16)");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(48)");

                entity.Property(e => e.HouseNumber)
                    .IsRequired()
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(48)");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.Verified).HasColumnType("bit(1)");

                entity.Property(e => e.ZipCode).HasColumnType("int(10) unsigned");
            });

            modelBuilder.Entity<CustomerToken>(entity =>
            {
                entity.ToTable("customer_token");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("CustomerId");

                entity.Property(e => e.CustomerTokenId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerToken)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("customer_token_ibfk_1");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoice");

                entity.Property(e => e.InvoiceId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Payed).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("invoice_item");

                entity.HasIndex(e => e.InvoiceId)
                    .HasName("InvoiceId");

                entity.Property(e => e.InvoiceItemId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.InvoiceId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceItem)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("invoice_item_ibfk_1");
            });

            modelBuilder.Entity<Maintenance>(entity =>
            {
                entity.ToTable("maintenance");

                entity.Property(e => e.MaintenanceId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.AtDate).HasColumnType("datetime");

                entity.Property(e => e.AtMileage).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Spontaneously).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<Statistic>(entity =>
            {
                entity.HasKey(e => e.Date)
                    .HasName("PK_statistic");

                entity.ToTable("statistic");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Bookings).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.ToTable("trip");

                entity.HasIndex(e => e.CarId)
                    .HasName("trip_ibfk_1");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("trip_ibfk_2");

                entity.HasIndex(e => e.EndChargingStationId)
                    .HasName("trip_ibfk_4");

                entity.HasIndex(e => e.StartChargingStationId)
                    .HasName("trip_ibfk_3");

                entity.Property(e => e.TripId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.CustomerId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.EndChargingStationId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartChargingStationId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Trip)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("trip_ibfk_1");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Trip)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("trip_ibfk_2");

                entity.HasOne(d => d.EndChargingStation)
                    .WithMany(p => p.TripEndChargingStation)
                    .HasForeignKey(d => d.EndChargingStationId)
                    .HasConstraintName("trip_ibfk_4");

                entity.HasOne(d => d.StartChargingStation)
                    .WithMany(p => p.TripStartChargingStation)
                    .HasForeignKey(d => d.StartChargingStationId)
                    .HasConstraintName("trip_ibfk_3");
            });
        }
    }
}
