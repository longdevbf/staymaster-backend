USE [master]
GO
/****** Object:  Database [QL_NhaNghi]    Script Date: 4/7/2026 3:20:35 PM ******/
-- Đường dẫn file .mdf/.ldf để SQL Server tự chọn, chạy được cả trên Docker/Linux
CREATE DATABASE [QL_NhaNghi]
GO
ALTER DATABASE [QL_NhaNghi] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [QL_NhaNghi].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [QL_NhaNghi] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET ARITHABORT OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [QL_NhaNghi] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [QL_NhaNghi] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET  DISABLE_BROKER 
GO
ALTER DATABASE [QL_NhaNghi] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [QL_NhaNghi] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [QL_NhaNghi] SET  MULTI_USER 
GO
ALTER DATABASE [QL_NhaNghi] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [QL_NhaNghi] SET DB_CHAINING OFF 
GO
ALTER DATABASE [QL_NhaNghi] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [QL_NhaNghi] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [QL_NhaNghi] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [QL_NhaNghi] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [QL_NhaNghi] SET QUERY_STORE = ON
GO
ALTER DATABASE [QL_NhaNghi] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [QL_NhaNghi]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[anh_co_so]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[anh_co_so](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[co_so_id] [int] NOT NULL,
	[url_anh] [nvarchar](max) NOT NULL,
	[la_anh_chinh] [bit] NULL,
	[thu_tu_hien_thi] [int] NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[anh_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[anh_phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[phong_id] [int] NOT NULL,
	[url_anh] [nvarchar](max) NOT NULL,
	[la_anh_chinh] [bit] NULL,
	[thu_tu_hien_thi] [int] NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[co_so_luu_tru]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[co_so_luu_tru](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[doi_tac_id] [int] NOT NULL,
	[ten_co_so] [nvarchar](150) NOT NULL,
	[loai_co_so] [nvarchar](100) NULL,
	[tinh_thanh] [nvarchar](100) NULL,
	[quan_huyen] [nvarchar](100) NULL,
	[phuong_xa] [nvarchar](100) NULL,
	[dia_chi_chi_tiet] [nvarchar](255) NULL,
	[kinh_do] [decimal](10, 6) NULL,
	[vi_do] [decimal](10, 6) NULL,
	[mo_ta] [nvarchar](max) NULL,
	[gio_nhan_phong_mac_dinh] [time](7) NULL,
	[gio_tra_phong_mac_dinh] [time](7) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[diem_trung_binh] [decimal](3, 2) NULL,
	[tong_so_danh_gia] [int] NULL,
	[ngay_tao] [datetime] NULL,
	[ngay_cap_nhat] [datetime] NULL,
	[url_anh_dai_dien] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[danh_gia]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[danh_gia](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[phong_id] [int] NOT NULL,
	[dat_phong_id] [int] NULL,
	[so_sao] [int] NOT NULL,
	[binh_luan] [nvarchar](max) NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[dat_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dat_phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[phong_id] [int] NOT NULL,
	[ngay_nhan_phong] [datetime] NOT NULL,
	[ngay_tra_phong] [datetime] NOT NULL,
	[so_dem] [int] NOT NULL,
	[so_nguoi] [int] NOT NULL,
	[gio_nhan_phong] [time](7) NULL,
	[gio_tra_phong] [time](7) NULL,
	[gia_moi_dem] [decimal](18, 2) NOT NULL,
	[phi_don_dep] [decimal](18, 2) NULL,
	[phi_dich_vu] [decimal](18, 2) NULL,
	[thue] [decimal](18, 2) NULL,
	[dat_coc] [decimal](18, 2) NULL,
	[tong_tien] [decimal](18, 2) NOT NULL,
	[trang_thai] [nvarchar](50) NULL,
	[ghi_chu] [nvarchar](max) NULL,
	[ngay_tao] [datetime] NULL,
	[ngay_cap_nhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[doi_tac]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[doi_tac](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[ma_doi_tac] [nvarchar](50) NULL,
	[ten_doanh_nghiep] [nvarchar](150) NOT NULL,
	[loai_hinh_kinh_doanh] [nvarchar](100) NULL,
	[dia_chi] [nvarchar](255) NULL,
	[mo_ta] [nvarchar](max) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[ngay_tao] [datetime] NULL,
	[ngay_cap_nhat] [datetime] NULL,
	[so_du_vi] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[duyet_bai_dang]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[duyet_bai_dang](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[phong_id] [int] NOT NULL,
	[admin_id] [int] NOT NULL,
	[trang_thai] [nvarchar](50) NOT NULL,
	[ghi_chu] [nvarchar](max) NULL,
	[ngay_duyet] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[duyet_dat_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[duyet_dat_phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[dat_phong_id] [int] NOT NULL,
	[admin_id] [int] NOT NULL,
	[trang_thai] [nvarchar](50) NOT NULL,
	[ghi_chu] [nvarchar](max) NULL,
	[ngay_duyet] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[giao_dich_thanh_toan]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[giao_dich_thanh_toan](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[thanh_toan_id] [int] NOT NULL,
	[cong_thanh_toan] [nvarchar](50) NULL,
	[ma_giao_dich] [nvarchar](100) NULL,
	[ma_tham_chieu] [nvarchar](100) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[noi_dung_phan_hoi] [nvarchar](max) NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[khach_dat_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[khach_dat_phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[dat_phong_id] [int] NOT NULL,
	[ho_ten] [nvarchar](100) NOT NULL,
	[so_dien_thoai] [nvarchar](15) NULL,
	[cccd] [nvarchar](20) NULL,
	[vai_tro] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[lich_su_trang_thai_dat_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lich_su_trang_thai_dat_phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[dat_phong_id] [int] NOT NULL,
	[trang_thai_cu] [nvarchar](50) NULL,
	[trang_thai_moi] [nvarchar](50) NOT NULL,
	[nguoi_thay_doi_id] [int] NULL,
	[ghi_chu] [nvarchar](max) NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[log_he_thong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[log_he_thong](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NULL,
	[hanh_dong] [nvarchar](100) NOT NULL,
	[bang_tac_dong] [nvarchar](100) NULL,
	[khoa_chinh] [nvarchar](100) NULL,
	[du_lieu_cu] [nvarchar](max) NULL,
	[du_lieu_moi] [nvarchar](max) NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[nguoi_dung]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[nguoi_dung](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ho_ten] [nvarchar](100) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[so_dien_thoai] [nvarchar](15) NULL,
	[cccd] [nvarchar](20) NULL,
	[ten_dang_nhap] [nvarchar](50) NOT NULL,
	[mat_khau] [nvarchar](255) NOT NULL,
	[anh_dai_dien] [nvarchar](max) NULL,
	[dia_chi] [nvarchar](255) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[lan_cuoi_hoat_dong] [datetime] NULL,
	[ngay_tao] [datetime] NULL,
	[ngay_cap_nhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[nguoi_dung_vai_tro]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[nguoi_dung_vai_tro](
	[nguoi_dung_id] [int] NOT NULL,
	[vai_tro_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[nguoi_dung_id] ASC,
	[vai_tro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phien_dang_nhap]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phien_dang_nhap](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[token] [nvarchar](max) NOT NULL,
	[thiet_bi] [nvarchar](200) NULL,
	[dia_chi_ip] [nvarchar](50) NULL,
	[het_han_luc] [datetime] NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phong](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[co_so_id] [int] NOT NULL,
	[ten_phong] [nvarchar](100) NOT NULL,
	[ma_phong] [nvarchar](50) NOT NULL,
	[loai_phong] [nvarchar](100) NULL,
	[suc_chua] [int] NOT NULL,
	[so_giuong] [int] NULL,
	[gia_co_ban] [decimal](18, 2) NOT NULL,
	[dien_tich] [decimal](10, 2) NULL,
	[mo_ta] [nvarchar](max) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[duoc_duyet] [bit] NULL,
	[ngay_tao] [datetime] NULL,
	[ngay_cap_nhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[phong_tien_ich]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[phong_tien_ich](
	[phong_id] [int] NOT NULL,
	[tien_ich_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[phong_id] ASC,
	[tien_ich_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tai_khoan_ngan_hang_doi_tac]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tai_khoan_ngan_hang_doi_tac](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[doi_tac_id] [int] NOT NULL,
	[ten_ngan_hang] [nvarchar](max) NOT NULL,
	[so_tai_khoan] [nvarchar](max) NOT NULL,
	[chu_tai_khoan] [nvarchar](max) NOT NULL,
	[la_tai_khoan_chinh] [bit] NOT NULL,
	[ngay_tao] [datetime2](7) NOT NULL,
	[ngay_cap_nhat] [datetime2](7) NULL,
 CONSTRAINT [PK_tai_khoan_ngan_hang_doi_tac] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tien_ich]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tien_ich](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten_tien_ich] [nvarchar](100) NOT NULL,
	[nhom_tien_ich] [nvarchar](100) NULL,
	[icon] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[thanh_toan]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[thanh_toan](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[dat_phong_id] [int] NOT NULL,
	[loai_thanh_toan] [nvarchar](50) NULL,
	[so_tien] [decimal](18, 2) NOT NULL,
	[phuong_thuc] [nvarchar](50) NOT NULL,
	[trang_thai] [nvarchar](50) NULL,
	[han_thanh_toan] [datetime] NULL,
	[ngay_thanh_toan] [datetime] NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[thanh_toan_doi_tac]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[thanh_toan_doi_tac](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[doi_tac_id] [int] NOT NULL,
	[so_tien] [decimal](18, 2) NOT NULL,
	[ten_ngan_hang] [nvarchar](max) NOT NULL,
	[so_tai_khoan] [nvarchar](max) NOT NULL,
	[chu_tai_khoan] [nvarchar](max) NOT NULL,
	[trang_thai] [nvarchar](max) NOT NULL,
	[url_bang_chung] [nvarchar](max) NULL,
	[id_giao_dich] [nvarchar](max) NULL,
	[ghi_chu_admin] [nvarchar](max) NULL,
	[ngay_tao] [datetime2](7) NOT NULL,
	[ngay_cap_nhat] [datetime2](7) NULL,
	[ngay_thanh_toan] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[thong_bao]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[thong_bao](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[tieu_de] [nvarchar](200) NOT NULL,
	[noi_dung] [nvarchar](max) NULL,
	[loai] [nvarchar](50) NULL,
	[da_doc] [bit] NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[vai_tro]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[vai_tro](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten_vai_tro] [nvarchar](50) NOT NULL,
	[nhom_vai_tro] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[yeu_thich]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[yeu_thich](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nguoi_dung_id] [int] NOT NULL,
	[phong_id] [int] NOT NULL,
	[ngay_tao] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260331071240_InitialCreate', N'9.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260405072304_AddPayouts', N'9.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260405082124_AddPartnerResourcesSyncFinal', N'9.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260405082134_AddPartnerSyncClean', N'9.0.0')
GO
SET IDENTITY_INSERT [dbo].[anh_co_so] ON 

INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (1, 1, N'https://example.com/facilities/facility_1.jpg', 1, 1, CAST(N'2026-02-01T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (2, 2, N'https://example.com/facilities/facility_2.jpg', 1, 1, CAST(N'2026-02-02T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (3, 3, N'https://example.com/facilities/facility_3.jpg', 1, 1, CAST(N'2026-02-03T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (4, 4, N'https://example.com/facilities/facility_4.jpg', 1, 1, CAST(N'2026-02-04T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (5, 5, N'https://example.com/facilities/facility_5.jpg', 1, 1, CAST(N'2026-02-05T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (6, 6, N'https://example.com/facilities/facility_6.jpg', 1, 1, CAST(N'2026-02-06T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (7, 7, N'https://example.com/facilities/facility_7.jpg', 1, 1, CAST(N'2026-02-07T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (8, 8, N'https://example.com/facilities/facility_8.jpg', 1, 1, CAST(N'2026-02-08T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (9, 9, N'https://example.com/facilities/facility_9.jpg', 1, 1, CAST(N'2026-02-09T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (10, 10, N'https://example.com/facilities/facility_10.jpg', 1, 1, CAST(N'2026-02-10T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (11, 11, N'https://example.com/facilities/facility_11.jpg', 1, 1, CAST(N'2026-02-11T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (12, 12, N'https://example.com/facilities/facility_12.jpg', 1, 1, CAST(N'2026-02-12T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (13, 13, N'https://example.com/facilities/facility_13.jpg', 1, 1, CAST(N'2026-02-13T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (14, 14, N'https://example.com/facilities/facility_14.jpg', 1, 1, CAST(N'2026-02-14T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (15, 15, N'https://example.com/facilities/facility_15.jpg', 1, 1, CAST(N'2026-02-15T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (16, 16, N'https://example.com/facilities/facility_16.jpg', 1, 1, CAST(N'2026-02-16T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (17, 17, N'https://example.com/facilities/facility_17.jpg', 1, 1, CAST(N'2026-02-17T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (18, 18, N'https://example.com/facilities/facility_18.jpg', 1, 1, CAST(N'2026-02-18T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (19, 19, N'https://example.com/facilities/facility_19.jpg', 1, 1, CAST(N'2026-02-19T10:00:00.000' AS DateTime))
INSERT [dbo].[anh_co_so] ([id], [co_so_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (20, 20, N'https://example.com/facilities/facility_20.jpg', 1, 1, CAST(N'2026-02-20T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[anh_co_so] OFF
GO
SET IDENTITY_INSERT [dbo].[anh_phong] ON 

INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (1, 1, N'https://example.com/rooms/room_1.jpg', 1, 1, CAST(N'2026-03-01T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (2, 2, N'https://example.com/rooms/room_2.jpg', 1, 1, CAST(N'2026-03-02T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (3, 3, N'https://example.com/rooms/room_3.jpg', 1, 1, CAST(N'2026-03-03T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (4, 4, N'https://example.com/rooms/room_4.jpg', 1, 1, CAST(N'2026-03-04T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (5, 5, N'https://example.com/rooms/room_5.jpg', 1, 1, CAST(N'2026-03-05T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (6, 6, N'https://example.com/rooms/room_6.jpg', 1, 1, CAST(N'2026-03-06T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (7, 7, N'https://example.com/rooms/room_7.jpg', 1, 1, CAST(N'2026-03-07T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (9, 9, N'https://example.com/rooms/room_9.jpg', 1, 1, CAST(N'2026-03-09T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (10, 10, N'https://example.com/rooms/room_10.jpg', 1, 1, CAST(N'2026-03-10T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (11, 11, N'https://example.com/rooms/room_11.jpg', 1, 1, CAST(N'2026-03-11T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (13, 13, N'https://example.com/rooms/room_13.jpg', 1, 1, CAST(N'2026-03-13T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (14, 14, N'https://example.com/rooms/room_14.jpg', 1, 1, CAST(N'2026-03-14T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (15, 15, N'https://example.com/rooms/room_15.jpg', 1, 1, CAST(N'2026-03-15T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (19, 19, N'https://example.com/rooms/room_19.jpg', 1, 1, CAST(N'2026-03-19T11:00:00.000' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (33, 27, N'http://localhost:5000/uploads/rooms/a146fa67-a4f8-415d-852d-926b688667d8_OIP (2) - Copy.jpg', 1, 0, CAST(N'2026-04-03T21:20:08.470' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (34, 27, N'http://localhost:5000/uploads/rooms/f665245c-7161-4f46-826d-6278b78cdd9f_OIP (2).jpg', 0, 1, CAST(N'2026-04-03T21:20:08.470' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (35, 27, N'http://localhost:5000/uploads/rooms/dbebe08f-2b17-42aa-ac01-d7bf10fbf61f_OIP (4) - Copy.jpg', 0, 2, CAST(N'2026-04-03T21:20:08.470' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (36, 27, N'http://localhost:5000/uploads/rooms/cd6f3734-8f85-4ebe-accb-67c579e3e02c_OIP (4).jpg', 0, 3, CAST(N'2026-04-03T21:20:08.470' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (37, 27, N'http://localhost:5000/uploads/rooms/97a734bc-f174-47b6-af4f-4c2687454bd4_OIP (5).jpg', 0, 4, CAST(N'2026-04-03T21:20:08.470' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (39, 29, N'http://localhost:5000/uploads/rooms/f367ce94-f746-4196-a380-756050a23258_OIP (4).jpg', 1, 0, CAST(N'2026-04-03T22:53:41.740' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (45, 31, N'http://localhost:5000/uploads/rooms/e6b1f2fe-f5f9-4c48-9920-f59d6c3cb466_OIP (1) - Copy.jpg', 1, 0, CAST(N'2026-04-05T00:07:31.323' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (46, 31, N'http://localhost:5000/uploads/rooms/41b886ef-411f-4c53-ba67-56da7f3a0e1c_OIP (4) - Copy.jpg', 0, 1, CAST(N'2026-04-05T00:07:31.323' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (47, 31, N'http://localhost:5000/uploads/rooms/445cd14e-3f4e-446d-ac7f-8c1d771e310e_OIP (5).jpg', 0, 2, CAST(N'2026-04-05T00:07:31.323' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (48, 31, N'http://localhost:5000/uploads/rooms/37c34171-1ce1-46f0-a8a6-e56fcc0c9000_OIP (6).jpg', 0, 3, CAST(N'2026-04-05T00:07:31.323' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (49, 32, N'http://localhost:5000/uploads/rooms/3e038920-b001-4b55-b984-703bbb99ac46_OIP (1).jpg', 1, 0, CAST(N'2026-04-05T22:11:25.390' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (50, 32, N'http://localhost:5000/uploads/rooms/46cee8b0-00de-4f63-8bab-e6249ea3320e_OIP (2) - Copy.jpg', 0, 1, CAST(N'2026-04-05T22:11:25.390' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (51, 32, N'http://localhost:5000/uploads/rooms/61befff6-a1d5-4f21-85ba-c01f3ab0db06_OIP (5) - Copy.jpg', 0, 2, CAST(N'2026-04-05T22:11:25.390' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (52, 32, N'http://localhost:5000/uploads/rooms/5b935ac3-ad5e-4edf-97ef-01d290fedb15_OIP (6) - Copy.jpg', 0, 3, CAST(N'2026-04-05T22:11:25.390' AS DateTime))
INSERT [dbo].[anh_phong] ([id], [phong_id], [url_anh], [la_anh_chinh], [thu_tu_hien_thi], [ngay_tao]) VALUES (53, 32, N'http://localhost:5000/uploads/rooms/6eef7608-ce57-4fa0-abd8-003d9a401863_OIP - Copy.jpg', 0, 4, CAST(N'2026-04-05T22:11:25.390' AS DateTime))
SET IDENTITY_INSERT [dbo].[anh_phong] OFF
GO
SET IDENTITY_INSERT [dbo].[co_so_luu_tru] ON 

INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (1, 21, N'Azure Bay Resort & Spa', N'Khach san', N'Ha Noi', N'Ba Dinh', N'Phuong 1', N'101 Trung Tam, Ba Dinh, Ha Noi', CAST(105.710000 AS Decimal(10, 6)), CAST(21.020000 AS Decimal(10, 6)), N'Resort sang trong gan trung tam Ha Noi.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.85 AS Decimal(3, 2)), 42, CAST(N'2026-01-01T08:00:00.000' AS DateTime), CAST(N'2026-02-01T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (2, 2, N'Urban Loft Downtown', N'Homestay', N'Ho Chi Minh', N'Quan 1', N'Phuong 2', N'102 Nguyen Hue, Quan 1, Ho Chi Minh', CAST(106.695000 AS Decimal(10, 6)), CAST(10.773000 AS Decimal(10, 6)), N'Can ho phong cach giua trung tam SG.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.62 AS Decimal(3, 2)), 35, CAST(N'2026-01-02T08:00:00.000' AS DateTime), CAST(N'2026-02-02T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (3, 3, N'Luxury Sunset Villa', N'Villa', N'Da Nang', N'Hai Chau', N'Phuong 3', N'103 Bach Dang, Hai Chau, Da Nang', CAST(108.220000 AS Decimal(10, 6)), CAST(16.047000 AS Decimal(10, 6)), N'Villa view bien tuyet dep tai Da Nang.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.90 AS Decimal(3, 2)), 58, CAST(N'2026-01-03T08:00:00.000' AS DateTime), CAST(N'2026-02-03T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (4, 4, N'Seaside Cottage Halong', N'Homestay', N'Quang Ninh', N'Ha Long', N'Phuong 4', N'104 Tran Quoc Nghien, Ha Long', CAST(107.060000 AS Decimal(10, 6)), CAST(20.951000 AS Decimal(10, 6)), N'Nha nghi ven bien Ha Long Bay.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.40 AS Decimal(3, 2)), 21, CAST(N'2026-01-04T08:00:00.000' AS DateTime), CAST(N'2026-02-04T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (5, 5, N'Mountain Cabin Sapa', N'Homestay', N'Lao Cai', N'Sa Pa', N'Phuong 5', N'105 Ham Rong, Sa Pa, Lao Cai', CAST(103.842000 AS Decimal(10, 6)), CAST(22.336000 AS Decimal(10, 6)), N'Cabin nghi duong giua rung thong Sa Pa.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.72 AS Decimal(3, 2)), 49, CAST(N'2026-01-05T08:00:00.000' AS DateTime), CAST(N'2026-02-05T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (6, 6, N'Modern Villa Bali Style', N'Villa', N'Khanh Hoa', N'Nha Trang', N'Phuong 6', N'106 Tran Phu, Nha Trang, Khanh Hoa', CAST(109.194000 AS Decimal(10, 6)), CAST(12.238000 AS Decimal(10, 6)), N'Villa phong cach Bali view bien Nha Trang.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.78 AS Decimal(3, 2)), 63, CAST(N'2026-01-06T08:00:00.000' AS DateTime), CAST(N'2026-02-06T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (7, 7, N'Heritage Homestay Hue', N'Homestay', N'Thua Thien Hue', N'Hue', N'Phuong 7', N'107 Le Loi, Hue, Thua Thien Hue', CAST(107.590000 AS Decimal(10, 6)), CAST(16.462000 AS Decimal(10, 6)), N'Homestay phong cach co kinh giua Hue.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.55 AS Decimal(3, 2)), 30, CAST(N'2026-01-07T08:00:00.000' AS DateTime), CAST(N'2026-02-07T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (8, 8, N'Oceanfront Apartment', N'Can ho dich vu', N'Ba Ria - Vung Tau', N'Vung Tau', N'Phuong 8', N'108 Tran Phu, Vung Tau', CAST(107.083000 AS Decimal(10, 6)), CAST(10.346000 AS Decimal(10, 6)), N'Can ho view bien Vung Tau full tien nghi.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Bao tri', CAST(4.30 AS Decimal(3, 2)), 17, CAST(N'2026-01-08T08:00:00.000' AS DateTime), CAST(N'2026-02-08T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (9, 9, N'Dalat Flower Villa', N'Villa', N'Lam Dong', N'Da Lat', N'Phuong 9', N'109 Nguyen Van Cu, Da Lat, Lam Dong', CAST(108.438000 AS Decimal(10, 6)), CAST(11.940000 AS Decimal(10, 6)), N'Villa giua thung lung hoa Da Lat.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.67 AS Decimal(3, 2)), 44, CAST(N'2026-01-09T08:00:00.000' AS DateTime), CAST(N'2026-02-09T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (10, 10, N'Quy Nhon Beach Resort', N'Resort', N'Binh Dinh', N'Quy Nhon', N'Phuong 10', N'110 An Duong Vuong, Quy Nhon, Binh Dinh', CAST(109.220000 AS Decimal(10, 6)), CAST(13.776000 AS Decimal(10, 6)), N'Resort bien sang trong tai Quy Nhon.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.50 AS Decimal(3, 2)), 28, CAST(N'2026-01-10T08:00:00.000' AS DateTime), CAST(N'2026-02-10T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (11, 11, N'Phu Quoc Pearl Hotel', N'Khach san', N'Kien Giang', N'Phu Quoc', N'Phuong 11', N'111 Tran Hung Dao, Duong Dong, Phu Quoc', CAST(103.966000 AS Decimal(10, 6)), CAST(10.290000 AS Decimal(10, 6)), N'Khach san 4 sao giua dao Phu Quoc.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.80 AS Decimal(3, 2)), 71, CAST(N'2026-01-11T08:00:00.000' AS DateTime), CAST(N'2026-02-11T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (12, 12, N'Mekong Delta Riverside', N'Homestay', N'Ben Tre', N'Ben Tre', N'Phuong 12', N'112 Hung Vuong, Ben Tre', CAST(106.375000 AS Decimal(10, 6)), CAST(10.243000 AS Decimal(10, 6)), N'Homestay ben song Mekong thu vi.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.35 AS Decimal(3, 2)), 19, CAST(N'2026-01-12T08:00:00.000' AS DateTime), CAST(N'2026-02-12T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (13, 13, N'Saigon Business Hotel', N'Khach san', N'Ho Chi Minh', N'Quan 3', N'Phuong 13', N'113 Vo Van Tan, Quan 3, Ho Chi Minh', CAST(106.685000 AS Decimal(10, 6)), CAST(10.777000 AS Decimal(10, 6)), N'Khach san thuong mai trung tam SG.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.20 AS Decimal(3, 2)), 33, CAST(N'2026-01-13T08:00:00.000' AS DateTime), CAST(N'2026-02-13T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (14, 14, N'Sam Son Beach Hotel', N'Khach san', N'Thanh Hoa', N'Sam Son', N'Phuong 14', N'114 Le Loi, Sam Son, Thanh Hoa', CAST(105.897000 AS Decimal(10, 6)), CAST(19.737000 AS Decimal(10, 6)), N'Khach san bien Sam Son.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.10 AS Decimal(3, 2)), 15, CAST(N'2026-01-14T08:00:00.000' AS DateTime), CAST(N'2026-02-14T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (15, 15, N'Cua Lo Sea View Resort', N'Resort', N'Nghe An', N'Cua Lo', N'Phuong 15', N'115 Bung, Cua Lo, Nghe An', CAST(105.729000 AS Decimal(10, 6)), CAST(18.804000 AS Decimal(10, 6)), N'Resort view bien Cua Lo thoang mat.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(3.95 AS Decimal(3, 2)), 11, CAST(N'2026-01-15T08:00:00.000' AS DateTime), CAST(N'2026-02-15T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (16, 16, N'Binh Duong Business Center', N'Can ho dich vu', N'Binh Duong', N'Thu Dau Mot', N'Phuong 16', N'116 Dai Lo Binh Duong, Thu Dau Mot', CAST(106.653000 AS Decimal(10, 6)), CAST(10.980000 AS Decimal(10, 6)), N'Can ho dich vu cao cap Binh Duong.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.25 AS Decimal(3, 2)), 22, CAST(N'2026-01-16T08:00:00.000' AS DateTime), CAST(N'2026-02-16T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (17, 17, N'Bien Hoa Garden Stay', N'Homestay', N'Dong Nai', N'Bien Hoa', N'Phuong 17', N'117 Pham Van Thuan, Bien Hoa, Dong Nai', CAST(106.825000 AS Decimal(10, 6)), CAST(10.947000 AS Decimal(10, 6)), N'Homestay trong xanh tai Bien Hoa.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.15 AS Decimal(3, 2)), 14, CAST(N'2026-01-17T08:00:00.000' AS DateTime), CAST(N'2026-02-17T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (18, 18, N'Long An River Hotel', N'Khach san', N'Long An', N'Tan An', N'Phuong 18', N'118 Hung Vuong, Tan An, Long An', CAST(106.411000 AS Decimal(10, 6)), CAST(10.535000 AS Decimal(10, 6)), N'Khach san ben song tai Tan An.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Bao tri', CAST(4.00 AS Decimal(3, 2)), 9, CAST(N'2026-01-18T08:00:00.000' AS DateTime), CAST(N'2026-02-18T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (19, 19, N'Con Dao Island Retreat', N'Resort', N'Ba Ria - Vung Tau', N'Con Dao', N'Phuong 19', N'119 Ton Duc Thang, Con Dao', CAST(106.608000 AS Decimal(10, 6)), CAST(8.683000 AS Decimal(10, 6)), N'Resort sang trong tren dao Con Dao.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.95 AS Decimal(3, 2)), 87, CAST(N'2026-01-19T08:00:00.000' AS DateTime), CAST(N'2026-02-19T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (20, 20, N'Chau Doc Floating Hotel', N'Homestay', N'An Giang', N'Chau Doc', N'Phuong 20', N'120 Thuong Dang Le, Chau Doc, An Giang', CAST(105.119000 AS Decimal(10, 6)), CAST(10.705000 AS Decimal(10, 6)), N'Homestay noi song doc dao tai Chau Doc.', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Dang hoat dong', CAST(4.45 AS Decimal(3, 2)), 26, CAST(N'2026-01-20T08:00:00.000' AS DateTime), CAST(N'2026-02-20T08:00:00.000' AS DateTime), NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (22, 22, N'Grand Plaza Hotel', N'Khách sạn', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', NULL, NULL, NULL, NULL, NULL, N'active', CAST(0.00 AS Decimal(3, 2)), 0, CAST(N'2026-04-03T22:50:42.263' AS DateTime), NULL, NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (23, 24, N'Long dz vcl', N'Khách sạn', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', NULL, NULL, NULL, NULL, NULL, N'active', CAST(0.00 AS Decimal(3, 2)), 0, CAST(N'2026-04-03T23:44:48.503' AS DateTime), NULL, NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (24, 25, N'Test Partner', N'Khách sạn', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', CAST(0.000000 AS Decimal(10, 6)), CAST(0.000000 AS Decimal(10, 6)), NULL, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'active', CAST(0.00 AS Decimal(3, 2)), 0, CAST(N'2026-04-05T15:26:02.917' AS DateTime), NULL, NULL)
INSERT [dbo].[co_so_luu_tru] ([id], [doi_tac_id], [ten_co_so], [loai_co_so], [tinh_thanh], [quan_huyen], [phuong_xa], [dia_chi_chi_tiet], [kinh_do], [vi_do], [mo_ta], [gio_nhan_phong_mac_dinh], [gio_tra_phong_mac_dinh], [trang_thai], [diem_trung_binh], [tong_so_danh_gia], [ngay_tao], [ngay_cap_nhat], [url_anh_dai_dien]) VALUES (25, 26, N'Khách Sạn Nam Định', N'Khách sạn', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', N'Chưa cập nhật', CAST(0.000000 AS Decimal(10, 6)), CAST(0.000000 AS Decimal(10, 6)), NULL, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'active', CAST(0.00 AS Decimal(3, 2)), 0, CAST(N'2026-04-05T22:10:42.190' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[co_so_luu_tru] OFF
GO
SET IDENTITY_INSERT [dbo].[danh_gia] ON 

INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (1, 15, 3, 2, 5, N'Phong tuyet voi, view dep, dich vu chuyen nghiep. Se quay lai!', CAST(N'2026-03-16T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (2, 17, 6, 4, 5, N'Villa Bali phong cach, ho boi sang trong. Nhan vien than thien.', CAST(N'2026-03-21T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (3, 19, 11, 6, 5, N'Island Suite xung dang tien, bai bien rieng, thuc an ngon.', CAST(N'2026-03-17T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (4, 18, 13, 12, 4, N'Phong sach, vi tri tot, gia hop ly cho cong tac.', CAST(N'2026-03-06T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (5, 16, 19, 10, 5, N'Con Dao Villa dep nhu mo, bien xanh, khong khi trong lanh. Perfect!', CAST(N'2026-03-31T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (6, 17, 3, 18, 5, N'Valentine at Sunset Villa - khong the quen! Cam on doi ngu nhiet tinh.', CAST(N'2026-02-18T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (7, 20, 1, 14, 4, N'Phong Ocean View dep, sach se, gia hop ly. Khuyen khich ban be.', CAST(N'2026-03-09T10:00:00.000' AS DateTime))
INSERT [dbo].[danh_gia] ([id], [nguoi_dung_id], [phong_id], [dat_phong_id], [so_sao], [binh_luan], [ngay_tao]) VALUES (8, 16, 11, 11, 5, N'Phu Quoc Island Suite - thien duong nghi duong. Dich vu 5 sao!', CAST(N'2026-03-05T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[danh_gia] OFF
GO
SET IDENTITY_INSERT [dbo].[dat_phong] ON 

INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (1, 14, 1, CAST(N'2026-04-01T14:00:00.000' AS DateTime), CAST(N'2026-04-04T12:00:00.000' AS DateTime), 3, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1200000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(120000.00 AS Decimal(18, 2)), CAST(147000.00 AS Decimal(18, 2)), CAST(600000.00 AS Decimal(18, 2)), CAST(4017000.00 AS Decimal(18, 2)), N'checked_in', N'Can phong view bien.', CAST(N'2026-03-01T10:00:00.000' AS DateTime), CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (2, 15, 3, CAST(N'2026-04-05T14:00:00.000' AS DateTime), CAST(N'2026-04-08T12:00:00.000' AS DateTime), 3, 4, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(2500000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), CAST(295000.00 AS Decimal(18, 2)), CAST(1250000.00 AS Decimal(18, 2)), CAST(8245000.00 AS Decimal(18, 2)), N'Completed', N'Dat nhan dip le.', CAST(N'2026-03-02T10:00:00.000' AS DateTime), CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (3, 16, 5, CAST(N'2026-04-10T14:00:00.000' AS DateTime), CAST(N'2026-04-12T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(750000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), CAST(92500.00 AS Decimal(18, 2)), CAST(375000.00 AS Decimal(18, 2)), CAST(2017500.00 AS Decimal(18, 2)), N'Confirmed', N'Nghi duong cuoi tuan.', CAST(N'2026-03-03T10:00:00.000' AS DateTime), CAST(N'2026-03-03T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (4, 17, 6, CAST(N'2026-03-15T14:00:00.000' AS DateTime), CAST(N'2026-03-18T12:00:00.000' AS DateTime), 3, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1800000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(180000.00 AS Decimal(18, 2)), CAST(219000.00 AS Decimal(18, 2)), CAST(900000.00 AS Decimal(18, 2)), CAST(5949000.00 AS Decimal(18, 2)), N'Completed', N'Tuan trang mat.', CAST(N'2026-03-01T09:00:00.000' AS DateTime), CAST(N'2026-03-20T09:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (5, 18, 2, CAST(N'2026-04-15T14:00:00.000' AS DateTime), CAST(N'2026-04-17T12:00:00.000' AS DateTime), 2, 1, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(850000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), CAST(85000.00 AS Decimal(18, 2)), CAST(101500.00 AS Decimal(18, 2)), CAST(425000.00 AS Decimal(18, 2)), CAST(2116500.00 AS Decimal(18, 2)), N'Confirmed', N'Cong tac Ha Noi.', CAST(N'2026-03-05T10:00:00.000' AS DateTime), CAST(N'2026-03-05T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (6, 19, 11, CAST(N'2026-03-10T14:00:00.000' AS DateTime), CAST(N'2026-03-15T12:00:00.000' AS DateTime), 5, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(2800000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), CAST(280000.00 AS Decimal(18, 2)), CAST(334000.00 AS Decimal(18, 2)), CAST(1400000.00 AS Decimal(18, 2)), CAST(14814000.00 AS Decimal(18, 2)), N'Completed', N'Nghi le thang 3.', CAST(N'2026-03-01T08:00:00.000' AS DateTime), CAST(N'2026-03-16T08:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (7, 20, 9, CAST(N'2026-04-20T14:00:00.000' AS DateTime), CAST(N'2026-04-22T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(680000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), CAST(68000.00 AS Decimal(18, 2)), CAST(82800.00 AS Decimal(18, 2)), CAST(340000.00 AS Decimal(18, 2)), CAST(1910800.00 AS Decimal(18, 2)), N'Pending', N'Dang cho xac nhan.', CAST(N'2026-03-10T10:00:00.000' AS DateTime), CAST(N'2026-03-10T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (8, 14, 10, CAST(N'2026-03-20T14:00:00.000' AS DateTime), CAST(N'2026-03-23T12:00:00.000' AS DateTime), 3, 4, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1400000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(140000.00 AS Decimal(18, 2)), CAST(172000.00 AS Decimal(18, 2)), CAST(700000.00 AS Decimal(18, 2)), CAST(4762000.00 AS Decimal(18, 2)), N'Cancelled', N'Huy vi su co ca nhan.', CAST(N'2026-03-05T09:00:00.000' AS DateTime), CAST(N'2026-03-12T09:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (9, 15, 7, CAST(N'2026-05-01T14:00:00.000' AS DateTime), CAST(N'2026-05-03T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(550000.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), CAST(55000.00 AS Decimal(18, 2)), CAST(66500.00 AS Decimal(18, 2)), CAST(275000.00 AS Decimal(18, 2)), CAST(1531500.00 AS Decimal(18, 2)), N'Confirmed', N'Tham quan Hue.', CAST(N'2026-03-15T10:00:00.000' AS DateTime), CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (10, 16, 19, CAST(N'2026-03-25T14:00:00.000' AS DateTime), CAST(N'2026-03-29T12:00:00.000' AS DateTime), 4, 6, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(4500000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), CAST(450000.00 AS Decimal(18, 2)), CAST(525000.00 AS Decimal(18, 2)), CAST(2250000.00 AS Decimal(18, 2)), CAST(21275000.00 AS Decimal(18, 2)), N'Completed', N'Nghi duong Con Dao.', CAST(N'2026-03-10T08:00:00.000' AS DateTime), CAST(N'2026-03-30T08:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (11, 17, 4, CAST(N'2026-04-25T14:00:00.000' AS DateTime), CAST(N'2026-04-27T12:00:00.000' AS DateTime), 2, 3, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(650000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), CAST(65000.00 AS Decimal(18, 2)), CAST(79500.00 AS Decimal(18, 2)), CAST(325000.00 AS Decimal(18, 2)), CAST(1874500.00 AS Decimal(18, 2)), N'Confirmed', N'Tham quan Ha Long.', CAST(N'2026-03-20T10:00:00.000' AS DateTime), CAST(N'2026-03-20T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (12, 18, 13, CAST(N'2026-03-01T14:00:00.000' AS DateTime), CAST(N'2026-03-04T12:00:00.000' AS DateTime), 3, 1, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1100000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), CAST(110000.00 AS Decimal(18, 2)), CAST(133000.00 AS Decimal(18, 2)), CAST(550000.00 AS Decimal(18, 2)), CAST(3643000.00 AS Decimal(18, 2)), N'Completed', N'Cong tac SG.', CAST(N'2026-02-20T10:00:00.000' AS DateTime), CAST(N'2026-03-05T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (13, 19, 15, CAST(N'2026-05-10T14:00:00.000' AS DateTime), CAST(N'2026-05-14T12:00:00.000' AS DateTime), 4, 5, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(950000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(95000.00 AS Decimal(18, 2)), CAST(114000.00 AS Decimal(18, 2)), CAST(475000.00 AS Decimal(18, 2)), CAST(4159000.00 AS Decimal(18, 2)), N'Confirmed', N'Du lich gia dinh.', CAST(N'2026-03-25T10:00:00.000' AS DateTime), CAST(N'2026-03-25T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (14, 20, 1, CAST(N'2026-03-05T14:00:00.000' AS DateTime), CAST(N'2026-03-07T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1200000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(120000.00 AS Decimal(18, 2)), CAST(147000.00 AS Decimal(18, 2)), CAST(600000.00 AS Decimal(18, 2)), CAST(2817000.00 AS Decimal(18, 2)), N'Completed', N'Nghi cuoi tuan.', CAST(N'2026-03-01T09:00:00.000' AS DateTime), CAST(N'2026-03-08T09:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (15, 14, 6, CAST(N'2026-04-08T14:00:00.000' AS DateTime), CAST(N'2026-04-10T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(1800000.00 AS Decimal(18, 2)), CAST(150000.00 AS Decimal(18, 2)), CAST(180000.00 AS Decimal(18, 2)), CAST(219000.00 AS Decimal(18, 2)), CAST(900000.00 AS Decimal(18, 2)), CAST(4149000.00 AS Decimal(18, 2)), N'Cancelled', N'Doi lich cong tac.', CAST(N'2026-03-15T10:00:00.000' AS DateTime), CAST(N'2026-03-20T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (16, 15, 9, CAST(N'2026-05-20T14:00:00.000' AS DateTime), CAST(N'2026-05-23T12:00:00.000' AS DateTime), 3, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(680000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), CAST(68000.00 AS Decimal(18, 2)), CAST(82800.00 AS Decimal(18, 2)), CAST(340000.00 AS Decimal(18, 2)), CAST(2270800.00 AS Decimal(18, 2)), N'Pending', N'Cho xac nhan phong.', CAST(N'2026-03-28T10:00:00.000' AS DateTime), CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (17, 16, 11, CAST(N'2026-06-01T14:00:00.000' AS DateTime), CAST(N'2026-06-05T12:00:00.000' AS DateTime), 4, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(2800000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), CAST(280000.00 AS Decimal(18, 2)), CAST(334000.00 AS Decimal(18, 2)), CAST(1400000.00 AS Decimal(18, 2)), CAST(11814000.00 AS Decimal(18, 2)), N'Confirmed', N'Nghi he Phu Quoc.', CAST(N'2026-03-29T10:00:00.000' AS DateTime), CAST(N'2026-03-29T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (18, 17, 3, CAST(N'2026-02-14T14:00:00.000' AS DateTime), CAST(N'2026-02-16T12:00:00.000' AS DateTime), 2, 4, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(2500000.00 AS Decimal(18, 2)), CAST(200000.00 AS Decimal(18, 2)), CAST(250000.00 AS Decimal(18, 2)), CAST(295000.00 AS Decimal(18, 2)), CAST(1250000.00 AS Decimal(18, 2)), CAST(5745000.00 AS Decimal(18, 2)), N'Completed', N'Valentine trip.', CAST(N'2026-02-10T10:00:00.000' AS DateTime), CAST(N'2026-02-17T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (19, 18, 5, CAST(N'2026-04-30T14:00:00.000' AS DateTime), CAST(N'2026-05-02T12:00:00.000' AS DateTime), 2, 2, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(750000.00 AS Decimal(18, 2)), CAST(100000.00 AS Decimal(18, 2)), CAST(75000.00 AS Decimal(18, 2)), CAST(92500.00 AS Decimal(18, 2)), CAST(375000.00 AS Decimal(18, 2)), CAST(2017500.00 AS Decimal(18, 2)), N'Confirmed', N'Nghi Sa Pa thang 4.', CAST(N'2026-03-28T10:00:00.000' AS DateTime), CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (20, 19, 14, CAST(N'2026-03-22T14:00:00.000' AS DateTime), CAST(N'2026-03-25T12:00:00.000' AS DateTime), 3, 3, CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), CAST(600000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), CAST(60000.00 AS Decimal(18, 2)), CAST(72000.00 AS Decimal(18, 2)), CAST(300000.00 AS Decimal(18, 2)), CAST(1952000.00 AS Decimal(18, 2)), N'Cancelled', N'Huy do thoi tiet.', CAST(N'2026-03-15T10:00:00.000' AS DateTime), CAST(N'2026-03-22T10:00:00.000' AS DateTime))
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (21, 21, 27, CAST(N'2026-04-14T00:00:00.000' AS DateTime), CAST(N'2026-04-15T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(63.90 AS Decimal(18, 2)), CAST(213.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-04T13:39:35.760' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (22, 21, 29, CAST(N'2026-04-14T00:00:00.000' AS DateTime), CAST(N'2026-04-16T00:00:00.000' AS DateTime), 2, 1, NULL, NULL, CAST(12.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(7.20 AS Decimal(18, 2)), CAST(24.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-04T14:22:10.303' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (23, 21, 27, CAST(N'2026-04-17T00:00:00.000' AS DateTime), CAST(N'2026-04-30T00:00:00.000' AS DateTime), 13, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(830.70 AS Decimal(18, 2)), CAST(2769.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-04T14:24:50.310' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (24, 21, 27, CAST(N'2026-05-01T00:00:00.000' AS DateTime), CAST(N'2026-05-02T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(63.90 AS Decimal(18, 2)), CAST(213.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-04T14:48:19.670' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (25, 21, 27, CAST(N'2026-05-03T00:00:00.000' AS DateTime), CAST(N'2026-05-29T00:00:00.000' AS DateTime), 26, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(1661.40 AS Decimal(18, 2)), CAST(5538.00 AS Decimal(18, 2)), N'checked_in', NULL, CAST(N'2026-04-04T14:49:53.167' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (26, 21, 29, CAST(N'2026-05-04T00:00:00.000' AS DateTime), CAST(N'2026-12-31T00:00:00.000' AS DateTime), 241, 1, NULL, NULL, CAST(12.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(867.60 AS Decimal(18, 2)), CAST(2892.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-04T14:55:02.723' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (27, 21, 27, CAST(N'2026-06-01T00:00:00.000' AS DateTime), CAST(N'2026-07-01T00:00:00.000' AS DateTime), 30, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(1917.00 AS Decimal(18, 2)), CAST(6390.00 AS Decimal(18, 2)), N'checked_in', NULL, CAST(N'2026-04-04T15:03:08.077' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (28, 21, 27, CAST(N'2027-01-01T00:00:00.000' AS DateTime), CAST(N'2027-02-01T00:00:00.000' AS DateTime), 31, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(1980.90 AS Decimal(18, 2)), CAST(6603.00 AS Decimal(18, 2)), N'checked_in', NULL, CAST(N'2026-04-04T15:14:22.827' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (29, 67, 27, CAST(N'2028-01-01T00:00:00.000' AS DateTime), CAST(N'2028-02-01T00:00:00.000' AS DateTime), 31, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(1980.90 AS Decimal(18, 2)), CAST(6603.00 AS Decimal(18, 2)), N'completed', NULL, CAST(N'2026-04-04T15:52:45.573' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (30, 21, 31, CAST(N'2029-01-02T00:00:00.000' AS DateTime), CAST(N'2029-01-03T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(1000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(300.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-05T00:08:22.100' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (31, 21, 31, CAST(N'2030-03-01T00:00:00.000' AS DateTime), CAST(N'2030-03-04T00:00:00.000' AS DateTime), 3, 1, NULL, NULL, CAST(1000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(900.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), N'completed', NULL, CAST(N'2026-04-05T00:09:29.680' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (32, 66, 31, CAST(N'2031-03-01T00:00:00.000' AS DateTime), CAST(N'2031-03-04T00:00:00.000' AS DateTime), 3, 1, NULL, NULL, CAST(1000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(900.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), N'completed', NULL, CAST(N'2026-04-05T00:11:41.380' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (33, 68, 31, CAST(N'2026-04-05T00:00:00.000' AS DateTime), CAST(N'2026-04-06T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(1000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(300.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-05T15:41:10.370' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (34, 68, 27, CAST(N'2026-04-05T00:00:00.000' AS DateTime), CAST(N'2026-04-06T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(63.90 AS Decimal(18, 2)), CAST(213.00 AS Decimal(18, 2)), N'pending', NULL, CAST(N'2026-04-05T15:44:07.990' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (35, 69, 32, CAST(N'2026-04-05T00:00:00.000' AS DateTime), CAST(N'2026-04-06T00:00:00.000' AS DateTime), 1, 1, NULL, NULL, CAST(3000.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(900.00 AS Decimal(18, 2)), CAST(3000.00 AS Decimal(18, 2)), N'completed', NULL, CAST(N'2026-04-05T22:12:36.150' AS DateTime), NULL)
INSERT [dbo].[dat_phong] ([id], [nguoi_dung_id], [phong_id], [ngay_nhan_phong], [ngay_tra_phong], [so_dem], [so_nguoi], [gio_nhan_phong], [gio_tra_phong], [gia_moi_dem], [phi_don_dep], [phi_dich_vu], [thue], [dat_coc], [tong_tien], [trang_thai], [ghi_chu], [ngay_tao], [ngay_cap_nhat]) VALUES (36, 49, 27, CAST(N'2090-07-03T00:00:00.000' AS DateTime), CAST(N'2090-07-31T00:00:00.000' AS DateTime), 28, 1, NULL, NULL, CAST(213.00 AS Decimal(18, 2)), NULL, NULL, NULL, CAST(1789.20 AS Decimal(18, 2)), CAST(5964.00 AS Decimal(18, 2)), N'confirmed', NULL, CAST(N'2026-04-07T11:46:25.457' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[dat_phong] OFF
GO
SET IDENTITY_INSERT [dbo].[doi_tac] ON 

INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (1, 4, N'DT001', N'Cong ty Luu Tru A', N'Khach san', N'Ha Noi', N'Doi tac quan ly co so tai Ha Noi.', N'Dang hoat dong', CAST(N'2026-01-01T09:00:00.000' AS DateTime), CAST(N'2026-02-01T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (2, 5, N'DT002', N'Cong ty Luu Tru B', N'Homestay', N'Ho Chi Minh', N'Doi tac quan ly co so tai Ho Chi Minh.', N'Dang hoat dong', CAST(N'2026-01-02T09:00:00.000' AS DateTime), CAST(N'2026-02-02T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (3, 6, N'DT003', N'Cong ty Luu Tru C', N'Can ho dich vu', N'Da Nang', N'Doi tac quan ly co so tai Da Nang.', N'Dang hoat dong', CAST(N'2026-01-03T09:00:00.000' AS DateTime), CAST(N'2026-02-03T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (4, 7, N'DT004', N'Cong ty Luu Tru D', N'Villa', N'Hai Phong', N'Doi tac quan ly co so tai Hai Phong.', N'Tam ngung', CAST(N'2026-01-04T09:00:00.000' AS DateTime), CAST(N'2026-02-04T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (5, 8, N'DT005', N'Cong ty Luu Tru E', N'Nha nghi', N'Can Tho', N'Doi tac quan ly co so tai Can Tho.', N'Dang hoat dong', CAST(N'2026-01-05T09:00:00.000' AS DateTime), CAST(N'2026-02-05T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (6, 9, N'DT006', N'Cong ty Luu Tru F', N'Khach san', N'Khanh Hoa', N'Doi tac quan ly co so tai Khanh Hoa.', N'Dang hoat dong', CAST(N'2026-01-06T09:00:00.000' AS DateTime), CAST(N'2026-02-06T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (7, 10, N'DT007', N'Cong ty Luu Tru G', N'Homestay', N'Thua Thien Hue', N'Doi tac quan ly co so tai Hue.', N'Dang hoat dong', CAST(N'2026-01-07T09:00:00.000' AS DateTime), CAST(N'2026-02-07T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (8, 11, N'DT008', N'Cong ty Luu Tru H', N'Can ho dich vu', N'Ba Ria - Vung Tau', N'Doi tac quan ly co so tai Vung Tau.', N'Tam ngung', CAST(N'2026-01-08T09:00:00.000' AS DateTime), CAST(N'2026-02-08T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (9, 12, N'DT009', N'Cong ty Luu Tru I', N'Villa', N'Lam Dong', N'Doi tac quan ly co so tai Da Lat.', N'Dang hoat dong', CAST(N'2026-01-09T09:00:00.000' AS DateTime), CAST(N'2026-02-09T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (10, 13, N'DT010', N'Cong ty Luu Tru J', N'Resort', N'Binh Dinh', N'Doi tac quan ly co so tai Quy Nhon.', N'Dang hoat dong', CAST(N'2026-01-10T09:00:00.000' AS DateTime), CAST(N'2026-02-10T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (11, 1, N'DT011', N'Cong ty Luu Tru K', N'Khach san', N'Kien Giang', N'Doi tac quan ly co so tai Phu Quoc.', N'Dang hoat dong', CAST(N'2026-01-11T09:00:00.000' AS DateTime), CAST(N'2026-02-11T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (12, 2, N'DT012', N'Cong ty Luu Tru L', N'Homestay', N'Quang Ninh', N'Doi tac quan ly co so tai Ha Long.', N'Tam ngung', CAST(N'2026-01-12T09:00:00.000' AS DateTime), CAST(N'2026-02-12T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (13, 3, N'DT013', N'Cong ty Luu Tru M', N'Can ho dich vu', N'Lao Cai', N'Doi tac quan ly co so tai Sa Pa.', N'Dang hoat dong', CAST(N'2026-01-13T09:00:00.000' AS DateTime), CAST(N'2026-02-13T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (14, 16, N'DT014', N'Cong ty Luu Tru N', N'Villa', N'Thanh Hoa', N'Doi tac quan ly co so tai Sam Son.', N'Dang hoat dong', CAST(N'2026-01-14T09:00:00.000' AS DateTime), CAST(N'2026-02-14T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (15, 17, N'DT015', N'Cong ty Luu Tru O', N'Resort', N'Nghe An', N'Doi tac quan ly co so tai Cua Lo.', N'Dang hoat dong', CAST(N'2026-01-15T09:00:00.000' AS DateTime), CAST(N'2026-02-15T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (16, 18, N'DT016', N'Cong ty Luu Tru P', N'Khach san', N'Binh Duong', N'Doi tac quan ly co so tai Thu Dau Mot.', N'Tam ngung', CAST(N'2026-01-16T09:00:00.000' AS DateTime), CAST(N'2026-02-16T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (17, 19, N'DT017', N'Cong ty Luu Tru Q', N'Homestay', N'Dong Nai', N'Doi tac quan ly co so tai Bien Hoa.', N'Dang hoat dong', CAST(N'2026-01-17T09:00:00.000' AS DateTime), CAST(N'2026-02-17T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (18, 20, N'DT018', N'Cong ty Luu Tru R', N'Can ho dich vu', N'Long An', N'Doi tac quan ly co so tai Tan An.', N'Dang hoat dong', CAST(N'2026-01-18T09:00:00.000' AS DateTime), CAST(N'2026-02-18T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (19, 14, N'DT019', N'Cong ty Luu Tru S', N'Villa', N'Ben Tre', N'Doi tac quan ly co so tai Ben Tre.', N'Dang hoat dong', CAST(N'2026-01-19T09:00:00.000' AS DateTime), CAST(N'2026-02-19T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (20, 15, N'DT020', N'Cong ty Luu Tru T', N'Nha nghi', N'An Giang', N'Doi tac quan ly co so tai Long Xuyen.', N'Tam ngung', CAST(N'2026-01-20T09:00:00.000' AS DateTime), CAST(N'2026-02-20T09:00:00.000' AS DateTime), CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (21, 49, N'DT049', N'Azure Bay Resort', NULL, NULL, NULL, N'active', CAST(N'2026-04-03T21:55:44.277' AS DateTime), CAST(N'2026-04-03T21:55:44.277' AS DateTime), CAST(12603.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (22, 55, NULL, N'Grand Plaza Hotel', N'Cá nhân', NULL, NULL, N'active', CAST(N'2026-04-03T22:50:42.240' AS DateTime), NULL, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (24, 65, N'PARTNER-3A78DA5A', N'Long dz vcl', N'Cá nhân', NULL, NULL, N'active', CAST(N'2026-04-03T23:44:48.480' AS DateTime), NULL, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (25, 68, N'PARTNER-D716B100', N'Test Partner', N'Cá nhân', NULL, NULL, N'active', CAST(N'2026-04-05T15:26:02.873' AS DateTime), NULL, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[doi_tac] ([id], [nguoi_dung_id], [ma_doi_tac], [ten_doanh_nghiep], [loai_hinh_kinh_doanh], [dia_chi], [mo_ta], [trang_thai], [ngay_tao], [ngay_cap_nhat], [so_du_vi]) VALUES (26, 70, N'PARTNER-186C1312', N'Khách Sạn Nam Định', N'Cá nhân', NULL, NULL, N'active', CAST(N'2026-04-05T22:10:42.170' AS DateTime), NULL, CAST(3000.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[doi_tac] OFF
GO
SET IDENTITY_INSERT [dbo].[duyet_bai_dang] ON 

INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (1, 1, 1, N'Da duyet', N'Phong du tieu chuan, duyet dang.', CAST(N'2026-02-02T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (2, 2, 2, N'Da duyet', N'Phong du tieu chuan, duyet dang.', CAST(N'2026-02-03T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (3, 3, 1, N'Da duyet', N'Villa cao cap, duyet dang.', CAST(N'2026-02-04T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (4, 4, 2, N'Da duyet', N'Phong du tieu chuan, duyet dang.', CAST(N'2026-02-05T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (5, 5, 3, N'Da duyet', N'Phong du tieu chuan, duyet dang.', CAST(N'2026-02-06T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (6, 6, 1, N'Da duyet', N'Villa Bali dep, duyet dang.', CAST(N'2026-02-07T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (7, 7, 2, N'Da duyet', N'Phong co kinh, duyet dang.', CAST(N'2026-02-08T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (9, 9, 1, N'Da duyet', N'Phong du tieu chuan, duyet dang.', CAST(N'2026-02-10T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (10, 10, 2, N'Da duyet', N'Bungalow bien dep, duyet dang.', CAST(N'2026-02-11T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (11, 11, 1, N'Da duyet', N'Suite cao cap, duyet dang.', CAST(N'2026-02-12T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (13, 13, 2, N'Da duyet', N'Phong doanh nhan, duyet dang.', CAST(N'2026-02-14T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (14, 14, 1, N'Da duyet', N'Phong bien du tieu chuan.', CAST(N'2026-02-15T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (15, 15, 3, N'Da duyet', N'Phong gia dinh rong rai, duyet.', CAST(N'2026-02-16T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_bai_dang] ([id], [phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (19, 19, 2, N'Da duyet', N'Con Dao Villa cao cap, duyet.', CAST(N'2026-02-20T09:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[duyet_bai_dang] OFF
GO
SET IDENTITY_INSERT [dbo].[duyet_dat_phong] ON 

INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (1, 1, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-01T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (2, 2, 2, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-02T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (3, 3, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-03T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (4, 4, 3, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (5, 5, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-05T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (6, 6, 2, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (7, 7, 3, N'Cho xu ly', N'Dang kiem tra lich phong.', CAST(N'2026-03-10T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (8, 8, 1, N'Tu choi', N'Khach huy, hoan tien da thuc hien.', CAST(N'2026-03-12T10:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (9, 9, 2, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-15T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (10, 10, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-10T09:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (11, 11, 3, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-20T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (12, 12, 2, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-02-20T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (13, 13, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-25T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (14, 14, 3, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (15, 15, 2, N'Tu choi', N'Khach huy, da hoan tien.', CAST(N'2026-03-20T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (16, 16, 1, N'Cho xu ly', N'Dang kiem tra lich phong.', CAST(N'2026-03-28T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (17, 17, 3, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-29T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (18, 18, 2, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-02-10T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (19, 19, 1, N'Da duyet', N'Booking hop le, da xac nhan.', CAST(N'2026-03-28T11:00:00.000' AS DateTime))
INSERT [dbo].[duyet_dat_phong] ([id], [dat_phong_id], [admin_id], [trang_thai], [ghi_chu], [ngay_duyet]) VALUES (20, 20, 3, N'Tu choi', N'Huy do thoi tiet, da hoan tien.', CAST(N'2026-03-22T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[duyet_dat_phong] OFF
GO
SET IDENTITY_INSERT [dbo].[giao_dich_thanh_toan] ON 

INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (1, 1, N'MoMo', N'MOMO_TXN_001', N'REF_001', N'Completed', N'Thanh toan MoMo thanh cong.', CAST(N'2026-03-01T11:30:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (2, 2, N'VNPay', N'VNPAY_TXN_002', N'REF_002', N'Completed', N'Thanh toan VNPay thanh cong.', CAST(N'2026-03-02T12:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (3, 3, N'MoMo', N'MOMO_TXN_003', N'REF_003', N'Completed', N'Dat coc MoMo thanh cong.', CAST(N'2026-03-03T11:30:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (4, 4, N'BankTrans', N'BANK_TXN_004', N'REF_004', N'Completed', N'Chuyen khoan thanh cong.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (5, 5, N'MoMo', N'MOMO_TXN_005', N'REF_005', N'Completed', N'MoMo xac nhan thanh cong.', CAST(N'2026-03-05T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (6, 6, N'VNPay', N'VNPAY_TXN_006', N'REF_006', N'Completed', N'Thanh toan VNPay thanh cong.', CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (7, 7, N'Cash', N'CASH_TXN_007', N'REF_007', N'Pending', N'Cho thu tien mat.', CAST(N'2026-03-10T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (8, 8, N'MoMo', N'MOMO_TXN_008', N'REF_008', N'Refunded', N'Hoan tien MoMo thanh cong.', CAST(N'2026-03-13T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (9, 9, N'BankTrans', N'BANK_TXN_009', N'REF_009', N'Completed', N'Chuyen khoan thanh cong.', CAST(N'2026-03-15T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (10, 10, N'VNPay', N'VNPAY_TXN_010', N'REF_010', N'Completed', N'VNPay xac nhan thanh cong.', CAST(N'2026-03-10T09:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (11, 11, N'MoMo', N'MOMO_TXN_011', N'REF_011', N'Completed', N'MoMo thanh cong.', CAST(N'2026-03-20T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (12, 12, N'BankTrans', N'BANK_TXN_012', N'REF_012', N'Completed', N'Chuyen khoan thanh cong.', CAST(N'2026-02-20T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (13, 13, N'MoMo', N'MOMO_TXN_013', N'REF_013', N'Completed', N'MoMo thanh cong.', CAST(N'2026-03-25T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (14, 14, N'VNPay', N'VNPAY_TXN_014', N'REF_014', N'Completed', N'VNPay thanh cong.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (15, 15, N'MoMo', N'MOMO_TXN_015', N'REF_015', N'Refunded', N'Hoan tien MoMo.', CAST(N'2026-03-21T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (16, 16, N'Cash', N'CASH_TXN_016', N'REF_016', N'Pending', N'Cho thu tien mat.', CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (17, 17, N'MoMo', N'MOMO_TXN_017', N'REF_017', N'Completed', N'MoMo thanh cong.', CAST(N'2026-03-29T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (18, 18, N'BankTrans', N'BANK_TXN_018', N'REF_018', N'Completed', N'Chuyen khoan thanh cong.', CAST(N'2026-02-10T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (19, 19, N'MoMo', N'MOMO_TXN_019', N'REF_019', N'Completed', N'MoMo thanh cong.', CAST(N'2026-03-28T11:00:00.000' AS DateTime))
INSERT [dbo].[giao_dich_thanh_toan] ([id], [thanh_toan_id], [cong_thanh_toan], [ma_giao_dich], [ma_tham_chieu], [trang_thai], [noi_dung_phan_hoi], [ngay_tao]) VALUES (20, 20, N'BankTrans', N'BANK_TXN_020', N'REF_020', N'Refunded', N'Hoan tien chuyen khoan.', CAST(N'2026-03-23T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[giao_dich_thanh_toan] OFF
GO
SET IDENTITY_INSERT [dbo].[khach_dat_phong] ON 

INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (1, 1, N'Nguyen Van An', N'0900000001', N'001234567001', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (2, 2, N'Tran Thi Binh', N'0900000002', N'001234567002', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (3, 3, N'Le Quoc Cuong', N'0900000003', N'001234567003', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (4, 4, N'Pham Thu Dung', N'0900000004', N'001234567004', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (5, 5, N'Hoang Gia Huy', N'0900000005', N'001234567005', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (6, 6, N'Vo Minh Khang', N'0900000006', N'001234567006', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (7, 7, N'Dang Ngoc Lan', N'0900000007', N'001234567007', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (8, 8, N'Bui Thanh Nam', N'0900000008', N'001234567008', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (9, 9, N'Do Mai Oanh', N'0900000009', N'001234567009', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (10, 10, N'Phan Gia Phuc', N'0900000010', N'001234567010', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (11, 11, N'Nguyen Thu Quynh', N'0900000011', N'001234567011', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (12, 12, N'Tran Bao Son', N'0900000012', N'001234567012', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (13, 13, N'Le Thi Thao', N'0900000013', N'001234567013', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (14, 14, N'Pham Duc Trung', N'0900000014', N'001234567014', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (15, 15, N'Ho Thi Uyen', N'0900000015', N'001234567015', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (16, 16, N'Vo Anh Viet', N'0900000016', N'001234567016', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (17, 17, N'Dang Khac Wy', N'0900000017', N'001234567017', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (18, 18, N'Bui Hai Xuan', N'0900000018', N'001234567018', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (19, 19, N'Do Thanh Yen', N'0900000019', N'001234567019', N'Truong nhom')
INSERT [dbo].[khach_dat_phong] ([id], [dat_phong_id], [ho_ten], [so_dien_thoai], [cccd], [vai_tro]) VALUES (20, 20, N'Phan Minh Zed', N'0900000020', N'001234567020', N'Truong nhom')
SET IDENTITY_INSERT [dbo].[khach_dat_phong] OFF
GO
SET IDENTITY_INSERT [dbo].[lich_su_trang_thai_dat_phong] ON 

INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (1, 1, NULL, N'Pending', 14, N'Khach dat phong.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (2, 1, N'Pending', N'Confirmed', 1, N'Admin xac nhan.', CAST(N'2026-03-01T11:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (3, 2, NULL, N'Pending', 15, N'Khach dat phong.', CAST(N'2026-03-02T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (4, 2, N'Pending', N'Confirmed', 1, N'Admin xac nhan.', CAST(N'2026-03-02T11:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (5, 2, N'Confirmed', N'Completed', 1, N'Check-out thanh cong.', CAST(N'2026-03-15T13:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (6, 4, NULL, N'Pending', 17, N'Khach dat phong.', CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (7, 4, N'Pending', N'Confirmed', 2, N'Admin xac nhan.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (8, 4, N'Confirmed', N'Completed', 2, N'Check-out thanh cong.', CAST(N'2026-03-20T13:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (9, 8, NULL, N'Pending', 14, N'Khach dat phong.', CAST(N'2026-03-05T09:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (10, 8, N'Pending', N'Confirmed', 1, N'Admin xac nhan.', CAST(N'2026-03-05T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (11, 8, N'Confirmed', N'Cancelled', 14, N'Huy theo yeu cau khach.', CAST(N'2026-03-12T09:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (12, 7, NULL, N'Pending', 20, N'Khach dat phong.', CAST(N'2026-03-10T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (13, 15, NULL, N'Pending', 14, N'Khach dat phong.', CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (14, 15, N'Pending', N'Cancelled', 2, N'Admin huy theo yc khach.', CAST(N'2026-03-20T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (15, 10, NULL, N'Pending', 16, N'Khach dat phong.', CAST(N'2026-03-10T08:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (16, 10, N'Pending', N'Confirmed', 1, N'Admin xac nhan.', CAST(N'2026-03-10T09:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (17, 10, N'Confirmed', N'Completed', 1, N'Check-out thanh cong.', CAST(N'2026-03-30T13:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (18, 16, NULL, N'Pending', 15, N'Khach dat phong.', CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (19, 20, NULL, N'Pending', 19, N'Khach dat phong.', CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[lich_su_trang_thai_dat_phong] ([id], [dat_phong_id], [trang_thai_cu], [trang_thai_moi], [nguoi_thay_doi_id], [ghi_chu], [ngay_tao]) VALUES (20, 20, N'Pending', N'Cancelled', 3, N'Huy do thoi tiet xau.', CAST(N'2026-03-22T09:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[lich_su_trang_thai_dat_phong] OFF
GO
SET IDENTITY_INSERT [dbo].[log_he_thong] ON 

INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (1, 1, N'LOGIN', N'phien_dang_nhap', N'1', NULL, N'Token tao moi', CAST(N'2026-03-30T07:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (2, 1, N'APPROVE', N'duyet_bai_dang', N'1', N'trang_thai: Cho duyet', N'trang_thai: Da duyet', CAST(N'2026-02-02T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (3, 2, N'APPROVE', N'duyet_bai_dang', N'2', N'trang_thai: Cho duyet', N'trang_thai: Da duyet', CAST(N'2026-02-03T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (4, 3, N'APPROVE', N'duyet_dat_phong', N'3', N'trang_thai: Pending', N'trang_thai: Da duyet', CAST(N'2026-03-03T11:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (5, 1, N'APPROVE', N'duyet_dat_phong', N'1', N'trang_thai: Pending', N'trang_thai: Da duyet', CAST(N'2026-03-01T11:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (6, 14, N'INSERT', N'dat_phong', N'1', NULL, N'Dat phong moi tao.', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (7, 15, N'INSERT', N'dat_phong', N'2', NULL, N'Dat phong moi tao.', CAST(N'2026-03-02T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (8, 14, N'UPDATE', N'dat_phong', N'8', N'trang_thai: Confirmed', N'trang_thai: Cancelled', CAST(N'2026-03-12T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (9, 1, N'REJECT', N'duyet_bai_dang', N'12', N'trang_thai: Cho duyet', N'trang_thai: Tu choi', CAST(N'2026-02-13T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (10, 2, N'UPDATE', N'nguoi_dung', N'5', N'trang_thai: Hoat dong', N'trang_thai: Bi khoa', CAST(N'2026-01-06T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (11, 1, N'PAYMENT', N'thanh_toan', N'8', N'trang_thai: Completed', N'trang_thai: Refunded', CAST(N'2026-03-13T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (12, 3, N'APPROVE', N'duyet_dat_phong', N'4', N'trang_thai: Pending', N'trang_thai: Da duyet', CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (13, 16, N'INSERT', N'dat_phong', N'10', NULL, N'Dat phong moi tao.', CAST(N'2026-03-10T08:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (14, 2, N'UPDATE', N'co_so_luu_tru', N'8', N'trang_thai: Dang hoat dong', N'trang_thai: Bao tri', CAST(N'2026-02-08T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (15, 19, N'INSERT', N'dat_phong', N'6', NULL, N'Dat phong moi tao.', CAST(N'2026-03-01T08:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (16, 1, N'UPDATE', N'phong', N'12', N'trang_thai: Online', N'trang_thai: Archived', CAST(N'2026-02-12T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (17, 3, N'LOGIN', N'phien_dang_nhap', N'3', NULL, N'Token tao moi', CAST(N'2026-03-30T07:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (18, 17, N'INSERT', N'dat_phong', N'4', NULL, N'Dat phong moi tao.', CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (19, 2, N'UPDATE', N'nguoi_dung', N'10', N'trang_thai: Hoat dong', N'trang_thai: Bi khoa', CAST(N'2026-01-11T10:00:00.000' AS DateTime))
INSERT [dbo].[log_he_thong] ([id], [nguoi_dung_id], [hanh_dong], [bang_tac_dong], [khoa_chinh], [du_lieu_cu], [du_lieu_moi], [ngay_tao]) VALUES (20, 1, N'APPROVE', N'duyet_dat_phong', N'6', N'trang_thai: Pending', N'trang_thai: Da duyet', CAST(N'2026-03-01T09:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[log_he_thong] OFF
GO
SET IDENTITY_INSERT [dbo].[nguoi_dung] ON 

INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (1, N'Nguyen Van Ana', N'an@example.com', N'0900000001', N'001234567001', N'nguyenvanan', N'$2b$12$if6PeSpN49NI8oM2AbBfrOFznaXK.FaPDKt52zdoNGJNNdCb4nA8O', N'https://example.com/avatar/user_1.jpg', N'Ha Noi', N'Hoat dong', CAST(N'2026-01-02T08:00:00.000' AS DateTime), CAST(N'2025-12-03T08:00:00.000' AS DateTime), CAST(N'2026-04-03T21:09:41.353' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (2, N'Tran Thi Binh', N'binh@example.com', N'0900000002', N'001234567002', N'tranthibinh', N'$2b$12$h0dX1T4fmWWQzHOr7QJr/uuKFt2wl/96JnlE42ZkhmHWuIqAsMy5W', N'https://example.com/avatar/user_2.jpg', N'Ho Chi Minh', N'Hoat dong', CAST(N'2026-01-03T08:00:00.000' AS DateTime), CAST(N'2025-12-04T08:00:00.000' AS DateTime), CAST(N'2026-01-03T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (3, N'Le Quoc Cuong', N'cuong@example.com', N'0900000003', N'001234567003', N'lequoccuong', N'$2b$12$bADZlc9S.Qii5T09JX83MedIcc48DjR604J0xWOVNfDwrDvoKFuKm', N'https://example.com/avatar/user_3.jpg', N'Da Nang', N'Khong hoat dong', CAST(N'2026-01-04T08:00:00.000' AS DateTime), CAST(N'2025-12-05T08:00:00.000' AS DateTime), CAST(N'2026-01-04T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (4, N'Pham Thu Dung', N'dung@example.com', N'0900000004', N'001234567004', N'phamthudung', N'$2b$12$w7nTAVZnwBkuDdeO.XD9ie/sxtSndisYbvKx1pEiLd60.xVrtCcSC', N'https://example.com/avatar/user_4.jpg', N'Hai Phong', N'Hoat dong', CAST(N'2026-01-05T08:00:00.000' AS DateTime), CAST(N'2025-12-06T08:00:00.000' AS DateTime), CAST(N'2026-01-05T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (5, N'Hoang Gia Huy', N'huy@example.com', N'0900000005', N'001234567005', N'hoanggiahuy', N'$2b$12$LN8EFTIAcwNvTaxuSd32aupGdaeELmhldn93oF7GxijrXxCtCP65.', N'https://example.com/avatar/user_5.jpg', N'Can Tho', N'Bi khoa', CAST(N'2026-01-06T08:00:00.000' AS DateTime), CAST(N'2025-12-07T08:00:00.000' AS DateTime), CAST(N'2026-01-06T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (6, N'Vo Minh Khang', N'khang@example.com', N'0900000006', N'001234567006', N'vominhkhang', N'$2b$12$sRJU81HJCbcswYos6L/OyOqW96y8KzDkYM4h8zmjrCXLvmjc348Uu', N'https://example.com/avatar/user_6.jpg', N'Nha Trang', N'Hoat dong', CAST(N'2026-01-07T08:00:00.000' AS DateTime), CAST(N'2025-12-08T08:00:00.000' AS DateTime), CAST(N'2026-01-07T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (7, N'Dang Ngoc Lan', N'lan@example.com', N'0900000007', N'001234567007', N'dangngoclan', N'$2b$12$Tw84qQTCXaoGN4RmifIcDOUKEmkL9Ma9/ieX4AtMCaeyDd4zAbRmK', N'https://example.com/avatar/user_7.jpg', N'Hue', N'Khong hoat dong', CAST(N'2026-01-08T08:00:00.000' AS DateTime), CAST(N'2025-12-09T08:00:00.000' AS DateTime), CAST(N'2026-01-08T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (8, N'Bui Thanh Nam', N'nam@example.com', N'0900000008', N'001234567008', N'buithanhnam', N'$2b$12$iDef/Oal7UeXuwlxR0tS0exmHUgOe4WuJeWyp48C6qWxxe1Y8k8J6', N'https://example.com/avatar/user_8.jpg', N'Vung Tau', N'Hoat dong', CAST(N'2026-01-09T08:00:00.000' AS DateTime), CAST(N'2025-12-10T08:00:00.000' AS DateTime), CAST(N'2026-01-09T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (9, N'Do Mai Oanh', N'oanh@example.com', N'0900000009', N'001234567009', N'domaioanh', N'$2b$12$rNmvuaIym4dz2y8YQCWL9eg57HIiniWoHdKX6/jWETRom8YlcvlzW', N'https://example.com/avatar/user_9.jpg', N'Da Lat', N'Hoat dong', CAST(N'2026-01-10T08:00:00.000' AS DateTime), CAST(N'2025-12-11T08:00:00.000' AS DateTime), CAST(N'2026-01-10T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (10, N'Phan Gia Phuc', N'phuc@example.com', N'0900000010', N'001234567010', N'phangiaphuc', N'$2b$12$mYyUELq03yt0XrUS1F2MUOgvqjP/vhV2Gr/AOerDs/2w85wlIy2UG', N'https://example.com/avatar/user_10.jpg', N'Quy Nhon', N'Bi khoa', CAST(N'2026-01-11T08:00:00.000' AS DateTime), CAST(N'2025-12-12T08:00:00.000' AS DateTime), CAST(N'2026-01-11T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (11, N'Nguyen Thu Quynh', N'quynh@example.com', N'0900000011', N'001234567011', N'nguyenthuquynh', N'$2b$12$Onl3gd4asTmEmnY6IKyKP.aeNasezHVIw53OCSnlzk3zL9I8p6Pfa', N'https://example.com/avatar/user_11.jpg', N'Phu Quoc', N'Hoat dong', CAST(N'2026-01-12T08:00:00.000' AS DateTime), CAST(N'2025-12-13T08:00:00.000' AS DateTime), CAST(N'2026-01-12T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (12, N'Tran Bao Son', N'son@example.com', N'0900000012', N'001234567012', N'tranbaoson', N'$2b$12$3xtFTLBjsWF3tmfGSTrw2u4VrcjuxnvPGWUQRPsmMwsrVq73WR54K', N'https://example.com/avatar/user_12.jpg', N'Ha Long', N'Hoat dong', CAST(N'2026-01-13T08:00:00.000' AS DateTime), CAST(N'2025-12-14T08:00:00.000' AS DateTime), CAST(N'2026-01-13T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (13, N'Le Thi Thao', N'thao@example.com', N'0900000013', N'001234567013', N'lethithao', N'$2b$12$bdBpbf.TNvAXzsLcadqEY.PzyXEGFRLI5.ulcqLeGbZyGoepFDJf2', N'https://example.com/avatar/user_13.jpg', N'Sapa', N'Khong hoat dong', CAST(N'2026-01-14T08:00:00.000' AS DateTime), CAST(N'2025-12-15T08:00:00.000' AS DateTime), CAST(N'2026-01-14T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (14, N'Pham Duc Trung', N'trung@example.com', N'0900000014', N'001234567014', N'phamductrung', N'$2b$12$6YOqbPWpJ6sHmMNq3Tx4uepSpHdJ1LPH5l6SHnkwmKLQ5XCaks.Ny', N'https://example.com/avatar/user_14.jpg', N'Thanh Hoa', N'Hoat dong', CAST(N'2026-01-15T08:00:00.000' AS DateTime), CAST(N'2025-12-16T08:00:00.000' AS DateTime), CAST(N'2026-01-15T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (15, N'Ho Thi Uyen', N'uyen@example.com', N'0900000015', N'001234567015', N'hothiuyen', N'$2b$12$J6TIiaFnoIk42jeMWQGTBulsp/HnQ53BvQVTLI4HJbKrD./3Z7D5G', N'https://example.com/avatar/user_15.jpg', N'Nghe An', N'Bi khoa', CAST(N'2026-01-16T08:00:00.000' AS DateTime), CAST(N'2025-12-17T08:00:00.000' AS DateTime), CAST(N'2026-01-16T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (16, N'Vo Anh Viet', N'viet@example.com', N'0900000016', N'001234567016', N'voanhviet', N'$2b$12$brluCj7kYRog7Vq00MPhyOQzigdZ4Qlkzlatku1Ur/5HYLjmpvTuS', N'https://example.com/avatar/user_16.jpg', N'Binh Duong', N'Hoat dong', CAST(N'2026-01-17T08:00:00.000' AS DateTime), CAST(N'2025-12-18T08:00:00.000' AS DateTime), CAST(N'2026-01-17T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (17, N'Dang Khac Wy', N'wy@example.com', N'0900000017', N'001234567017', N'dangkhacwy', N'$2b$12$mEo6qFAMPVMN22u/nV2nxeIKwY4qwSmPr1q8KhBXcPgLOYsbAJovG', N'https://example.com/avatar/user_17.jpg', N'Dong Nai', N'Hoat dong', CAST(N'2026-01-18T08:00:00.000' AS DateTime), CAST(N'2025-12-19T08:00:00.000' AS DateTime), CAST(N'2026-01-18T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (18, N'Bui Hai Xuan', N'xuan@example.com', N'0900000018', N'001234567018', N'buihaixuan', N'$2b$12$JIJFpdrTZMDwHFeLySdcp.LNAb0PGIzU9gi0K68BmkAVAnRu4fsgy', N'https://example.com/avatar/user_18.jpg', N'Long An', N'Hoat dong', CAST(N'2026-01-19T08:00:00.000' AS DateTime), CAST(N'2025-12-20T08:00:00.000' AS DateTime), CAST(N'2026-01-19T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (19, N'Do Thanh Yen', N'yen@example.com', N'0900000019', N'001234567019', N'dothanhyen', N'$2b$12$U1D5vspWXfG/JYxQYLXvu.uz6/C/5Iga05gZU.TfxRtO/vNIrMYhG', N'https://example.com/avatar/user_19.jpg', N'Ben Tre', N'Hoat dong', CAST(N'2026-01-20T08:00:00.000' AS DateTime), CAST(N'2025-12-21T08:00:00.000' AS DateTime), CAST(N'2026-01-20T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (20, N'Phan Minh Zed', N'zed@example.com', N'0900000020', N'001234567020', N'phanminhzed', N'$2b$12$N1FnVYAuhV0mM4XZn0mJhe0T0SRy36W1wk5OU.wx5XMnDFcjAuTRS', N'https://example.com/avatar/user_20.jpg', N'Kien Giang', N'Bi khoa', CAST(N'2026-01-21T08:00:00.000' AS DateTime), CAST(N'2025-12-22T08:00:00.000' AS DateTime), CAST(N'2026-01-21T08:00:00.000' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (21, N'Trần Đức Long', N'longdeptrai@gmail.com', N'0385485516', N'', N'longdeptrai@gmail.com', N'$2a$11$X5xaYFr8O/0ASyWed7IQ0OpY5inxfqzazmYWsG1ZJDxrs5xaZppke', N'https://scontent.fhan14-5.fna.fbcdn.net/v/t39.30808-1/452982723_504905618725225_2553083424152706719_n.jpg?stp=c0.73.1025.1025a_dst-jpg_s200x200_tt6&_nc_cat=106&ccb=1-7&_nc_sid=2d3e12&_nc_ohc=0LU5I9QfBOMQ7kNvwHWHBt8&_nc_oc=Adqkl0oBbRX6YB-1QevxzqlBXUxDiHXEzVAhm1tj9md5hs6lfEKct2_RUGjIEcqpkwg&_nc_zt=24&_nc_ht=scontent.fhan14-5.fna&_nc_gid=MyXE3LohyOQkboo0hnP5rg&_nc_ss=7a389&oh=00_Af2-uU3qU-A7Iojsuc0f-04I_Qhz_b81d67O1szTnpkcKA&oe=69D5BB74', N'', N'active', NULL, CAST(N'2026-04-02T22:41:06.010' AS DateTime), CAST(N'2026-04-03T23:46:27.270' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (48, N'Constraint Fix', N'test_cfix@example.com', N'0123456789', NULL, N'test_cfix@example.com', N'$2a$11$CA8GDuNBivO4GSSrufrfheM6dzysgv5znKXh1oSbUQ1.i.4sR8ARG', NULL, NULL, N'active', NULL, CAST(N'2026-04-03T09:40:08.083' AS DateTime), NULL)
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (49, N'Grand Plaza Hotel', N'longdeptrai1@gmail.com', N'12434312', N'036205004557', N'longdeptrai1@gmail.com', N'$2a$11$mjSGlTCDtIJInpijS59Lv.fqucIuKr8ztvWrG1Q3qKYX9Vc1MZE.O', N'data:image/jpeg;base64,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', N'', N'active', NULL, CAST(N'2026-04-03T09:41:36.487' AS DateTime), CAST(N'2026-04-04T00:50:23.807' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (54, N'Long Plaza Hotel', N'longdeptrai33@gmail.com', N'123123', NULL, N'longdeptrai33@gmail.com', N'$2a$11$YpFPwKCol5d/HOAMyGvl5OH61AgllkXeIUiDWt7FowBlg9tGrOudC', NULL, NULL, N'active', NULL, CAST(N'2026-04-03T22:40:18.617' AS DateTime), NULL)
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (55, N'Grand Plaza Hotel', N'longdeptrai2@gmail.com', N'123123123', N'123123123', N'longdeptrai2@gmail.com', N'$2a$11$gPykRgpNVs2qVaPHkfHlx.tHKo4PSKJ/DLKnc0FxIkyeFCxbksFPi', N'data:image/jpeg;base64,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', N'TDP Lâm Thọ, Thị trấn Quất Lâm, Giao Thủy, Nam Định', N'active', NULL, CAST(N'2026-04-03T22:50:41.977' AS DateTime), CAST(N'2026-04-03T23:09:32.940' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (59, N'asdasd', N'longdeptrai5@gmail.com', N'123', NULL, N'longdeptrai5@gmail.com', N'$2a$11$q7OwTGvjIdp05s3K5.8e6.WeCgVl6euYE28a9HhKKSZleXRX6d4Ge', NULL, NULL, N'active', NULL, CAST(N'2026-04-03T23:23:20.850' AS DateTime), NULL)
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (65, N'Long dz vcl', N'newbusiness@gmail.com', N'0776267518', N'', N'newbusiness@gmail.com', N'$2a$11$VktRsFT/LwNuqJpo26L5aeqxFu9FHd4iTGDuWUzlTt9rPjPSQsaYy', N'https://scontent.fhan14-5.fna.fbcdn.net/v/t39.30808-1/452982723_504905618725225_2553083424152706719_n.jpg?stp=c0.73.1025.1025a_dst-jpg_s200x200_tt6&_nc_cat=106&ccb=1-7&_nc_sid=2d3e12&_nc_ohc=0LU5I9QfBOMQ7kNvwHWHBt8&_nc_oc=Adqkl0oBbRX6YB-1QevxzqlBXUxDiHXEzVAhm1tj9md5hs6lfEKct2_RUGjIEcqpkwg&_nc_zt=24&_nc_ht=scontent.fhan14-5.fna&_nc_gid=MyXE3LohyOQkboo0hnP5rg&_nc_ss=7a389&oh=00_Af2-uU3qU-A7Iojsuc0f-04I_Qhz_b81d67O1szTnpkcKA&oe=69D5BB74', N'', N'active', NULL, CAST(N'2026-04-03T23:44:48.290' AS DateTime), CAST(N'2026-04-03T23:45:47.953' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (66, N'Admin', N'admin@staymaster.com', N'', N'', N'admin@staymaster.com', N'$2a$11$q5dTfe2XCQvLScRkk1STX.eM1T5LnvtZamTVIUdBetbWqKfZ1toKC', N'data:image/png;base64,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', N'', N'active', NULL, CAST(N'2026-04-04T00:32:17.583' AS DateTime), CAST(N'2026-04-04T13:26:17.547' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (67, N'Nguyen Van Nang', N'nguyenvannang@gmail.com', N'0999999999', N'', N'nguyenvannang@gmail.com', N'$2a$11$G4tvVl.G24LVRpF88S/6Oe9Jl6Zv3Zn9mnnf5X9sfeYqrpWu5CdtW', N'data:image/jpeg;base64,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', N'', N'active', NULL, CAST(N'2026-04-04T15:52:20.053' AS DateTime), CAST(N'2026-04-04T15:54:02.117' AS DateTime))
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (68, N'Test Partner', N'testpartner@gmail.com', N'0987654321', NULL, N'testpartner@gmail.com', N'$2a$11$U7P4JfaRafD1xFweMkpe3usfA4lEe8b549pBvPTwsBAwykvB/AnUa', NULL, NULL, N'active', NULL, CAST(N'2026-04-05T15:26:02.603' AS DateTime), NULL)
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (69, N'Trịnh Hữu Tài', N'tai@gmail.com', N'1234567890', NULL, N'tai@gmail.com', N'$2a$11$HkiHX0KqddhY7ckwP/lWgONtyzVIySMQcxeacOz9ZXN0KC7CdJ812', NULL, NULL, N'active', NULL, CAST(N'2026-04-05T22:10:08.163' AS DateTime), NULL)
INSERT [dbo].[nguoi_dung] ([id], [ho_ten], [email], [so_dien_thoai], [cccd], [ten_dang_nhap], [mat_khau], [anh_dai_dien], [dia_chi], [trang_thai], [lan_cuoi_hoat_dong], [ngay_tao], [ngay_cap_nhat]) VALUES (70, N'Khách Sạn Nam Định', N'namdinhks@gmail.com', N'12345678901', NULL, N'namdinhks@gmail.com', N'$2a$11$YU.ztQvlgl0GvvqCK0wyseIgvYmc6Xle1oTeLtvrpdBFmovJFKiQe', NULL, NULL, N'active', NULL, CAST(N'2026-04-05T22:10:42.153' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[nguoi_dung] OFF
GO
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (1, 1)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (2, 2)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (3, 3)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (4, 4)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (5, 5)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (6, 6)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (7, 7)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (8, 8)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (9, 9)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (10, 12)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (11, 13)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (12, 14)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (13, 16)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (14, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (15, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (16, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (17, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (18, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (19, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (20, 15)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (48, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (49, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (54, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (55, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (59, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (65, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (66, 2)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (67, 21)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (68, 22)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (69, 21)
INSERT [dbo].[nguoi_dung_vai_tro] ([nguoi_dung_id], [vai_tro_id]) VALUES (70, 22)
GO
SET IDENTITY_INSERT [dbo].[phien_dang_nhap] ON 

INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (1, 1, N'token_001_abcdef123456', N'Chrome Windows', N'192.168.1.1', CAST(N'2026-04-06T07:00:00.000' AS DateTime), CAST(N'2026-03-30T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (2, 2, N'token_002_abcdef123456', N'Safari iPhone', N'192.168.1.2', CAST(N'2026-04-06T07:00:00.000' AS DateTime), CAST(N'2026-03-30T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (3, 3, N'token_003_abcdef123456', N'Edge Windows', N'192.168.1.3', CAST(N'2026-04-06T07:00:00.000' AS DateTime), CAST(N'2026-03-30T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (4, 4, N'token_004_abcdef123456', N'Chrome Android', N'192.168.1.4', CAST(N'2026-04-05T07:00:00.000' AS DateTime), CAST(N'2026-03-29T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (5, 6, N'token_006_abcdef123456', N'Safari iPhone', N'192.168.1.6', CAST(N'2026-04-05T07:00:00.000' AS DateTime), CAST(N'2026-03-29T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (6, 8, N'token_008_abcdef123456', N'Chrome Android', N'192.168.1.8', CAST(N'2026-04-04T07:00:00.000' AS DateTime), CAST(N'2026-03-28T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (7, 9, N'token_009_abcdef123456', N'Chrome Windows', N'192.168.1.9', CAST(N'2026-04-04T07:00:00.000' AS DateTime), CAST(N'2026-03-28T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (8, 11, N'token_011_abcdef123456', N'Edge Windows', N'192.168.1.11', CAST(N'2026-04-03T07:00:00.000' AS DateTime), CAST(N'2026-03-27T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (9, 12, N'token_012_abcdef123456', N'Chrome Android', N'192.168.1.12', CAST(N'2026-04-03T07:00:00.000' AS DateTime), CAST(N'2026-03-27T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (10, 14, N'token_014_abcdef123456', N'Safari iPhone', N'192.168.1.14', CAST(N'2026-04-02T07:00:00.000' AS DateTime), CAST(N'2026-03-26T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (11, 15, N'token_015_abcdef123456', N'Edge Windows', N'192.168.1.15', CAST(N'2026-04-02T07:00:00.000' AS DateTime), CAST(N'2026-03-26T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (12, 16, N'token_016_abcdef123456', N'Chrome Android', N'192.168.1.16', CAST(N'2026-04-01T07:00:00.000' AS DateTime), CAST(N'2026-03-25T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (13, 17, N'token_017_abcdef123456', N'Chrome Windows', N'192.168.1.17', CAST(N'2026-04-01T07:00:00.000' AS DateTime), CAST(N'2026-03-25T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (14, 18, N'token_018_abcdef123456', N'Safari iPhone', N'192.168.1.18', CAST(N'2026-03-31T07:00:00.000' AS DateTime), CAST(N'2026-03-24T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (15, 19, N'token_019_abcdef123456', N'Edge Windows', N'192.168.1.19', CAST(N'2026-03-31T07:00:00.000' AS DateTime), CAST(N'2026-03-24T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (16, 20, N'token_020_abcdef123456', N'Chrome Android', N'192.168.1.20', CAST(N'2026-03-30T07:00:00.000' AS DateTime), CAST(N'2026-03-23T07:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (17, 1, N'token_101_xyz987654321', N'Safari MacOS', N'10.0.0.1', CAST(N'2026-04-06T08:00:00.000' AS DateTime), CAST(N'2026-03-30T08:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (18, 14, N'token_102_xyz987654321', N'Chrome Windows', N'10.0.0.14', CAST(N'2026-04-02T08:00:00.000' AS DateTime), CAST(N'2026-03-26T08:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (19, 15, N'token_103_xyz987654321', N'Firefox Windows', N'10.0.0.15', CAST(N'2026-04-02T08:00:00.000' AS DateTime), CAST(N'2026-03-26T08:00:00.000' AS DateTime))
INSERT [dbo].[phien_dang_nhap] ([id], [nguoi_dung_id], [token], [thiet_bi], [dia_chi_ip], [het_han_luc], [ngay_tao]) VALUES (20, 16, N'token_104_xyz987654321', N'Chrome Android', N'10.0.0.16', CAST(N'2026-04-01T08:00:00.000' AS DateTime), CAST(N'2026-03-25T08:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[phien_dang_nhap] OFF
GO
SET IDENTITY_INSERT [dbo].[phong] ON 

INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (1, 1, N'Deluxe Ocean View', N'P001', N'Deluxe', 2, 1, CAST(1200000.00 AS Decimal(18, 2)), CAST(32.00 AS Decimal(10, 2)), N'Phong deluxe view bien, day du tien nghi cao cap.', N'occupied', 1, CAST(N'2026-02-01T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:17:55.493' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (2, 2, N'Urban Loft Studio', N'P002', N'Standard', 2, 1, CAST(850000.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(10, 2)), N'Studio phong cach hien dai giua trung tam thanh pho.', N'deleted', 0, CAST(N'2026-02-02T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:37:26.963' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (3, 3, N'Sunset Villa Suite', N'P003', N'Suite', 4, 2, CAST(2500000.00 AS Decimal(18, 2)), CAST(65.00 AS Decimal(10, 2)), N'Suite cao cap view hoang hon tuyet dep.', N'deleted', 0, CAST(N'2026-02-03T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:37:32.427' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (4, 4, N'Halong Bay Cabin', N'P004', N'Standard', 3, 2, CAST(650000.00 AS Decimal(18, 2)), CAST(22.00 AS Decimal(10, 2)), N'Cabin am cung nhin ra vinh Ha Long.', N'deleted', 0, CAST(N'2026-02-04T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:37:53.110' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (5, 5, N'Sapa Forest Cabin', N'P005', N'Standard', 2, 1, CAST(750000.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(10, 2)), N'Cabin giua rung thong Sa Pa thu vi.', N'deleted', 0, CAST(N'2026-02-05T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:36.323' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (6, 6, N'Bali Infinity Pool', N'P006', N'Deluxe', 2, 1, CAST(1800000.00 AS Decimal(18, 2)), CAST(45.00 AS Decimal(10, 2)), N'Phong deluxe ho boi vo cuc view bien.', N'deleted', 0, CAST(N'2026-02-06T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:42.697' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (7, 7, N'Heritage Room Hue', N'P007', N'Standard', 2, 1, CAST(550000.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(10, 2)), N'Phong co dien phong cach hoang trieu Hue.', N'deleted', 0, CAST(N'2026-02-07T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:45.723' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (9, 9, N'Dalat Garden Room', N'P009', N'Standard', 2, 1, CAST(680000.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(10, 2)), N'Phong view vuon hoa Da Lat tuyet dep.', N'deleted', 0, CAST(N'2026-02-09T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:02.407' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (10, 10, N'Beach Front Bungalow', N'P010', N'Deluxe', 4, 2, CAST(1400000.00 AS Decimal(18, 2)), CAST(40.00 AS Decimal(10, 2)), N'Bungalow sat bien Quy Nhon rieng tu.', N'deleted', 0, CAST(N'2026-02-10T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:51.123' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (11, 11, N'Island Paradise Suite', N'P011', N'Suite', 2, 1, CAST(2800000.00 AS Decimal(18, 2)), CAST(55.00 AS Decimal(10, 2)), N'Suite hang sang giua dao Phu Quoc.', N'deleted', 0, CAST(N'2026-02-11T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:55.167' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (13, 13, N'Business Executive', N'P013', N'Deluxe', 1, 1, CAST(1100000.00 AS Decimal(18, 2)), CAST(30.00 AS Decimal(10, 2)), N'Phong doanh nhan day du tien nghi cong so.', N'deleted', 0, CAST(N'2026-02-13T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:58.103' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (14, 14, N'Beachside Standard', N'P014', N'Standard', 3, 2, CAST(600000.00 AS Decimal(18, 2)), CAST(22.00 AS Decimal(10, 2)), N'Phong standard gan bien Sam Son.', N'deleted', 0, CAST(N'2026-02-14T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:36:57.260' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (15, 15, N'Sea Breeze Family', N'P015', N'Family', 5, 3, CAST(950000.00 AS Decimal(18, 2)), CAST(38.00 AS Decimal(10, 2)), N'Phong gia dinh rong rai gan bien Cua Lo.', N'deleted', 0, CAST(N'2026-02-15T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:38:09.947' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (19, 19, N'Con Dao Ocean Villa', N'P019', N'Villa', 6, 3, CAST(4500000.00 AS Decimal(18, 2)), CAST(120.00 AS Decimal(10, 2)), N'Villa hang sang rieng tu tren dao Con Dao.', N'deleted', 1, CAST(N'2026-02-19T09:30:00.000' AS DateTime), CAST(N'2026-04-03T10:25:49.023' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (27, 1, N'Ocean Park', N'RM-1-899', N'Standard', 2, 1, CAST(213.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(10, 2)), N'Beautiful Sea', N'available', 1, CAST(N'2026-04-03T21:20:08.433' AS DateTime), CAST(N'2026-04-03T21:20:33.783' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (29, 22, N'q2312', N'R-P0DNY', N'Standard', 2, 1, CAST(12.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(10, 2)), N'12312', N'approved', 1, CAST(N'2026-04-03T22:53:41.583' AS DateTime), CAST(N'2026-04-03T23:06:51.980' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (31, 1, N'VinHome 1', N'R-4002', N'Standard', 2, 1, CAST(1200.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(10, 2)), N'Beautiful', N'available', 1, CAST(N'2026-04-05T00:07:31.183' AS DateTime), CAST(N'2026-04-07T12:09:42.410' AS DateTime))
INSERT [dbo].[phong] ([id], [co_so_id], [ten_phong], [ma_phong], [loai_phong], [suc_chua], [so_giuong], [gia_co_ban], [dien_tich], [mo_ta], [trang_thai], [duoc_duyet], [ngay_tao], [ngay_cap_nhat]) VALUES (32, 25, N'Khách Sạn 5 sao', N'R-401', N'Standard', 2, 1, CAST(3000.00 AS Decimal(18, 2)), CAST(25.00 AS Decimal(10, 2)), N'Beautiful room', N'rejected', 0, CAST(N'2026-04-05T22:11:25.377' AS DateTime), CAST(N'2026-04-05T22:18:52.290' AS DateTime))
SET IDENTITY_INSERT [dbo].[phong] OFF
GO
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (1, 20)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (2, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (2, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (2, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (2, 4)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (2, 16)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 4)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 7)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (3, 20)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (4, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (4, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (4, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (4, 14)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (5, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (5, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (5, 5)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (5, 14)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (6, 20)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (7, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (7, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (7, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (7, 16)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (9, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (9, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (9, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (9, 5)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (10, 20)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 12)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (11, 20)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 4)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 9)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (13, 16)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (14, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (14, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (14, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (14, 14)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (15, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (15, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (15, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (15, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (15, 12)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 1)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 2)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 3)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 6)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 7)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 9)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 17)
INSERT [dbo].[phong_tien_ich] ([phong_id], [tien_ich_id]) VALUES (19, 20)
GO
SET IDENTITY_INSERT [dbo].[tai_khoan_ngan_hang_doi_tac] ON 

INSERT [dbo].[tai_khoan_ngan_hang_doi_tac] ([id], [doi_tac_id], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [la_tai_khoan_chinh], [ngay_tao], [ngay_cap_nhat]) VALUES (1, 25, N'Vietcombank', N'123456789', N'NGUYEN VAN A', 0, CAST(N'2026-04-05T15:26:57.9107642' AS DateTime2), NULL)
INSERT [dbo].[tai_khoan_ngan_hang_doi_tac] ([id], [doi_tac_id], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [la_tai_khoan_chinh], [ngay_tao], [ngay_cap_nhat]) VALUES (2, 21, N'Vietcombank', N'1028660104', N'TRAN DUC LONG', 1, CAST(N'2026-04-05T15:29:16.1206493' AS DateTime2), NULL)
INSERT [dbo].[tai_khoan_ngan_hang_doi_tac] ([id], [doi_tac_id], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [la_tai_khoan_chinh], [ngay_tao], [ngay_cap_nhat]) VALUES (3, 26, N'Vietcombank', N'1028660104', N'TRAN DUC LONG', 1, CAST(N'2026-04-05T22:15:26.9738439' AS DateTime2), NULL)
SET IDENTITY_INSERT [dbo].[tai_khoan_ngan_hang_doi_tac] OFF
GO
SET IDENTITY_INSERT [dbo].[tien_ich] ON 

INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (1, N'Wifi', N'Co ban', N'wifi')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (2, N'May lanh', N'Co ban', N'ac')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (3, N'TV', N'Giai tri', N'tv')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (4, N'Tu lanh', N'Tien nghi', N'fridge')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (5, N'Ban cong', N'Phong canh', N'balcony')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (6, N'Ho boi', N'Giai tri', N'pool')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (7, N'Bep rieng', N'Tien nghi', N'kitchen')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (8, N'May giat', N'Tien nghi', N'washer')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (9, N'Bai do xe', N'Dich vu', N'parking')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (10, N'Thang may', N'Dich vu', N'elevator')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (11, N'Le tan 24/7', N'Dich vu', N'reception')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (12, N'An sang', N'Am thuc', N'breakfast')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (13, N'Phong gym', N'Suc khoe', N'gym')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (14, N'Bao ve', N'An ninh', N'security')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (15, N'Camera', N'An ninh', N'camera')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (16, N'Ban lam viec', N'Cong viec', N'desk')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (17, N'Bon tam', N'Tien nghi', N'bath')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (18, N'May say toc', N'Tien nghi', N'dryer')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (19, N'Ban ui', N'Tien nghi', N'iron')
INSERT [dbo].[tien_ich] ([id], [ten_tien_ich], [nhom_tien_ich], [icon]) VALUES (20, N'View bien', N'Phong canh', N'sea-view')
SET IDENTITY_INSERT [dbo].[tien_ich] OFF
GO
SET IDENTITY_INSERT [dbo].[thanh_toan] ON 

INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (1, 1, N'Toan bo', CAST(4017000.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-03T12:00:00.000' AS DateTime), CAST(N'2026-03-01T11:30:00.000' AS DateTime), CAST(N'2026-03-01T11:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (2, 2, N'Toan bo', CAST(8245000.00 AS Decimal(18, 2)), N'The tin dung', N'Completed', CAST(N'2026-03-04T12:00:00.000' AS DateTime), CAST(N'2026-03-02T12:00:00.000' AS DateTime), CAST(N'2026-03-02T11:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (3, 3, N'Dat coc', CAST(2017500.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-05T12:00:00.000' AS DateTime), CAST(N'2026-03-03T11:30:00.000' AS DateTime), CAST(N'2026-03-03T11:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (4, 4, N'Toan bo', CAST(5949000.00 AS Decimal(18, 2)), N'Chuyen khoan ngan hang', N'Completed', CAST(N'2026-03-02T12:00:00.000' AS DateTime), CAST(N'2026-03-01T10:00:00.000' AS DateTime), CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (5, 5, N'Dat coc', CAST(2116500.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-07T12:00:00.000' AS DateTime), CAST(N'2026-03-05T11:00:00.000' AS DateTime), CAST(N'2026-03-05T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (6, 6, N'Toan bo', CAST(14814000.00 AS Decimal(18, 2)), N'The tin dung', N'Completed', CAST(N'2026-03-02T12:00:00.000' AS DateTime), CAST(N'2026-03-01T09:00:00.000' AS DateTime), CAST(N'2026-03-01T08:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (7, 7, N'Dat coc', CAST(1910800.00 AS Decimal(18, 2)), N'Tien mat', N'Pending', CAST(N'2026-03-12T12:00:00.000' AS DateTime), NULL, CAST(N'2026-03-10T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (8, 8, N'Dat coc', CAST(4762000.00 AS Decimal(18, 2)), N'MoMo', N'Refunded', CAST(N'2026-03-07T12:00:00.000' AS DateTime), CAST(N'2026-03-13T10:00:00.000' AS DateTime), CAST(N'2026-03-05T09:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (9, 9, N'Dat coc', CAST(1531500.00 AS Decimal(18, 2)), N'Chuyen khoan ngan hang', N'Completed', CAST(N'2026-03-17T12:00:00.000' AS DateTime), CAST(N'2026-03-15T11:00:00.000' AS DateTime), CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (10, 10, N'Toan bo', CAST(21275000.00 AS Decimal(18, 2)), N'The tin dung', N'Completed', CAST(N'2026-03-12T12:00:00.000' AS DateTime), CAST(N'2026-03-10T09:00:00.000' AS DateTime), CAST(N'2026-03-10T08:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (11, 11, N'Dat coc', CAST(11814000.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-22T12:00:00.000' AS DateTime), CAST(N'2026-03-20T11:00:00.000' AS DateTime), CAST(N'2026-03-20T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (12, 12, N'Toan bo', CAST(3643000.00 AS Decimal(18, 2)), N'Chuyen khoan ngan hang', N'Completed', CAST(N'2026-02-22T12:00:00.000' AS DateTime), CAST(N'2026-02-20T11:00:00.000' AS DateTime), CAST(N'2026-02-20T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (13, 13, N'Dat coc', CAST(4159000.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-27T12:00:00.000' AS DateTime), CAST(N'2026-03-25T11:00:00.000' AS DateTime), CAST(N'2026-03-25T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (14, 14, N'Toan bo', CAST(2817000.00 AS Decimal(18, 2)), N'The tin dung', N'Completed', CAST(N'2026-03-03T12:00:00.000' AS DateTime), CAST(N'2026-03-01T10:00:00.000' AS DateTime), CAST(N'2026-03-01T09:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (15, 15, N'Hoan tien', CAST(4149000.00 AS Decimal(18, 2)), N'MoMo', N'Refunded', CAST(N'2026-03-17T12:00:00.000' AS DateTime), CAST(N'2026-03-21T10:00:00.000' AS DateTime), CAST(N'2026-03-15T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (16, 16, N'Dat coc', CAST(2270800.00 AS Decimal(18, 2)), N'Tien mat', N'Pending', CAST(N'2026-03-30T12:00:00.000' AS DateTime), NULL, CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (17, 17, N'Dat coc', CAST(11814000.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-31T12:00:00.000' AS DateTime), CAST(N'2026-03-29T11:00:00.000' AS DateTime), CAST(N'2026-03-29T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (18, 18, N'Toan bo', CAST(5745000.00 AS Decimal(18, 2)), N'Chuyen khoan ngan hang', N'Completed', CAST(N'2026-02-12T12:00:00.000' AS DateTime), CAST(N'2026-02-10T11:00:00.000' AS DateTime), CAST(N'2026-02-10T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (19, 19, N'Dat coc', CAST(2017500.00 AS Decimal(18, 2)), N'MoMo', N'Completed', CAST(N'2026-03-30T12:00:00.000' AS DateTime), CAST(N'2026-03-28T11:00:00.000' AS DateTime), CAST(N'2026-03-28T10:00:00.000' AS DateTime))
INSERT [dbo].[thanh_toan] ([id], [dat_phong_id], [loai_thanh_toan], [so_tien], [phuong_thuc], [trang_thai], [han_thanh_toan], [ngay_thanh_toan], [ngay_tao]) VALUES (20, 20, N'Hoan tien', CAST(1952000.00 AS Decimal(18, 2)), N'Chuyen khoan ngan hang', N'Refunded', CAST(N'2026-03-17T12:00:00.000' AS DateTime), CAST(N'2026-03-23T10:00:00.000' AS DateTime), CAST(N'2026-03-15T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[thanh_toan] OFF
GO
SET IDENTITY_INSERT [dbo].[thanh_toan_doi_tac] ON 

INSERT [dbo].[thanh_toan_doi_tac] ([id], [doi_tac_id], [so_tien], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [trang_thai], [url_bang_chung], [id_giao_dich], [ghi_chu_admin], [ngay_tao], [ngay_cap_nhat], [ngay_thanh_toan]) VALUES (1, 21, CAST(4000.00 AS Decimal(18, 2)), N'Vietcombank', N'1028660104', N'TRAN DUC LONG', N'paid', N'http://localhost:5000/uploads/rooms/500e56d9-af2a-4e5e-bda0-0099fb82e71a_OIP.jpg', NULL, N'21321', CAST(N'2026-04-05T14:30:30.9372745' AS DateTime2), CAST(N'2026-04-05T14:31:16.3487535' AS DateTime2), CAST(N'2026-04-05T14:31:16.3488065' AS DateTime2))
INSERT [dbo].[thanh_toan_doi_tac] ([id], [doi_tac_id], [so_tien], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [trang_thai], [url_bang_chung], [id_giao_dich], [ghi_chu_admin], [ngay_tao], [ngay_cap_nhat], [ngay_thanh_toan]) VALUES (2, 21, CAST(4000.00 AS Decimal(18, 2)), N'Vietcombank', N'1028660104', N'TRAN DUC LONG', N'paid', N'http://localhost:5000/uploads/rooms/1104915d-fe1a-44ff-ba6d-e6410e4babc3_OIP (6).jpg', NULL, N'12312', CAST(N'2026-04-05T14:33:14.1931718' AS DateTime2), CAST(N'2026-04-05T14:33:47.5667728' AS DateTime2), CAST(N'2026-04-05T14:33:47.5667763' AS DateTime2))
INSERT [dbo].[thanh_toan_doi_tac] ([id], [doi_tac_id], [so_tien], [ten_ngan_hang], [so_tai_khoan], [chu_tai_khoan], [trang_thai], [url_bang_chung], [id_giao_dich], [ghi_chu_admin], [ngay_tao], [ngay_cap_nhat], [ngay_thanh_toan]) VALUES (3, 26, CAST(3000.00 AS Decimal(18, 2)), N'Vietcombank', N'1028660104', N'TRAN DUC LONG', N'paid', N'http://localhost:5000/uploads/rooms/4c4f2727-5e55-43fc-8f1a-f73f05e6dbd0_OIP (5) - Copy.jpg', NULL, N'123', CAST(N'2026-04-05T22:15:36.9741851' AS DateTime2), CAST(N'2026-04-05T22:16:51.4674955' AS DateTime2), CAST(N'2026-04-05T22:16:51.4675390' AS DateTime2))
SET IDENTITY_INSERT [dbo].[thanh_toan_doi_tac] OFF
GO
SET IDENTITY_INSERT [dbo].[thong_bao] ON 

INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (1, 14, N'Dat phong thanh cong', N'Dat phong #1 tai Azure Bay Resort da duoc xac nhan.', N'Dat phong', 0, CAST(N'2026-03-01T11:30:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (2, 15, N'Check-out thanh cong', N'Cam on ban da luu tru tai Sunset Villa. Hen gap lai!', N'Dat phong', 1, CAST(N'2026-03-15T13:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (3, 16, N'Dat phong duoc xac nhan', N'Booking Sapa Cabin #3 da duoc duyet.', N'Dat phong', 0, CAST(N'2026-03-03T11:30:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (4, 17, N'Thanh toan thanh cong', N'Thanh toan booking Villa Bali $1,800,000 da hoan tat.', N'Thanh toan', 1, CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (5, 18, N'Phong duoc duyet dang', N'Urban Loft cua ban da duoc duyet va hien thi cong khai.', N'He thong', 1, CAST(N'2026-02-03T10:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (6, 19, N'Dat phong moi', N'Co booking moi tu khach cho Island Suite.', N'Dat phong', 0, CAST(N'2026-03-20T11:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (7, 20, N'Khuyen mai he 2026', N'Giam 15% cho tat ca booking thang 6-8/2026.', N'Khuyen mai', 0, CAST(N'2026-03-01T08:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (8, 14, N'Hoan tien duoc xu ly', N'Hoan tien booking #8 da duoc chuyen khoan thanh cong.', N'Thanh toan', 1, CAST(N'2026-03-13T11:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (9, 15, N'Danh gia cua ban', N'Cam on ban da danh gia Sunset Villa 5 sao!', N'He thong', 1, CAST(N'2026-03-16T11:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (10, 16, N'Dat phong Con Dao', N'Booking Con Dao Villa 4 dem da xac nhan.', N'Dat phong', 1, CAST(N'2026-03-10T09:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (11, 1, N'Phong moi cho duyet', N'Co 3 phong moi dang cho duyet tu cac doi tac.', N'He thong', 0, CAST(N'2026-03-15T08:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (12, 2, N'Bao cao doanh thu thang 3', N'Tong doanh thu thang 3/2026 tang 12.5% so voi thang truoc.', N'He thong', 0, CAST(N'2026-03-30T08:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (13, 17, N'Dat phong Valentine', N'Cam on ban da chon Sunset Villa cho ky niem dac biet!', N'Dat phong', 1, CAST(N'2026-02-10T11:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (14, 18, N'Phong cho duyet lien', N'Riverside Standard dang can bo sung them thong tin.', N'He thong', 0, CAST(N'2026-02-20T10:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (15, 19, N'Dat phong moi - Gia dinh', N'Co booking tu khach cho Sea Breeze Family Room.', N'Dat phong', 0, CAST(N'2026-03-25T11:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (16, 20, N'Dat phong da huy', N'Booking Floating Waterfront bi huy do thoi tiet xau.', N'Dat phong', 0, CAST(N'2026-03-22T10:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (17, 14, N'Khuyen mai cuoi tuan', N'Giam 10% cho booking tu Thu 6 den CN tai chon loc.', N'Khuyen mai', 0, CAST(N'2026-03-28T08:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (18, 3, N'Tai khoan can xem xet', N'Tai khoan nguoi dung #5 co hien tuong bat thuong.', N'He thong', 0, CAST(N'2026-03-20T09:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (19, 15, N'Dat phong he sap toi', N'Dat phong Phu Quoc he 2026 truoc de duoc gia tot.', N'Khuyen mai', 0, CAST(N'2026-03-25T08:00:00.000' AS DateTime))
INSERT [dbo].[thong_bao] ([id], [nguoi_dung_id], [tieu_de], [noi_dung], [loai], [da_doc], [ngay_tao]) VALUES (20, 16, N'Check-in nhan nhac', N'Nhan nhac: Check-in Con Dao Villa ngay mai luc 14:00.', N'Dat phong', 0, CAST(N'2026-03-24T08:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[thong_bao] OFF
GO
SET IDENTITY_INSERT [dbo].[vai_tro] ON 

INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (1, N'Super Admin', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (2, N'Admin', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (3, N'Nhan vien duyet', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (4, N'Chu doi tac', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (5, N'Quan ly co so', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (6, N'Le tan', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (7, N'Ke toan', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (8, N'CSKH', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (9, N'Marketing', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (10, N'Kiem duyet noi dung', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (11, N'Ho tro ky thuat', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (12, N'Chu phong', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (13, N'Nhan vien don phong', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (14, N'Bao ve', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (15, N'Khach hang', N'Customer')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (16, N'Cong tac vien', N'Business Owner')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (17, N'Quan ly dat phong', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (18, N'Quan ly thanh toan', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (19, N'Quan ly bao cao', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (20, N'Moderator', N'Admin')
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (21, N'customer', NULL)
INSERT [dbo].[vai_tro] ([id], [ten_vai_tro], [nhom_vai_tro]) VALUES (22, N'partner', NULL)
SET IDENTITY_INSERT [dbo].[vai_tro] OFF
GO
SET IDENTITY_INSERT [dbo].[yeu_thich] ON 

INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (1, 14, 1, CAST(N'2026-02-15T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (2, 14, 3, CAST(N'2026-02-16T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (3, 14, 11, CAST(N'2026-02-17T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (4, 15, 3, CAST(N'2026-02-20T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (5, 15, 6, CAST(N'2026-02-21T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (6, 15, 19, CAST(N'2026-02-22T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (7, 16, 5, CAST(N'2026-02-25T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (8, 16, 9, CAST(N'2026-02-26T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (9, 16, 11, CAST(N'2026-02-27T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (10, 17, 6, CAST(N'2026-03-01T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (11, 17, 10, CAST(N'2026-03-02T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (12, 18, 2, CAST(N'2026-03-05T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (13, 18, 13, CAST(N'2026-03-06T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (14, 19, 11, CAST(N'2026-03-08T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (15, 19, 15, CAST(N'2026-03-09T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (16, 20, 1, CAST(N'2026-03-10T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (17, 20, 7, CAST(N'2026-03-11T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (18, 14, 19, CAST(N'2026-03-12T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (19, 15, 10, CAST(N'2026-03-13T10:00:00.000' AS DateTime))
INSERT [dbo].[yeu_thich] ([id], [nguoi_dung_id], [phong_id], [ngay_tao]) VALUES (20, 16, 3, CAST(N'2026-03-14T10:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[yeu_thich] OFF
GO
/****** Object:  Index [idx_co_so_doi_tac_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_co_so_doi_tac_id] ON [dbo].[co_so_luu_tru]
(
	[doi_tac_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_danh_gia_phong_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_danh_gia_phong_id] ON [dbo].[danh_gia]
(
	[phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_dat_phong_ngay_nhan]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_dat_phong_ngay_nhan] ON [dbo].[dat_phong]
(
	[ngay_nhan_phong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_dat_phong_nguoi_dung_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_dat_phong_nguoi_dung_id] ON [dbo].[dat_phong]
(
	[nguoi_dung_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_dat_phong_phong_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_dat_phong_phong_id] ON [dbo].[dat_phong]
(
	[phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_dat_phong_trang_thai]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_dat_phong_trang_thai] ON [dbo].[dat_phong]
(
	[trang_thai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__doi_tac__3F7CE076B9FA3D26]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[doi_tac] ADD UNIQUE NONCLUSTERED 
(
	[nguoi_dung_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__doi_tac__9EE94757C5DF2518]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[doi_tac] ADD UNIQUE NONCLUSTERED 
(
	[ma_doi_tac] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_duyet_bai_dang_phong_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_duyet_bai_dang_phong_id] ON [dbo].[duyet_bai_dang]
(
	[phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_duyet_dat_phong_dat_phong_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_duyet_dat_phong_dat_phong_id] ON [dbo].[duyet_dat_phong]
(
	[dat_phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__nguoi_du__363698B3E51325C0]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[nguoi_dung] ADD UNIQUE NONCLUSTERED 
(
	[ten_dang_nhap] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__nguoi_du__AB6E616468F28038]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[nguoi_dung] ADD UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__nguoi_du__BD03D94C57206B67]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[nguoi_dung] ADD UNIQUE NONCLUSTERED 
(
	[so_dien_thoai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [uq_co_so_ma_phong]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[phong] ADD  CONSTRAINT [uq_co_so_ma_phong] UNIQUE NONCLUSTERED 
(
	[co_so_id] ASC,
	[ma_phong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_phong_co_so_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_phong_co_so_id] ON [dbo].[phong]
(
	[co_so_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_phong_trang_thai]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_phong_trang_thai] ON [dbo].[phong]
(
	[trang_thai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BankAcc_Partner]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [IX_BankAcc_Partner] ON [dbo].[tai_khoan_ngan_hang_doi_tac]
(
	[doi_tac_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__tien_ich__072156A0121C1F86]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[tien_ich] ADD UNIQUE NONCLUSTERED 
(
	[ten_tien_ich] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_thanh_toan_dat_phong_id]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_thanh_toan_dat_phong_id] ON [dbo].[thanh_toan]
(
	[dat_phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_thanh_toan_trang_thai]    Script Date: 4/7/2026 3:20:35 PM ******/
CREATE NONCLUSTERED INDEX [idx_thanh_toan_trang_thai] ON [dbo].[thanh_toan]
(
	[trang_thai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__vai_tro__E59C937C0A93B2C5]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[vai_tro] ADD UNIQUE NONCLUSTERED 
(
	[ten_vai_tro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [uq_nguoi_dung_phong_yeu_thich]    Script Date: 4/7/2026 3:20:35 PM ******/
ALTER TABLE [dbo].[yeu_thich] ADD  CONSTRAINT [uq_nguoi_dung_phong_yeu_thich] UNIQUE NONCLUSTERED 
(
	[nguoi_dung_id] ASC,
	[phong_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[anh_co_so] ADD  DEFAULT ((0)) FOR [la_anh_chinh]
GO
ALTER TABLE [dbo].[anh_co_so] ADD  DEFAULT ((0)) FOR [thu_tu_hien_thi]
GO
ALTER TABLE [dbo].[anh_co_so] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[anh_phong] ADD  DEFAULT ((0)) FOR [la_anh_chinh]
GO
ALTER TABLE [dbo].[anh_phong] ADD  DEFAULT ((0)) FOR [thu_tu_hien_thi]
GO
ALTER TABLE [dbo].[anh_phong] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[co_so_luu_tru] ADD  DEFAULT (N'Dang hoat dong') FOR [trang_thai]
GO
ALTER TABLE [dbo].[co_so_luu_tru] ADD  DEFAULT ((0)) FOR [diem_trung_binh]
GO
ALTER TABLE [dbo].[co_so_luu_tru] ADD  DEFAULT ((0)) FOR [tong_so_danh_gia]
GO
ALTER TABLE [dbo].[co_so_luu_tru] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[co_so_luu_tru] ADD  DEFAULT (getdate()) FOR [ngay_cap_nhat]
GO
ALTER TABLE [dbo].[danh_gia] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT ((0)) FOR [phi_don_dep]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT ((0)) FOR [phi_dich_vu]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT ((0)) FOR [thue]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT (N'Pending') FOR [trang_thai]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[dat_phong] ADD  DEFAULT (getdate()) FOR [ngay_cap_nhat]
GO
ALTER TABLE [dbo].[doi_tac] ADD  DEFAULT (N'Dang hoat dong') FOR [trang_thai]
GO
ALTER TABLE [dbo].[doi_tac] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[doi_tac] ADD  DEFAULT (getdate()) FOR [ngay_cap_nhat]
GO
ALTER TABLE [dbo].[doi_tac] ADD  DEFAULT ((0.0)) FOR [so_du_vi]
GO
ALTER TABLE [dbo].[duyet_bai_dang] ADD  DEFAULT (getdate()) FOR [ngay_duyet]
GO
ALTER TABLE [dbo].[duyet_dat_phong] ADD  DEFAULT (getdate()) FOR [ngay_duyet]
GO
ALTER TABLE [dbo].[giao_dich_thanh_toan] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[lich_su_trang_thai_dat_phong] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[log_he_thong] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[nguoi_dung] ADD  DEFAULT (N'Hoat dong') FOR [trang_thai]
GO
ALTER TABLE [dbo].[nguoi_dung] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[nguoi_dung] ADD  DEFAULT (getdate()) FOR [ngay_cap_nhat]
GO
ALTER TABLE [dbo].[phien_dang_nhap] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[phong] ADD  DEFAULT ((1)) FOR [so_giuong]
GO
ALTER TABLE [dbo].[phong] ADD  DEFAULT (N'Draft') FOR [trang_thai]
GO
ALTER TABLE [dbo].[phong] ADD  DEFAULT ((0)) FOR [duoc_duyet]
GO
ALTER TABLE [dbo].[phong] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[phong] ADD  DEFAULT (getdate()) FOR [ngay_cap_nhat]
GO
ALTER TABLE [dbo].[thanh_toan] ADD  DEFAULT (N'Pending') FOR [trang_thai]
GO
ALTER TABLE [dbo].[thanh_toan] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[thanh_toan_doi_tac] ADD  DEFAULT ('pending') FOR [trang_thai]
GO
ALTER TABLE [dbo].[thanh_toan_doi_tac] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[thong_bao] ADD  DEFAULT ((0)) FOR [da_doc]
GO
ALTER TABLE [dbo].[thong_bao] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[yeu_thich] ADD  DEFAULT (getdate()) FOR [ngay_tao]
GO
ALTER TABLE [dbo].[anh_co_so]  WITH CHECK ADD  CONSTRAINT [fk_anh_co_so] FOREIGN KEY([co_so_id])
REFERENCES [dbo].[co_so_luu_tru] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[anh_co_so] CHECK CONSTRAINT [fk_anh_co_so]
GO
ALTER TABLE [dbo].[anh_phong]  WITH CHECK ADD  CONSTRAINT [fk_anh_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[anh_phong] CHECK CONSTRAINT [fk_anh_phong]
GO
ALTER TABLE [dbo].[co_so_luu_tru]  WITH CHECK ADD  CONSTRAINT [fk_co_so_doi_tac] FOREIGN KEY([doi_tac_id])
REFERENCES [dbo].[doi_tac] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[co_so_luu_tru] CHECK CONSTRAINT [fk_co_so_doi_tac]
GO
ALTER TABLE [dbo].[danh_gia]  WITH CHECK ADD  CONSTRAINT [fk_danh_gia_dat_phong] FOREIGN KEY([dat_phong_id])
REFERENCES [dbo].[dat_phong] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[danh_gia] CHECK CONSTRAINT [fk_danh_gia_dat_phong]
GO
ALTER TABLE [dbo].[danh_gia]  WITH CHECK ADD  CONSTRAINT [fk_danh_gia_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[danh_gia] CHECK CONSTRAINT [fk_danh_gia_nguoi_dung]
GO
ALTER TABLE [dbo].[danh_gia]  WITH CHECK ADD  CONSTRAINT [fk_danh_gia_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
GO
ALTER TABLE [dbo].[danh_gia] CHECK CONSTRAINT [fk_danh_gia_phong]
GO
ALTER TABLE [dbo].[dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_dat_phong_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[dat_phong] CHECK CONSTRAINT [fk_dat_phong_nguoi_dung]
GO
ALTER TABLE [dbo].[dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_dat_phong_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
GO
ALTER TABLE [dbo].[dat_phong] CHECK CONSTRAINT [fk_dat_phong_phong]
GO
ALTER TABLE [dbo].[doi_tac]  WITH CHECK ADD  CONSTRAINT [fk_doi_tac_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[doi_tac] CHECK CONSTRAINT [fk_doi_tac_nguoi_dung]
GO
ALTER TABLE [dbo].[duyet_bai_dang]  WITH CHECK ADD  CONSTRAINT [fk_duyet_bai_dang_admin] FOREIGN KEY([admin_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[duyet_bai_dang] CHECK CONSTRAINT [fk_duyet_bai_dang_admin]
GO
ALTER TABLE [dbo].[duyet_bai_dang]  WITH CHECK ADD  CONSTRAINT [fk_duyet_bai_dang_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
GO
ALTER TABLE [dbo].[duyet_bai_dang] CHECK CONSTRAINT [fk_duyet_bai_dang_phong]
GO
ALTER TABLE [dbo].[duyet_dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_duyet_dat_phong_admin] FOREIGN KEY([admin_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[duyet_dat_phong] CHECK CONSTRAINT [fk_duyet_dat_phong_admin]
GO
ALTER TABLE [dbo].[duyet_dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_duyet_dat_phong_dat_phong] FOREIGN KEY([dat_phong_id])
REFERENCES [dbo].[dat_phong] ([id])
GO
ALTER TABLE [dbo].[duyet_dat_phong] CHECK CONSTRAINT [fk_duyet_dat_phong_dat_phong]
GO
ALTER TABLE [dbo].[giao_dich_thanh_toan]  WITH CHECK ADD  CONSTRAINT [fk_gdtt_thanh_toan] FOREIGN KEY([thanh_toan_id])
REFERENCES [dbo].[thanh_toan] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[giao_dich_thanh_toan] CHECK CONSTRAINT [fk_gdtt_thanh_toan]
GO
ALTER TABLE [dbo].[khach_dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_khach_dat_phong] FOREIGN KEY([dat_phong_id])
REFERENCES [dbo].[dat_phong] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[khach_dat_phong] CHECK CONSTRAINT [fk_khach_dat_phong]
GO
ALTER TABLE [dbo].[lich_su_trang_thai_dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_lstt_dat_phong] FOREIGN KEY([dat_phong_id])
REFERENCES [dbo].[dat_phong] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[lich_su_trang_thai_dat_phong] CHECK CONSTRAINT [fk_lstt_dat_phong]
GO
ALTER TABLE [dbo].[lich_su_trang_thai_dat_phong]  WITH CHECK ADD  CONSTRAINT [fk_lstt_nguoi_dung] FOREIGN KEY([nguoi_thay_doi_id])
REFERENCES [dbo].[nguoi_dung] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[lich_su_trang_thai_dat_phong] CHECK CONSTRAINT [fk_lstt_nguoi_dung]
GO
ALTER TABLE [dbo].[log_he_thong]  WITH CHECK ADD  CONSTRAINT [fk_log_he_thong_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[log_he_thong] CHECK CONSTRAINT [fk_log_he_thong_nguoi_dung]
GO
ALTER TABLE [dbo].[nguoi_dung_vai_tro]  WITH CHECK ADD  CONSTRAINT [fk_ndvt_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[nguoi_dung_vai_tro] CHECK CONSTRAINT [fk_ndvt_nguoi_dung]
GO
ALTER TABLE [dbo].[nguoi_dung_vai_tro]  WITH CHECK ADD  CONSTRAINT [fk_ndvt_vai_tro] FOREIGN KEY([vai_tro_id])
REFERENCES [dbo].[vai_tro] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[nguoi_dung_vai_tro] CHECK CONSTRAINT [fk_ndvt_vai_tro]
GO
ALTER TABLE [dbo].[phien_dang_nhap]  WITH CHECK ADD  CONSTRAINT [fk_phien_dang_nhap_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[phien_dang_nhap] CHECK CONSTRAINT [fk_phien_dang_nhap_nguoi_dung]
GO
ALTER TABLE [dbo].[phong]  WITH CHECK ADD  CONSTRAINT [fk_phong_co_so] FOREIGN KEY([co_so_id])
REFERENCES [dbo].[co_so_luu_tru] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[phong] CHECK CONSTRAINT [fk_phong_co_so]
GO
ALTER TABLE [dbo].[phong_tien_ich]  WITH CHECK ADD  CONSTRAINT [fk_pti_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[phong_tien_ich] CHECK CONSTRAINT [fk_pti_phong]
GO
ALTER TABLE [dbo].[phong_tien_ich]  WITH CHECK ADD  CONSTRAINT [fk_pti_tien_ich] FOREIGN KEY([tien_ich_id])
REFERENCES [dbo].[tien_ich] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[phong_tien_ich] CHECK CONSTRAINT [fk_pti_tien_ich]
GO
ALTER TABLE [dbo].[tai_khoan_ngan_hang_doi_tac]  WITH CHECK ADD  CONSTRAINT [FK_BankAcc_Partner] FOREIGN KEY([doi_tac_id])
REFERENCES [dbo].[doi_tac] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tai_khoan_ngan_hang_doi_tac] CHECK CONSTRAINT [FK_BankAcc_Partner]
GO
ALTER TABLE [dbo].[thanh_toan]  WITH CHECK ADD  CONSTRAINT [fk_thanh_toan_dat_phong] FOREIGN KEY([dat_phong_id])
REFERENCES [dbo].[dat_phong] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[thanh_toan] CHECK CONSTRAINT [fk_thanh_toan_dat_phong]
GO
ALTER TABLE [dbo].[thanh_toan_doi_tac]  WITH CHECK ADD  CONSTRAINT [FK_thanh_toan_doi_tac_doi_tac] FOREIGN KEY([doi_tac_id])
REFERENCES [dbo].[doi_tac] ([id])
GO
ALTER TABLE [dbo].[thanh_toan_doi_tac] CHECK CONSTRAINT [FK_thanh_toan_doi_tac_doi_tac]
GO
ALTER TABLE [dbo].[thong_bao]  WITH CHECK ADD  CONSTRAINT [fk_thong_bao_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[thong_bao] CHECK CONSTRAINT [fk_thong_bao_nguoi_dung]
GO
ALTER TABLE [dbo].[yeu_thich]  WITH CHECK ADD  CONSTRAINT [fk_yeu_thich_nguoi_dung] FOREIGN KEY([nguoi_dung_id])
REFERENCES [dbo].[nguoi_dung] ([id])
GO
ALTER TABLE [dbo].[yeu_thich] CHECK CONSTRAINT [fk_yeu_thich_nguoi_dung]
GO
ALTER TABLE [dbo].[yeu_thich]  WITH CHECK ADD  CONSTRAINT [fk_yeu_thich_phong] FOREIGN KEY([phong_id])
REFERENCES [dbo].[phong] ([id])
GO
ALTER TABLE [dbo].[yeu_thich] CHECK CONSTRAINT [fk_yeu_thich_phong]
GO
ALTER TABLE [dbo].[danh_gia]  WITH CHECK ADD CHECK  (([so_sao]>=(1) AND [so_sao]<=(5)))
GO
/****** Object:  StoredProcedure [dbo].[sp_DangNhap]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- PHAN 3: STORED PROCEDURES (dung voi C# .NET + BCrypt.Net)
-- ============================================================

-- SP1: Dang nhap
CREATE   PROCEDURE [dbo].[sp_DangNhap]
    @ten_dang_nhap  NVARCHAR(50),
    @mat_khau_hash  NVARCHAR(255) OUTPUT,
    @nguoi_dung_id  INT           OUTPUT,
    @ho_ten         NVARCHAR(100) OUTPUT,
    @anh_dai_dien   NVARCHAR(MAX) OUTPUT,
    @trang_thai     NVARCHAR(50)  OUTPUT,
    @nhom_vai_tro   NVARCHAR(50)  OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    -- Tra ve hash de C# tu verify bang BCrypt.Net.BCrypt.Verify()
    SELECT TOP 1
        @mat_khau_hash  = nd.mat_khau,
        @nguoi_dung_id  = nd.id,
        @ho_ten         = nd.ho_ten,
        @anh_dai_dien   = nd.anh_dai_dien,
        @trang_thai     = nd.trang_thai,
        @nhom_vai_tro   = vt.nhom_vai_tro
    FROM nguoi_dung nd
    LEFT JOIN nguoi_dung_vai_tro ndvt ON ndvt.nguoi_dung_id = nd.id
    LEFT JOIN vai_tro vt ON vt.id = ndvt.vai_tro_id
    WHERE nd.ten_dang_nhap = @ten_dang_nhap
      AND nd.trang_thai = N'Hoat dong';

    -- Cap nhat lan cuoi hoat dong
    IF @nguoi_dung_id IS NOT NULL
        UPDATE nguoi_dung
        SET lan_cuoi_hoat_dong = GETDATE(), ngay_cap_nhat = GETDATE()
        WHERE id = @nguoi_dung_id;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_DuyetDatPhong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP4: Cap nhat trang thai dat phong (Admin)
CREATE   PROCEDURE [dbo].[sp_DuyetDatPhong]
    @dat_phong_id   INT,
    @admin_id       INT,
    @trang_thai_moi NVARCHAR(50),   -- 'Confirmed' | 'Cancelled'
    @ghi_chu        NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @trang_thai_cu NVARCHAR(50);
        DECLARE @nguoi_dung_id INT;

        SELECT @trang_thai_cu  = trang_thai,
               @nguoi_dung_id  = nguoi_dung_id
        FROM dat_phong WHERE id = @dat_phong_id;

        UPDATE dat_phong
        SET trang_thai = @trang_thai_moi, ngay_cap_nhat = GETDATE()
        WHERE id = @dat_phong_id;

        INSERT INTO lich_su_trang_thai_dat_phong (dat_phong_id, trang_thai_cu, trang_thai_moi, nguoi_thay_doi_id, ghi_chu)
        VALUES (@dat_phong_id, @trang_thai_cu, @trang_thai_moi, @admin_id, @ghi_chu);

        INSERT INTO duyet_dat_phong (dat_phong_id, admin_id, trang_thai, ghi_chu)
        VALUES (@dat_phong_id, @admin_id,
            CASE @trang_thai_moi
                WHEN N'Confirmed' THEN N'Da duyet'
                WHEN N'Cancelled' THEN N'Tu choi'
                ELSE N'Cho xu ly'
            END, @ghi_chu);

        -- Thong bao cho khach
        DECLARE @msg NVARCHAR(200) =
            CASE @trang_thai_moi
                WHEN N'Confirmed' THEN N'Dat phong #' + CAST(@dat_phong_id AS NVARCHAR) + N' da duoc xac nhan!'
                WHEN N'Cancelled' THEN N'Dat phong #' + CAST(@dat_phong_id AS NVARCHAR) + N' da bi huy.'
                ELSE N'Trang thai dat phong #' + CAST(@dat_phong_id AS NVARCHAR) + N' vua duoc cap nhat.'
            END;

        INSERT INTO thong_bao (nguoi_dung_id, tieu_de, noi_dung, loai)
        VALUES (@nguoi_dung_id, N'Cap nhat dat phong', @msg, N'Dat phong');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_LayAdminDashboard]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP6: Lay admin dashboard summary
CREATE   PROCEDURE [dbo].[sp_LayAdminDashboard]
AS
BEGIN
    SET NOCOUNT ON;

    -- User stats theo nhom_vai_tro (khop voi UI: Admin | Business Owner | Customer)
    SELECT vt.nhom_vai_tro, nd.trang_thai, COUNT(*) AS so_luong
    FROM nguoi_dung nd
    LEFT JOIN nguoi_dung_vai_tro ndvt ON ndvt.nguoi_dung_id = nd.id
    LEFT JOIN vai_tro vt ON vt.id = ndvt.vai_tro_id
    GROUP BY vt.nhom_vai_tro, nd.trang_thai;

    -- Booking stats
    SELECT trang_thai, COUNT(*) AS so_luong, ISNULL(SUM(tong_tien),0) AS tong_tien
    FROM dat_phong
    GROUP BY trang_thai;

    -- Revenue stats
    SELECT phuong_thuc, trang_thai, COUNT(*) AS so_giao_dich, ISNULL(SUM(so_tien),0) AS tong_tien
    FROM thanh_toan
    GROUP BY phuong_thuc, trang_thai;

    -- Phong stats theo trang_thai (Online | Draft | Pending Activation | Archived)
    SELECT trang_thai, COUNT(*) AS so_luong
    FROM phong
    GROUP BY trang_thai;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_LayDashboardBusiness]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP5: Lay dashboard business (cho trang Business Dashboard)
CREATE   PROCEDURE [dbo].[sp_LayDashboardBusiness]
    @doi_tac_id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Tong quan
    SELECT
        COUNT(DISTINCT p.id)                                                AS tong_phong,
        COUNT(DISTINCT CASE WHEN p.trang_thai = N'Online' THEN p.id END)   AS phong_online,
        COUNT(DISTINCT dp.id)                                               AS tong_booking,
        COUNT(DISTINCT CASE WHEN dp.trang_thai = N'Confirmed' THEN dp.id END) AS booking_confirmed,
        ISNULL(SUM(CASE WHEN tt.trang_thai = N'Completed' THEN tt.so_tien END), 0) AS tong_doanh_thu,
        ISNULL(AVG(CAST(cs.diem_trung_binh AS FLOAT)), 0)                  AS diem_tb
    FROM doi_tac dt
    INNER JOIN co_so_luu_tru cs ON cs.doi_tac_id = dt.id
    INNER JOIN phong p ON p.co_so_id = cs.id
    LEFT JOIN dat_phong dp ON dp.phong_id = p.id
    LEFT JOIN thanh_toan tt ON tt.dat_phong_id = dp.id
    WHERE dt.id = @doi_tac_id;

    -- 5 booking gan nhat
    SELECT TOP 5
        dp.id           AS booking_id,
        p.ten_phong,
        cs.ten_co_so,
        nd.ho_ten       AS ten_khach,
        dp.ngay_nhan_phong,
        dp.ngay_tra_phong,
        dp.tong_tien,
        dp.trang_thai
    FROM dat_phong dp
    INNER JOIN phong p ON p.id = dp.phong_id
    INNER JOIN co_so_luu_tru cs ON cs.id = p.co_so_id
    INNER JOIN doi_tac dt ON dt.id = cs.doi_tac_id
    INNER JOIN nguoi_dung nd ON nd.id = dp.nguoi_dung_id
    WHERE dt.id = @doi_tac_id
    ORDER BY dp.ngay_tao DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_LayPhongKhaDung]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP2: Lay danh sach phong kha dung (cho trang Discovery)
CREATE   PROCEDURE [dbo].[sp_LayPhongKhaDung]
    @ngay_nhan      DATETIME,
    @ngay_tra       DATETIME,
    @tinh_thanh     NVARCHAR(100) = NULL,
    @loai_phong     NVARCHAR(100) = NULL,
    @gia_min        DECIMAL(18,2) = NULL,
    @gia_max        DECIMAL(18,2) = NULL,
    @so_nguoi       INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.id,
        p.ten_phong,
        p.loai_phong,
        p.suc_chua,
        p.so_giuong,
        p.gia_co_ban,
        p.dien_tich,
        p.mo_ta,
        cs.ten_co_so,
        cs.tinh_thanh,
        cs.quan_huyen,
        cs.diem_trung_binh,
        cs.tong_so_danh_gia,
        (SELECT TOP 1 url_anh FROM anh_phong WHERE phong_id = p.id AND la_anh_chinh = 1) AS anh_chinh
    FROM phong p
    INNER JOIN co_so_luu_tru cs ON cs.id = p.co_so_id
    WHERE p.trang_thai     = N'Online'
      AND p.duoc_duyet     = 1
      AND cs.trang_thai    = N'Dang hoat dong'
      AND (@tinh_thanh IS NULL OR cs.tinh_thanh  = @tinh_thanh)
      AND (@loai_phong IS NULL OR p.loai_phong    = @loai_phong)
      AND (@gia_min    IS NULL OR p.gia_co_ban   >= @gia_min)
      AND (@gia_max    IS NULL OR p.gia_co_ban   <= @gia_max)
      AND (@so_nguoi   IS NULL OR p.suc_chua     >= @so_nguoi)
      AND p.id NOT IN (
          SELECT DISTINCT phong_id FROM dat_phong
          WHERE trang_thai IN (N'Confirmed', N'Pending')
            AND NOT (@ngay_tra  <= ngay_nhan_phong OR @ngay_nhan >= ngay_tra_phong)
      )
    ORDER BY cs.diem_trung_binh DESC, p.gia_co_ban ASC;
END;

GO
/****** Object:  StoredProcedure [dbo].[sp_TaoDatPhong]    Script Date: 4/7/2026 3:20:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- SP3: Tao dat phong moi
CREATE   PROCEDURE [dbo].[sp_TaoDatPhong]
    @nguoi_dung_id      INT,
    @phong_id           INT,
    @ngay_nhan          DATETIME,
    @ngay_tra           DATETIME,
    @so_nguoi           INT,
    @gio_nhan           TIME        = NULL,
    @gio_tra            TIME        = NULL,
    @ghi_chu            NVARCHAR(MAX) = NULL,
    @dat_phong_id       INT         OUTPUT,
    @tong_tien          DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @gia_co_ban   DECIMAL(18,2);
        DECLARE @so_dem       INT;
        DECLARE @phi_don_dep  DECIMAL(18,2) = 150000;
        DECLARE @phi_dich_vu  DECIMAL(18,2);
        DECLARE @thue         DECIMAL(18,2);
        DECLARE @dat_coc      DECIMAL(18,2);

        -- Kiem tra phong con trong
        IF EXISTS (
            SELECT 1 FROM dat_phong
            WHERE phong_id = @phong_id
              AND trang_thai IN (N'Confirmed', N'Pending')
              AND NOT (@ngay_tra <= ngay_nhan_phong OR @ngay_nhan >= ngay_tra_phong)
        )
        BEGIN
            RAISERROR(N'Phong da duoc dat trong khoang thoi gian nay.', 16, 1);
            RETURN;
        END;

        SELECT @gia_co_ban = gia_co_ban FROM phong WHERE id = @phong_id;
        SET @so_dem       = DATEDIFF(DAY, @ngay_nhan, @ngay_tra);
        SET @phi_dich_vu  = @gia_co_ban * @so_dem * 0.10;
        SET @thue         = (@gia_co_ban * @so_dem + @phi_dich_vu + @phi_don_dep) * 0.10;
        SET @tong_tien    = @gia_co_ban * @so_dem + @phi_don_dep + @phi_dich_vu + @thue;
        SET @dat_coc      = @tong_tien * 0.30;

        INSERT INTO dat_phong (nguoi_dung_id, phong_id, ngay_nhan_phong, ngay_tra_phong, so_dem, so_nguoi,
            gio_nhan_phong, gio_tra_phong, gia_moi_dem, phi_don_dep, phi_dich_vu, thue, dat_coc, tong_tien, trang_thai, ghi_chu)
        VALUES (@nguoi_dung_id, @phong_id, @ngay_nhan, @ngay_tra, @so_dem, @so_nguoi,
            @gio_nhan, @gio_tra, @gia_co_ban, @phi_don_dep, @phi_dich_vu, @thue, @dat_coc, @tong_tien, N'Pending', @ghi_chu);

        SET @dat_phong_id = SCOPE_IDENTITY();

        -- Ghi lich su trang thai
        INSERT INTO lich_su_trang_thai_dat_phong (dat_phong_id, trang_thai_cu, trang_thai_moi, nguoi_thay_doi_id, ghi_chu)
        VALUES (@dat_phong_id, NULL, N'Pending', @nguoi_dung_id, N'Khach dat phong moi.');

        -- Tao thong bao cho nguoi dung
        INSERT INTO thong_bao (nguoi_dung_id, tieu_de, noi_dung, loai)
        VALUES (@nguoi_dung_id, N'Dat phong thanh cong',
            N'Dat phong #' + CAST(@dat_phong_id AS NVARCHAR) + N' dang cho xac nhan.', N'Dat phong');

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

GO
USE [master]
GO
ALTER DATABASE [QL_NhaNghi] SET  READ_WRITE 
GO
