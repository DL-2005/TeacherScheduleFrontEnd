namespace TeacherScheduleFrontend.Models
{
    // ==================== AUTH MODELS ====================
    public class LoginRequest
    {
        public string MaTK { get; set; } = "";
        public string MatKhau { get; set; } = "";
    }

    public class LoginResponse
    {
        public string MaTK { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string? MaGV { get; set; }
        public string? TenGV { get; set; }
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }

    public class RegisterRequest
    {
        public string MaTK { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public string XacNhanMatKhau { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string? MaGV { get; set; }
        public string? MaKhoa { get; set; }
        public string? MaBM { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string MatKhauCu { get; set; } = "";
        public string MatKhauMoi { get; set; } = "";
        public string XacNhanMatKhauMoi { get; set; } = "";
    }

    // ==================== KHOA ====================
    public class Khoa
    {
        public string MaKhoa { get; set; } = "";
        public string TenKhoa { get; set; } = "";
        public string? Email { get; set; }
        public string? DienThoai { get; set; }
    }

    // ==================== BỘ MÔN ====================
    public class BoMon
    {
        public string MaBM { get; set; } = "";
        public string TenBM { get; set; } = "";
        public string? MaKhoa { get; set; }
        public Khoa? Khoa { get; set; }
        public string? MoTa { get; set; }
    }

    // ==================== GIẢNG VIÊN ====================
    public class GiangVien
    {
        public string MaGV { get; set; } = "";
        public string TenGV { get; set; } = "";
        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? MaKhoa { get; set; }
        public Khoa? Khoa { get; set; }
        public string? MaBM { get; set; }
        public BoMon? BoMon { get; set; }
    }

    // ==================== MÔN HỌC ====================
    public class MonHoc
    {
        public string MaMH { get; set; } = "";
        public string TenMH { get; set; } = "";
        public int SoTinChi { get; set; }
        public string? HeDaoTao { get; set; }
    }

    // ==================== LỚP ====================
    public class Lop
    {
        public string MaLop { get; set; } = "";
        public int SiSo { get; set; }
        public string? MaKhoa { get; set; }
        public Khoa? Khoa { get; set; }
        public string? Nganh { get; set; }
        public string? NamHoc { get; set; }
    }

    // ==================== PHÂN CÔNG ====================
    public class PhanCong
    {
        public int Id { get; set; }
        public string MaGV { get; set; } = "";
        public GiangVien? GiangVien { get; set; }
        public string MaMH { get; set; } = "";
        public MonHoc? MonHoc { get; set; }
        public string MaLop { get; set; } = "";
        public Lop? Lop { get; set; }
        public int TietBatDau { get; set; }
        public int SoTiet { get; set; }
        public int Thu { get; set; }
        public string? ThoiGianHoc { get; set; }
        public string? PhongHoc { get; set; }
        public string? GhiChu { get; set; }
    }

    // ==================== TÀI KHOẢN ====================
    public class TaiKhoan
    {
        public string MaTK { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string? MaGV { get; set; }
        public GiangVien? GiangVien { get; set; }
        public string? MaKhoa { get; set; }
        public Khoa? Khoa { get; set; }
        public string? MaBM { get; set; }
        public BoMon? BoMon { get; set; }
    }

    // ==================== NGHIÊN CỨU KHOA HỌC ====================
    public class NghienCuuKhoaHoc
    {
        public int Id { get; set; }
        public string MaGV { get; set; } = "";
        public GiangVien? GiangVien { get; set; }
        public string TenDeTai { get; set; } = "";
        public string TheLoai { get; set; } = "";
        public string? VaiTro { get; set; }
        public int GioNCKH { get; set; }
        public string? NamHoc { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string? TrangThai { get; set; }
        public string? MoTa { get; set; }
        public string? FileMinhChung { get; set; }
    }

    // ==================== BỒI DƯỠNG ====================
    public class BoiDuong
    {
        public int Id { get; set; }
        public string MaGV { get; set; } = "";
        public GiangVien? GiangVien { get; set; }
        public string NoiDung { get; set; } = "";
        public string? ChiTiet { get; set; }
        public int GioBoiDuong { get; set; }
        public string? NamHoc { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string? GhiChu { get; set; }
    }

    // ==================== NHIỆM VỤ KHÁC ====================
    public class NhiemVuKhac
    {
        public int Id { get; set; }
        public string MaGV { get; set; } = "";
        public GiangVien? GiangVien { get; set; }
        public string CongViec { get; set; } = "";
        public string? ChiTiet { get; set; }
        public int SoGio { get; set; }
        public string? NamHoc { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string? GhiChu { get; set; }
    }

    // ==================== ĐỊNH MỨC ====================
    public class DinhMuc
    {
        public int Id { get; set; }
        public string MaDM { get; set; } = ""; // Mã Định Mức
        public string ChucVu { get; set; } = ""; // Chức vụ áp dụng
        public int GioChuan { get; set; } // Giờ chuẩn
        public int GioToiThieu { get; set; } // Giờ tối thiểu
        public int GioToiDa { get; set; } // Giờ tối đa
        public string? MoTa { get; set; } // Mô tả

        // Các thuộc tính liên quan đến Dinh Muc Giảng Dạy/NCKH/Bồi dưỡng (nếu có)
        // đã bị xóa hoặc thay đổi theo cấu trúc mới của form.
    }

    // ==================== API RESPONSE ====================
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class ErrorResponse
    {
        public string? Message { get; set; }
    }
}
