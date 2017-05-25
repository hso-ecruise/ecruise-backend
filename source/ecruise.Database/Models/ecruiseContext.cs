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
                entity.HasKey(e => e.BookingId);

                entity.ToTable("booking");


                entity.HasIndex(e => e.CustomerId)
                    .HasName("booking_CustomerId");

                entity.HasIndex(e => e.TripId)
                    .HasName("booking_TripId");

                entity.HasIndex(e => e.InvoiceItemId)
                    .HasName("booking_InvoiceItemId");

                entity.Property(e => e.BookingId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.TripId)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.InvoiceItemId)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.BookedPositionLatitude)
                    .HasColumnType("double");

                entity.Property(e => e.BookedPositionLongitude)
                    .HasColumnType("double");

                entity.Property(e => e.BookingDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.PlannedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("booking_ibfk_1");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.TripId)
                    .HasConstraintName("booking_ibfk_2");

                entity.HasOne(d => d.InvoiceItem)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.InvoiceItemId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("booking_ibfk_3");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.CarId);

                entity.ToTable("car");


                entity.Property(e => e.CarId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.LicensePlate)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.ChargingState)
                    .IsRequired()
                    .HasColumnType("enum('DISCHARGING','CHARGING','FULL')");

                entity.Property(e => e.BookingState)
                    .IsRequired()
                    .HasColumnType("enum('AVAILABLE','BOOKED','BLOCKED')");

                entity.Property(e => e.Milage)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ChargeLevel)
                    .IsRequired()
                    .HasColumnType("double");

                entity.Property(e => e.Kilowatts)
                    .IsRequired()
                    .HasColumnType("int(3) unsigned");

                entity.Property(e => e.Manufacturer)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.YearOfConstruction)
                    .IsRequired()
                    .HasColumnType("int(4)");

                entity.Property(e => e.LastKnownPositionLatitude)
                    .HasColumnType("double");

                entity.Property(e => e.LastKnownPositionLongitude)
                    .HasColumnType("double");

                entity.Property(e => e.LastKnownPositionDate)
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CarChargingStation>(entity =>
            {
                entity.HasKey(e => e.CarChargingStationId);

                entity.ToTable("car_charging_station");


                entity.HasIndex(e => e.CarId)
                    .HasName("car_charging_station_CarId");

                entity.HasIndex(e => e.ChargingStationId)
                    .HasName("car_charging_station_ChargingStationId");


                entity.Property(e => e.CarChargingStationId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ChargingStationId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ChargeEnd)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.ChargeStart)
                    .IsRequired()
                    .HasColumnType("datetime");


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
                entity.HasKey(e => e.CarId);

                entity.ToTable("car_maintenance");


                entity.HasIndex(e => e.CarId)
                    .HasName("car_maintenance_CarId");

                entity.HasIndex(e => e.MaintenanceId)
                    .HasName("car_maintenance_MaintenanceId");

                entity.HasIndex(e => e.InvoiceItemId)
                    .HasName("car_maintenance_InvoiceItemId");


                entity.Property(e => e.CarMaintenanceId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.MaintenanceId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.InvoiceItemId)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PlannedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.CompletedDate)
                    .HasColumnType("datetime");


                entity.HasOne(d => d.Car)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_maintenance_ibfk_1");

                entity.HasOne(d => d.Maintenance)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.MaintenanceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("car_maintenance_ibfk_2");

                entity.HasOne(d => d.InvoiceItem)
                    .WithMany(p => p.CarMaintenance)
                    .HasForeignKey(d => d.InvoiceItemId)
                    .HasConstraintName("car_maintenance_ibfk_3");
            });

            modelBuilder.Entity<ChargingStation>(entity =>
            {
                entity.HasKey(e => e.ChargingStationId);

                entity.ToTable("charging_station");


                entity.Property(e => e.ChargingStationId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Slots)
                    .IsRequired()
                    .HasColumnType("int(3) unsigned");

                entity.Property(e => e.SlotsOccupied)
                    .IsRequired()
                    .HasColumnType("int(3) unsigned");

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnType("double");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnType("double");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasKey(e => e.ConfigurationId);

                entity.ToTable("configuration");


                entity.Property(e => e.ConfigurationId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AllowNewBookings)
                    .IsRequired()
                    .HasColumnType("bit(1)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);

                entity.ToTable("customer");


                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.ChipCardUid)
                    .HasColumnType("varchar(16)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(48)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(48)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.HouseNumber)
                    .IsRequired()
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.AddressExtraLine)
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.Activated)
                    .IsRequired()
                    .HasColumnType("bit(1)");

                entity.Property(e => e.Verified)
                    .IsRequired()
                    .HasColumnType("bit(1)");
            });

            modelBuilder.Entity<CustomerToken>(entity =>
            {
                entity.HasKey(e => e.CustomerTokenId);

                entity.ToTable("customer_token");


                entity.HasIndex(e => e.CustomerId)
                    .HasName("customer_token_CustomerId");


                entity.Property(e => e.CustomerTokenId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('EMAIL_ACTIVATION','LOGIN')");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnType("varchar(128)");

                entity.Property(e => e.CreationDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.ExpireDate)
                    .HasColumnType("datetime");


                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerToken)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("customer_token_ibfk_1");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.InvoiceId);

                entity.ToTable("invoice");


                entity.HasIndex(e => e.CustomerId)
                    .HasName("invoice_CustomerId");


                entity.Property(e => e.InvoiceId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.TotalAmount)
                    .IsRequired()
                    .HasColumnType("double");

                entity.Property(e => e.Payed)
                    .IsRequired()
                    .HasColumnType("bit(1)");


                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("invoice_ibfk_1");
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(e => e.InvoiceItemId);

                entity.ToTable("invoice_item");


                entity.HasIndex(e => e.InvoiceId)
                    .HasName("invoice_item_InvoiceId");


                entity.Property(e => e.InvoiceItemId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.InvoiceId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('DEBIT','CREDIT')");

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnType("double");


                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceItem)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("invoice_item_ibfk_1");
            });

            modelBuilder.Entity<Maintenance>(entity =>
            {
                entity.HasKey(e => e.MaintenanceId);

                entity.ToTable("maintenance");


                entity.Property(e => e.MaintenanceId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Spontaneously)
                    .IsRequired()
                    .HasColumnType("bit(1)");

                entity.Property(e => e.AtMileage)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AtDate)
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Statistic>(entity =>
            {
                entity.HasKey(e => e.Date);

                entity.ToTable("statistic");


                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.Property(e => e.Bookings)
                    .IsRequired()
                    .HasColumnType("int(11)");

                entity.Property(e => e.AverageChargeLevel)
                    .IsRequired()
                    .HasColumnType("double");
            });

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.TripId);

                entity.ToTable("trip");


                entity.HasIndex(e => e.CarId)
                    .HasName("trip_CarId");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("trip_CustomerId");

                entity.HasIndex(e => e.StartChargingStationId)
                    .HasName("trip_StartChargingStationId");

                entity.HasIndex(e => e.EndChargingStationId)
                    .HasName("trip_EndChargingStationId");


                entity.Property(e => e.TripId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CarId)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CustomerId)
                    .IsRequired()
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.StartChargingStationId)
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.EndChargingStationId)
                    .HasColumnType("int(10) unsigned");


                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Trip)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("trip_ibfk_1");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Trip)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("trip_ibfk_2");

                entity.HasOne(d => d.StartChargingStation)
                    .WithMany(p => p.TripStartChargingStation)
                    .HasForeignKey(d => d.StartChargingStationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("trip_ibfk_3");

                entity.HasOne(d => d.EndChargingStation)
                    .WithMany(p => p.TripEndChargingStation)
                    .HasForeignKey(d => d.EndChargingStationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("trip_ibfk_4");
            });
        }
    }
}
