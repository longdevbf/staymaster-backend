using HotelManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Payout> Payouts => Set<Payout>();
    public DbSet<PartnerBankAccount> PartnerBankAccounts => Set<PartnerBankAccount>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<InboxQuestion> InboxQuestions => Set<InboxQuestion>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<RoomBlock> RoomBlocks => Set<RoomBlock>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Map tên bảng ──────────────────────────────────────
        modelBuilder.Entity<User>().ToTable("nguoi_dung");
        modelBuilder.Entity<Role>().ToTable("vai_tro");
        modelBuilder.Entity<UserRole>().ToTable("nguoi_dung_vai_tro");
        modelBuilder.Entity<Room>().ToTable("phong");
        modelBuilder.Entity<Property>().ToTable("co_so_luu_tru");
        modelBuilder.Entity<RoomImage>().ToTable("anh_phong");
        modelBuilder.Entity<Booking>().ToTable("dat_phong");
        modelBuilder.Entity<Review>().ToTable("danh_gia");
        modelBuilder.Entity<Payment>().ToTable("thanh_toan");
        modelBuilder.Entity<Partner>().ToTable("doi_tac");

        // ── User ─────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.FullName).HasColumnName("ho_ten");
            e.Property(u => u.Email).HasColumnName("email");
            e.Property(u => u.Phone).HasColumnName("so_dien_thoai");
            e.Property(u => u.Cccd).HasColumnName("cccd");
            e.Property(u => u.Username).HasColumnName("ten_dang_nhap");
            e.Property(u => u.Password).HasColumnName("mat_khau");
            e.Property(u => u.Avatar).HasColumnName("anh_dai_dien");
            e.Property(u => u.Address).HasColumnName("dia_chi");
            e.Property(u => u.Status).HasColumnName("trang_thai");
            e.Property(u => u.LastActive).HasColumnName("lan_cuoi_hoat_dong");
            e.Property(u => u.CreatedAt).HasColumnName("ngay_tao");
            e.Property(u => u.UpdatedAt).HasColumnName("ngay_cap_nhat");
            e.HasIndex(u => u.Email).IsUnique();
        });

        // ── Role ─────────────────────────────────────────────
        modelBuilder.Entity<Role>(e =>
        {
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.RoleName).HasColumnName("ten_vai_tro");
        });

        // ── UserRole (junction) ───────────────────────────────
        modelBuilder.Entity<UserRole>(e =>
        {
            e.HasKey(ur => new { ur.UserId, ur.RoleId });
            e.Property(ur => ur.UserId).HasColumnName("nguoi_dung_id");
            e.Property(ur => ur.RoleId).HasColumnName("vai_tro_id");
            e.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            e.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
        });

        // ── Partner (doi_tac) ───────────────────────────────
        modelBuilder.Entity<Partner>(e =>
        {
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.UserId).HasColumnName("nguoi_dung_id");
            e.Property(p => p.PartnerCode).HasColumnName("ma_doi_tac");
            e.Property(p => p.BusinessName).HasColumnName("ten_doanh_nghiep");
            e.Property(p => p.BusinessType).HasColumnName("loai_hinh_kinh_doanh");
            e.Property(p => p.Address).HasColumnName("dia_chi");
            e.Property(p => p.Description).HasColumnName("mo_ta");
            e.Property(p => p.Status).HasColumnName("trang_thai");
            e.Property(p => p.WalletBalance).HasColumnName("so_du_vi").HasColumnType("decimal(18,2)");
            e.Property(p => p.CreatedAt).HasColumnName("ngay_tao");
            e.Property(p => p.UpdatedAt).HasColumnName("ngay_cap_nhat");
            
            e.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId);
        });

        // ── Property ─────────────────────────────────────────────
        modelBuilder.Entity<Property>(e =>
        {
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.PartnerId).HasColumnName("doi_tac_id");
            e.Property(p => p.Name).HasColumnName("ten_co_so");
            e.Property(p => p.Type).HasColumnName("loai_co_so");
            e.Property(p => p.City).HasColumnName("tinh_thanh");
            e.Property(p => p.District).HasColumnName("quan_huyen");
            e.Property(p => p.Ward).HasColumnName("phuong_xa");
            e.Property(p => p.DetailedAddress).HasColumnName("dia_chi_chi_tiet");
            e.Property(p => p.Longitude).HasColumnName("kinh_do").HasColumnType("decimal(18,8)");
            e.Property(p => p.Latitude).HasColumnName("vi_do").HasColumnType("decimal(18,8)");
            e.Property(p => p.Description).HasColumnName("mo_ta");
            e.Property(p => p.ImageUrl).HasColumnName("url_anh_dai_dien");
            e.Property(p => p.DefaultCheckInTime).HasColumnName("gio_nhan_phong_mac_dinh");
            e.Property(p => p.DefaultCheckOutTime).HasColumnName("gio_tra_phong_mac_dinh");
            e.Property(p => p.Status).HasColumnName("trang_thai");
            e.Property(p => p.AverageRating).HasColumnName("diem_trung_binh").HasColumnType("decimal(10,2)");
            e.Property(p => p.TotalReviews).HasColumnName("tong_so_danh_gia");
            e.Property(p => p.CreatedAt).HasColumnName("ngay_tao");
            e.Property(p => p.UpdatedAt).HasColumnName("ngay_cap_nhat");

            e.HasOne(p => p.Partner).WithMany(p => p.Properties).HasForeignKey(p => p.PartnerId);
        });

        // ── RoomImage ─────────────────────────────────────────────
        modelBuilder.Entity<RoomImage>(e =>
        {
            e.Property(ai => ai.Id).HasColumnName("id");
            e.Property(ai => ai.RoomId).HasColumnName("phong_id");
            e.Property(ai => ai.Url).HasColumnName("url_anh");
            e.Property(ai => ai.IsMainImage).HasColumnName("la_anh_chinh");
            e.Property(ai => ai.DisplayOrder).HasColumnName("thu_tu_hien_thi");
            e.Property(ai => ai.CreatedAt).HasColumnName("ngay_tao");
            e.HasOne(ai => ai.Room).WithMany(r => r.Images).HasForeignKey(ai => ai.RoomId);
        });

        // ── Room ─────────────────────────────────────────────
        modelBuilder.Entity<Room>(e =>
        {
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.PropertyId).HasColumnName("co_so_id");
            e.Property(r => r.Name).HasColumnName("ten_phong");
            e.Property(r => r.RoomCode).HasColumnName("ma_phong");
            e.Property(r => r.RoomType).HasColumnName("loai_phong");
            e.Property(r => r.Capacity).HasColumnName("suc_chua");
            e.Property(r => r.BedCount).HasColumnName("so_giuong");
            e.Property(r => r.BasePrice).HasColumnName("gia_co_ban").HasColumnType("decimal(18,2)");
            e.Property(r => r.Area).HasColumnName("dien_tich").HasColumnType("decimal(18,2)");
            e.Property(r => r.Description).HasColumnName("mo_ta");
            e.Property(r => r.Status).HasColumnName("trang_thai");
            e.Property(r => r.IsApproved).HasColumnName("duoc_duyet");
            e.Property(r => r.RejectReason).HasColumnName("ly_do_tu_choi").HasMaxLength(500);
            e.Property(r => r.WeekendPrice).HasColumnName("gia_cuoi_tuan").HasColumnType("decimal(18,2)");
            e.Property(r => r.CreatedAt).HasColumnName("ngay_tao");
            e.Property(r => r.UpdatedAt).HasColumnName("ngay_cap_nhat");
            e.HasOne(r => r.Property).WithMany(p => p.Rooms).HasForeignKey(r => r.PropertyId);
        });

        // ── Booking ───────────────────────────────────────────
        modelBuilder.Entity<Booking>(e =>
        {
            e.Property(b => b.Id).HasColumnName("id");
            e.Property(b => b.UserId).HasColumnName("nguoi_dung_id");
            e.Property(b => b.RoomId).HasColumnName("phong_id");
            e.Property(b => b.CheckIn).HasColumnName("ngay_nhan_phong");
            e.Property(b => b.CheckOut).HasColumnName("ngay_tra_phong");
            e.Property(b => b.Nights).HasColumnName("so_dem");
            e.Property(b => b.GuestCount).HasColumnName("so_nguoi");
            e.Property(b => b.CheckInTime).HasColumnName("gio_nhan_phong");
            e.Property(b => b.CheckOutTime).HasColumnName("gio_tra_phong");
            e.Property(b => b.PricePerNight).HasColumnName("gia_moi_dem").HasColumnType("decimal(18,2)");
            e.Property(b => b.CleaningFee).HasColumnName("phi_don_dep").HasColumnType("decimal(18,2)");
            e.Property(b => b.ServiceFee).HasColumnName("phi_dich_vu").HasColumnType("decimal(18,2)");
            e.Property(b => b.Tax).HasColumnName("thue").HasColumnType("decimal(18,2)");
            e.Property(b => b.TotalPrice).HasColumnName("tong_tien").HasColumnType("decimal(18,2)");
            e.Property(b => b.Deposit).HasColumnName("dat_coc").HasColumnType("decimal(18,2)");
            e.Property(b => b.Status).HasColumnName("trang_thai");
            e.Property(b => b.Note).HasColumnName("ghi_chu");
            e.Property(b => b.CreatedAt).HasColumnName("ngay_tao");
            e.Property(b => b.UpdatedAt).HasColumnName("ngay_cap_nhat");
            e.HasOne(b => b.User).WithMany(u => u.Bookings).HasForeignKey(b => b.UserId);
            e.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId);
        });

        // ── Review ────────────────────────────────────────────
        modelBuilder.Entity<Review>(e =>
        {
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.UserId).HasColumnName("nguoi_dung_id");
            e.Property(r => r.RoomId).HasColumnName("phong_id");
            e.Property(r => r.BookingId).HasColumnName("dat_phong_id");
            e.Property(r => r.Stars).HasColumnName("so_sao");
            e.Property(r => r.Comment).HasColumnName("binh_luan");
            e.Property(r => r.CreatedAt).HasColumnName("ngay_tao");
            e.Property(r => r.Reply).HasColumnName("phan_hoi_chu_nha");
            e.Property(r => r.ReplyAt).HasColumnName("ngay_phan_hoi").HasColumnType("datetime");
            e.Property(r => r.IsHidden).HasColumnName("bi_an").HasDefaultValue(false);
            e.Property(r => r.HiddenReason).HasColumnName("ly_do_an").HasMaxLength(500);
            e.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId);
            e.HasOne(r => r.Room).WithMany(r => r.Reviews).HasForeignKey(r => r.RoomId);
        });

        // ── Payment ───────────────────────────────────────────
        modelBuilder.Entity<Payment>(e =>
        {
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.BookingId).HasColumnName("dat_phong_id");
            e.Property(p => p.PaymentType).HasColumnName("loai_thanh_toan");
            e.Property(p => p.Amount).HasColumnName("so_tien").HasColumnType("decimal(18,2)");
            e.Property(p => p.Method).HasColumnName("phuong_thuc");
            e.Property(p => p.Status).HasColumnName("trang_thai");
            e.Property(p => p.DueDate).HasColumnName("han_thanh_toan");
            e.Property(p => p.PaidAt).HasColumnName("ngay_thanh_toan");
            e.Property(p => p.CreatedAt).HasColumnName("ngay_tao");
            e.HasOne(p => p.Booking).WithMany().HasForeignKey(p => p.BookingId);
        });

        // ── Payout (thanh_toan_doi_tac) ───────────────────────
        modelBuilder.Entity<Payout>(e =>
        {
            e.ToTable("thanh_toan_doi_tac");
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.PartnerId).HasColumnName("doi_tac_id");
            e.Property(p => p.Amount).HasColumnName("so_tien").HasColumnType("decimal(18,2)");
            e.Property(p => p.BankName).HasColumnName("ten_ngan_hang");
            e.Property(p => p.AccountNumber).HasColumnName("so_tai_khoan");
            e.Property(p => p.AccountHolder).HasColumnName("chu_tai_khoan");
            e.Property(p => p.Status).HasColumnName("trang_thai");
            e.Property(p => p.ProofImageUrl).HasColumnName("url_bang_chung");
            e.Property(p => p.TransactionId).HasColumnName("id_giao_dich");
            e.Property(p => p.AdminNote).HasColumnName("ghi_chu_admin");
            e.Property(p => p.CreatedAt).HasColumnName("ngay_tao");
            e.Property(p => p.UpdatedAt).HasColumnName("ngay_cap_nhat");
            e.Property(p => p.PaidAt).HasColumnName("ngay_thanh_toan");

            e.HasOne(p => p.Partner).WithMany().HasForeignKey(p => p.PartnerId);
        });

        // ── PartnerBankAccount (tai_khoan_ngan_hang_doi_tac) ──
        modelBuilder.Entity<PartnerBankAccount>(e =>
        {
            e.ToTable("tai_khoan_ngan_hang_doi_tac");
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.PartnerId).HasColumnName("doi_tac_id");
            e.Property(p => p.BankName).HasColumnName("ten_ngan_hang");
            e.Property(p => p.AccountNumber).HasColumnName("so_tai_khoan");
            e.Property(p => p.AccountHolder).HasColumnName("chu_tai_khoan");
            e.Property(p => p.IsPrimary).HasColumnName("la_tai_khoan_chinh");
            e.Property(p => p.CreatedAt).HasColumnName("ngay_tao");
            e.Property(p => p.UpdatedAt).HasColumnName("ngay_cap_nhat");

            e.HasOne(p => p.Partner).WithMany().HasForeignKey(p => p.PartnerId);
        });

        // ── PasswordResetToken (ma_dat_lai_mat_khau) ─────────
        modelBuilder.Entity<PasswordResetToken>(e =>
        {
            e.ToTable("ma_dat_lai_mat_khau");
            e.Property(t => t.Id).HasColumnName("id");
            e.Property(t => t.UserId).HasColumnName("nguoi_dung_id");
            e.Property(t => t.TokenHash).HasColumnName("ma_bam").HasMaxLength(64).IsRequired();
            e.Property(t => t.ExpiresAt).HasColumnName("het_han");
            e.Property(t => t.UsedAt).HasColumnName("da_dung_luc");
            e.Property(t => t.CreatedAt).HasColumnName("ngay_tao");
            e.HasIndex(t => t.TokenHash).IsUnique();
            e.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── InboxQuestion (hop_thu_cau_hoi) ──────────────────
        modelBuilder.Entity<InboxQuestion>(e =>
        {
            e.ToTable("hop_thu_cau_hoi");
            e.Property(q => q.Id).HasColumnName("id");
            e.Property(q => q.CustomerUserId).HasColumnName("khach_hang_id");
            e.Property(q => q.PartnerId).HasColumnName("doi_tac_id");
            e.Property(q => q.RoomId).HasColumnName("phong_id");
            e.Property(q => q.Question).HasColumnName("cau_hoi").HasMaxLength(2000).IsRequired();
            e.Property(q => q.Answer).HasColumnName("tra_loi").HasMaxLength(4000);
            e.Property(q => q.AnsweredByUserId).HasColumnName("nguoi_tra_loi_id");
            e.Property(q => q.Source).HasColumnName("nguon").HasMaxLength(20);
            e.Property(q => q.Status).HasColumnName("trang_thai").HasMaxLength(20);
            e.Property(q => q.CreatedAt).HasColumnName("ngay_tao");
            e.Property(q => q.AnsweredAt).HasColumnName("ngay_tra_loi");
            e.Property(q => q.LastMessageAt).HasColumnName("ngay_tin_nhan_cuoi");
            e.HasIndex(q => new { q.PartnerId, q.Status });
            e.HasOne(q => q.Customer).WithMany().HasForeignKey(q => q.CustomerUserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(q => q.Partner).WithMany().HasForeignKey(q => q.PartnerId).OnDelete(DeleteBehavior.NoAction);
            e.HasOne(q => q.Room).WithMany().HasForeignKey(q => q.RoomId).OnDelete(DeleteBehavior.NoAction);
        });

        // ── InboxMessage (hop_thu_tin_nhan) ──────────────────
        modelBuilder.Entity<InboxMessage>(e =>
        {
            e.ToTable("hop_thu_tin_nhan");
            e.Property(m => m.Id).HasColumnName("id");
            e.Property(m => m.QuestionId).HasColumnName("cau_hoi_id");
            e.Property(m => m.SenderUserId).HasColumnName("nguoi_gui_id");
            e.Property(m => m.Sender).HasColumnName("ben_gui").HasMaxLength(20);
            e.Property(m => m.Content).HasColumnName("noi_dung").HasMaxLength(4000).IsRequired();
            e.Property(m => m.CreatedAt).HasColumnName("ngay_tao");
            e.HasIndex(m => new { m.QuestionId, m.CreatedAt });
            e.HasOne(m => m.Question).WithMany(q => q.Messages).HasForeignKey(m => m.QuestionId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── AuditLog (log_he_thong) — bảng có sẵn ────────────
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("log_he_thong");
            e.Property(l => l.Id).HasColumnName("id");
            e.Property(l => l.UserId).HasColumnName("nguoi_dung_id");
            e.Property(l => l.Action).HasColumnName("hanh_dong").HasMaxLength(100).IsRequired();
            e.Property(l => l.TableName).HasColumnName("bang_tac_dong").HasMaxLength(100);
            e.Property(l => l.EntityKey).HasColumnName("khoa_chinh").HasMaxLength(100);
            e.Property(l => l.OldData).HasColumnName("du_lieu_cu");
            e.Property(l => l.NewData).HasColumnName("du_lieu_moi");
            e.Property(l => l.CreatedAt).HasColumnName("ngay_tao").HasColumnType("datetime");
        });

        // ── Amenity (tien_ich) — bảng có sẵn ─────────────────
        modelBuilder.Entity<Amenity>(e =>
        {
            e.ToTable("tien_ich");
            e.Property(a => a.Id).HasColumnName("id");
            e.Property(a => a.Name).HasColumnName("ten_tien_ich").HasMaxLength(100);
            e.Property(a => a.Group).HasColumnName("nhom_tien_ich").HasMaxLength(100);
            e.Property(a => a.Icon).HasColumnName("icon").HasMaxLength(100);
        });

        // ── RoomAmenity (phong_tien_ich) — bảng có sẵn ───────
        modelBuilder.Entity<RoomAmenity>(e =>
        {
            e.ToTable("phong_tien_ich");
            e.HasKey(ra => new { ra.RoomId, ra.AmenityId });
            e.Property(ra => ra.RoomId).HasColumnName("phong_id");
            e.Property(ra => ra.AmenityId).HasColumnName("tien_ich_id");
            e.HasOne(ra => ra.Room).WithMany(r => r.RoomAmenities).HasForeignKey(ra => ra.RoomId);
            e.HasOne(ra => ra.Amenity).WithMany(a => a.RoomAmenities).HasForeignKey(ra => ra.AmenityId);
        });
        // ── Notification (thong_bao) — bảng có sẵn, thêm cột duong_dan ─
        modelBuilder.Entity<Notification>(e =>
        {
            e.ToTable("thong_bao");
            e.Property(n => n.Id).HasColumnName("id");
            e.Property(n => n.UserId).HasColumnName("nguoi_dung_id");
            e.Property(n => n.Title).HasColumnName("tieu_de").HasMaxLength(200);
            e.Property(n => n.Content).HasColumnName("noi_dung");
            e.Property(n => n.Type).HasColumnName("loai").HasMaxLength(50);
            e.Property(n => n.IsRead).HasColumnName("da_doc");
            e.Property(n => n.CreatedAt).HasColumnName("ngay_tao").HasColumnType("datetime");
            e.Property(n => n.Link).HasColumnName("duong_dan").HasMaxLength(255);
            e.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId);
            e.HasIndex(n => new { n.UserId, n.IsRead }).HasDatabaseName("ix_thong_bao_nguoi_dung_da_doc");
        });

        // ── Favorite (yeu_thich) — bảng có sẵn ───────────────
        modelBuilder.Entity<Favorite>(e =>
        {
            e.ToTable("yeu_thich");
            e.Property(f => f.Id).HasColumnName("id");
            e.Property(f => f.UserId).HasColumnName("nguoi_dung_id");
            e.Property(f => f.RoomId).HasColumnName("phong_id");
            e.Property(f => f.CreatedAt).HasColumnName("ngay_tao").HasColumnType("datetime");
            e.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId);
            e.HasOne(f => f.Room).WithMany().HasForeignKey(f => f.RoomId);
            e.HasIndex(f => new { f.UserId, f.RoomId }).IsUnique().HasDatabaseName("uq_nguoi_dung_phong_yeu_thich");
        });

        // ── RoomBlock (ngay_chan_phong) — bảng mới ───────────
        modelBuilder.Entity<RoomBlock>(e =>
        {
            e.ToTable("ngay_chan_phong");
            e.Property(b => b.Id).HasColumnName("id");
            e.Property(b => b.RoomId).HasColumnName("phong_id");
            e.Property(b => b.FromDate).HasColumnName("tu_ngay").HasColumnType("date");
            e.Property(b => b.ToDate).HasColumnName("den_ngay").HasColumnType("date");
            e.Property(b => b.Reason).HasColumnName("ly_do").HasMaxLength(255);
            e.Property(b => b.CreatedAt).HasColumnName("ngay_tao").HasColumnType("datetime");
            e.HasOne(b => b.Room).WithMany(r => r.Blocks).HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(b => new { b.RoomId, b.FromDate });
        });

        // ── SystemSetting (cau_hinh_he_thong) — bảng mới ─────
        modelBuilder.Entity<SystemSetting>(e =>
        {
            e.ToTable("cau_hinh_he_thong");
            e.HasKey(x => x.Key);
            e.Property(x => x.Key).HasColumnName("khoa").HasMaxLength(100);
            e.Property(x => x.Value).HasColumnName("gia_tri").HasMaxLength(1000);
            e.Property(x => x.UpdatedAt).HasColumnName("ngay_cap_nhat").HasColumnType("datetime");
            e.Property(x => x.UpdatedBy).HasColumnName("nguoi_cap_nhat_id");
        });
    }
}
