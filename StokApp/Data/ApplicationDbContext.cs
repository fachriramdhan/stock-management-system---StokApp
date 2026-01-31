using Microsoft.EntityFrameworkCore;
using StokApp.Models;

namespace StokApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Kategori> Kategoris { get; set; }
        public DbSet<Barang> Barangs { get; set; }
        public DbSet<StokBarang> StokBarangs { get; set; }
        public DbSet<BarangMasuk> BarangMasuks { get; set; }
        public DbSet<BarangKeluar> BarangKeluars { get; set; }
        public DbSet<Mismatch> Mismatches { get; set; }
        public DbSet<Tools> Tools { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi relasi
            modelBuilder.Entity<Area>()
                .HasOne(a => a.Plant)
                .WithMany(p => p.Areas)
                .HasForeignKey(a => a.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Barang>()
                .HasOne(b => b.Kategori)
                .WithMany(k => k.Barangs)
                .HasForeignKey(b => b.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StokBarang>()
                .HasOne(s => s.Barang)
                .WithOne(b => b.StokBarang)
                .HasForeignKey<StokBarang>(s => s.BarangId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data awal
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "supervisor",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("super123"),
                    Role = "Supervisor",
                    CreatedAt = DateTime.Now
                }
            );

            modelBuilder.Entity<Plant>().HasData(
                new Plant { Id = 1, Nama = "Plant Timur", Deskripsi = "Plant di area Timur" },
                new Plant { Id = 2, Nama = "Plant Barat", Deskripsi = "Plant di area Barat" },
                new Plant { Id = 3, Nama = "Plant Utara", Deskripsi = "Plant di area Utara" }
            );

            modelBuilder.Entity<Kategori>().HasData(
                new Kategori { Id = 1, Nama = "PLC", Deskripsi = "Programmable Logic Controller" },
                new Kategori { Id = 2, Nama = "Sensor", Deskripsi = "Sensor dan komponen deteksi" },
                new Kategori { Id = 3, Nama = "Kabel", Deskripsi = "Kabel power, signal, LAN" },
                new Kategori { Id = 4, Nama = "Consumable", Deskripsi = "Barang habis pakai" },
                new Kategori { Id = 5, Nama = "Tools", Deskripsi = "Peralatan kerja" }
            );
        }
    }
}