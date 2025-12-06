# 🎓 Hệ Thống Quản Lý Giảng Viên - WinForms Frontend

Ứng dụng Desktop Windows Forms kết nối với ASP.NET Web API Backend để quản lý giảng viên, phân công giảng dạy.

## 📋 Yêu Cầu Hệ Thống

- Windows 10/11
- .NET 8.0 SDK
- Visual Studio 2022 (khuyến nghị)

## 🚀 Cài Đặt & Chạy

### 1. Clone hoặc Download Project

```bash
# Copy thư mục TeacherScheduleFrontend về máy
```

### 2. Cấu Hình API Endpoint

Mở file `appsettings.json` và cập nhật URL Backend:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-backend-url.railway.app/api"
  }
}
```

### 3. Build và Chạy

**Cách 1: Dùng Visual Studio**
1. Mở file `TeacherScheduleFrontend.csproj`
2. Nhấn F5 hoặc Ctrl+F5 để chạy

**Cách 2: Dùng Command Line**
```bash
cd TeacherScheduleFrontend
dotnet restore
dotnet build
dotnet run
```

## 📁 Cấu Trúc Project

```
TeacherScheduleFrontend/
├── Forms/                    # Các màn hình giao diện
│   ├── FormLogin.cs         # Đăng nhập
│   ├── FormMain.cs          # Dashboard chính
│   ├── FormKhoa.cs          # Quản lý Khoa
│   ├── FormBoMon.cs         # Quản lý Bộ môn
│   ├── FormGiangVien.cs     # Quản lý Giảng viên
│   ├── FormMonHoc.cs        # Quản lý Môn học
│   ├── FormLop.cs           # Quản lý Lớp
│   ├── FormPhanCong.cs      # Phân công giảng dạy
│   ├── FormTaiKhoan.cs      # Quản lý Tài khoản
│   ├── FormChangePassword.cs # Đổi mật khẩu
│   ├── FormThongKe.cs       # Thống kê
│   └── FormDinhMuc.cs       # Quản lý Định mức
├── Models/
│   └── Models.cs            # Các class đại diện dữ liệu
├── Services/
│   └── ApiService.cs        # Service gọi API
├── Program.cs               # Entry point
├── appsettings.json         # Cấu hình
└── TeacherScheduleFrontend.csproj
```

## 🔐 Phân Quyền

| Chức vụ | Mã | Quyền hạn |
|---------|-----|-----------|
| Cán bộ quản lý | CQC | Full access |
| Trưởng khoa | TK | Quản lý đơn vị, nhân sự |
| Trưởng bộ môn | TBM | Xem và phân công |
| Giảng viên | GV | Xem thông tin cá nhân |

## 📱 Các Chức Năng Chính

### 1. Đăng Nhập
- Nhập tên đăng nhập và mật khẩu
- Hệ thống xác thực qua API

### 2. Dashboard
- Hiển thị tổng quan: số giảng viên, khoa, môn học, lớp
- Truy cập nhanh các chức năng

### 3. Quản Lý Khoa/Bộ Môn
- Thêm, sửa, xóa khoa
- Thêm, sửa, xóa bộ môn (liên kết với khoa)

### 4. Quản Lý Giảng Viên
- CRUD giảng viên
- Gắn giảng viên với khoa/bộ môn

### 5. Quản Lý Môn Học & Lớp
- Quản lý danh sách môn học
- Quản lý danh sách lớp

### 6. Phân Công Giảng Dạy
- Phân công giảng viên dạy môn/lớp
- Chọn thứ, tiết, phòng học
- Theo học kỳ

### 7. Quản Lý Tài Khoản
- Tạo tài khoản người dùng
- Gán chức vụ và quyền

### 8. Thống Kê
- Thống kê giờ giảng theo giảng viên
- Thống kê theo khoa
- Tổng quan hệ thống
- Xuất Excel

## ⚙️ Cấu Hình

### appsettings.json

```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-api-url/api"
  }
}
```

## 🎨 Giao Diện

- **Theme màu xanh dương** chủ đạo
- Sidebar điều hướng bên trái
- Menu bar đầy đủ chức năng
- Form nhập liệu + DataGridView hiển thị
- Responsive với các kích thước màn hình

## 📦 Dependencies

- **Newtonsoft.Json** (13.0.3): Xử lý JSON
- **Microsoft.Extensions.Http** (8.0.0): HttpClient factory

## 🔧 Troubleshooting

### Lỗi kết nối API
1. Kiểm tra URL trong `appsettings.json`
2. Đảm bảo backend đang chạy
3. Kiểm tra kết nối mạng

### Lỗi đăng nhập
1. Kiểm tra tài khoản/mật khẩu
2. Đảm bảo tài khoản đã được tạo trong database

## 📝 Ghi Chú Phát Triển

- Sử dụng async/await cho tất cả API calls
- Token JWT được lưu trong ApiService
- Forms được load động vào content panel
- Error handling với try-catch và MessageBox

## 👨‍💻 Tác Giả

[Tên của bạn]  
Email: [your-email@example.com]

## 📄 License

© 2024 - All Rights Reserved
