using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Models;

public partial class QlksContext : DbContext
{
    public QlksContext()
    {
    }

    public QlksContext(DbContextOptions<QlksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiHinhDat> LoaiHinhDats { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongBan> PhongBans { get; set; }

    public virtual DbSet<PhongDichVu> PhongDichVus { get; set; }

    public virtual DbSet<PhongHuy> PhongHuys { get; set; }

    public virtual DbSet<Tang> Tangs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-1ARG99I\\SQLEXPRESS;Initial Catalog=QLKS;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaPhieuThue).HasName("PK_RoomRentalDetail");

            entity.ToTable("DatPhong");

            entity.Property(e => e.MaPhieuThue).ValueGeneratedNever();
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.TinhTrang).HasMaxLength(50);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomRentalDetail_Customer");

            entity.HasOne(d => d.MaLoaiHinhDatNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaLoaiHinhDat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomRentalDetail_BookType");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomRentalDetail_Employees");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomRentalDetail_Room");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK_Services");

            entity.ToTable("DichVu");

            entity.Property(e => e.MaDichVu).ValueGeneratedNever();
            entity.Property(e => e.TenDichVu).HasMaxLength(50);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK_Payment");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TinhTrang).HasMaxLength(50);

            entity.HasOne(d => d.MaPhieuThueNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaPhieuThue)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_RoomRentalDetail");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK_Customer");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .ValueGeneratedNever()
                .HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(15)
                .HasColumnName("CCCD");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.HoTenKh)
                .HasMaxLength(50)
                .HasColumnName("HoTenKH");
            entity.Property(e => e.NamSinh).HasMaxLength(5);
            entity.Property(e => e.QuocTich).HasMaxLength(50);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<LoaiHinhDat>(entity =>
        {
            entity.HasKey(e => e.MaLoaiHinhDat).HasName("PK_BookType");

            entity.ToTable("LoaiHinhDat");

            entity.Property(e => e.MaLoaiHinhDat).ValueGeneratedNever();
            entity.Property(e => e.TenLoaiHinhDat)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.HasKey(e => e.MaLp).HasName("PK_RoomType");

            entity.ToTable("LoaiPhong");

            entity.Property(e => e.MaLp)
                .ValueGeneratedNever()
                .HasColumnName("MaLP");
            entity.Property(e => e.TenLoaiPhong)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TienNghi)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK_Employees");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .ValueGeneratedNever()
                .HasColumnName("MaNV");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.HoTenNv)
                .HasMaxLength(50)
                .HasColumnName("HoTenNV");
            entity.Property(e => e.MaPb)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaPB");
            entity.Property(e => e.Sdt)
                .HasMaxLength(12)
                .HasColumnName("SDT");

            entity.HasOne(d => d.MaPbNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaPb)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employees_Departments");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK_Room");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong).ValueGeneratedNever();
            entity.Property(e => e.MaLp).HasColumnName("MaLP");
            entity.Property(e => e.SoPhong)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.MaLpNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaLp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_RoomType");

            entity.HasOne(d => d.MaTangNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaTang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_Tang");
        });

        modelBuilder.Entity<PhongBan>(entity =>
        {
            entity.HasKey(e => e.MaPb).HasName("PK_Departments");

            entity.ToTable("PhongBan");

            entity.Property(e => e.MaPb)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaPB");
            entity.Property(e => e.TenPb)
                .HasMaxLength(50)
                .HasColumnName("TenPB");
        });

        modelBuilder.Entity<PhongDichVu>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuThue, e.MaDichVu }).HasName("PK_Room_Service");

            entity.ToTable("Phong_DichVu");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.PhongDichVus)
                .HasForeignKey(d => d.MaDichVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_DichVu_DichVu");

            entity.HasOne(d => d.MaPhieuThueNavigation).WithMany(p => p.PhongDichVus)
                .HasForeignKey(d => d.MaPhieuThue)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Phong_DichVu_DatPhong");
        });

        modelBuilder.Entity<PhongHuy>(entity =>
        {
            entity.HasKey(e => e.MaPhongHuy).HasName("PK_RoomCancelled");

            entity.ToTable("PhongHuy");

            entity.Property(e => e.MaPhongHuy)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LyDo).HasMaxLength(50);

            entity.HasOne(d => d.MaPhieuThueNavigation).WithMany(p => p.PhongHuys)
                .HasForeignKey(d => d.MaPhieuThue)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomCancelled_RoomCancelled");
        });

        modelBuilder.Entity<Tang>(entity =>
        {
            entity.HasKey(e => e.MaTang);

            entity.ToTable("Tang");

            entity.Property(e => e.MaTang).ValueGeneratedNever();
            entity.Property(e => e.TenTang).HasMaxLength(15);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uid).HasName("PK_SystemAccount");

            entity.Property(e => e.Uid)
                .ValueGeneratedNever()
                .HasColumnName("UID");
            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.MatKhau).HasMaxLength(50);
            entity.Property(e => e.TaiKhoan).HasMaxLength(50);

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SystemAccount_Employees");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
