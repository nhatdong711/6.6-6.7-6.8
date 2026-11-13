# BÀI 6.8: HỆ THỐNG QUẢN LÝ BÀN BIDA & DỊCH VỤ GIẢI TRÍ (BILLIARDS CLUB MANAGEMENT SYSTEM)

> Bài tập 6.8 — Môn Lập trình .NET (D19QTANM1)

## MỤC 1: THÔNG TIN SINH VIÊN

- **Họ và tên sinh viên:** Trần Nhật Đông
- **Mã sinh viên:** 24810320177
- **Lớp / Khóa:** D19QTANM1

## MỤC 2: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

1. **Màn hình Chính / Tổng quan hệ thống:**
![Màn hình chính](images/01_main_screen.png)

2. **Màn hình Quản lý Danh mục (CRUD - Thêm/Sửa/Xóa):**
![Màn hình CRUD](images/02_crud_screen.png)

3. **Màn hình Thực thi Nghiệp vụ chính (Mở bàn, gọi dịch vụ, tính giờ & thanh toán):**
![Màn hình Nghiệp vụ](images/03_transaction_screen.png)

4. **Màn hình Báo cáo - Thống kê:**
![Màn hình Thống kê](images/04_report_screen.png)

5. **Minh họa An toàn Dữ liệu (Phòng chống SQL Injection / Báo lỗi Transaction Rollback):**
![Minh họa Xử lý lỗi](images/05_error_handling.png)

## MỤC 3: HƯỚNG DẪN CÀI ĐẶT & CHẠY

**Yêu cầu:** Visual Studio 2022 trở lên (workload *.NET desktop development*), .NET 8 SDK, SQL Server (Developer/Express/LocalDB).

1. Clone repository và mở file `BilliardsManagement.sln` bằng Visual Studio.
2. **Tạo CSDL:** ở lần chạy đầu, chương trình tự tìm SQL Server (`.`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`) và chạy `Database/ScriptDB.sql`. Cũng có thể chạy thủ công script này trong SSMS.
3. Nếu cần, sửa chuỗi kết nối trong `BilliardsManagement/App.config`.
4. Nhấn **F5** để chạy (NuGet tự khôi phục `Microsoft.Data.SqlClient`, `System.Configuration.ConfigurationManager`).

## MỤC 4: KIẾN TRÚC 3-TIER

```
Bai6.8_Billiards/
├── Database/
│   └── ScriptDB.sql          # Script khởi tạo CSDL SQL Server và chèn dữ liệu mẫu
├── BilliardsManagement/
│   ├── DTO/                  # BanBidaDTO, MenuDichVuDTO, HoaDonDTO
│   ├── DAL/                  # DBConnect, BanBidaDAL, MenuDichVuDAL (SqlDataAdapter), HoaDonDAL
│   ├── BUS/                  # BanBidaBUS, MenuDichVuBUS, HoaDonBUS
│   ├── GUI/                  # FrmMain (Sơ đồ bàn, Quản lý menu, Báo cáo doanh thu)
│   ├── App.config / Program.cs
├── images/                   # Ảnh chụp màn hình
├── README.md
└── BilliardsManagement.sln
```

| Tầng | Vai trò |
|------|---------|
| **DTO** | Đối tượng truyền dữ liệu giữa các tầng |
| **DAL** | Truy vấn ADO.NET: SqlConnection, SqlCommand + SqlParameter, SqlDataReader, SqlDataAdapter, SqlTransaction |
| **BUS** | Kiểm tra ràng buộc, tính toán nghiệp vụ |
| **GUI** | Giao diện WinForms, chỉ gọi tầng BUS |

## MỤC 5: CHỨC NĂNG ĐÃ HOÀN THÀNH
- [x] CSDL `BilliardsDB` 4 bảng: BanBida, MenuDichVu, HoaDon, ChiTietHoaDon (+ dữ liệu mẫu)
- [x] **Sơ đồ bàn trực quan**: Button sinh động từ CSDL, bàn "Trống" màu xanh lá, "Đang chơi" màu cam
- [x] Mở bàn: tạo HoaDon với `GioBatDau = GETDATE()`, bàn chuyển sang "Đang chơi"
- [x] **Mô hình ngắt kết nối**: dịch vụ bàn đã gọi nạp bằng `SqlDataAdapter` + `DataTable` lên DataGridView; thêm món mới hoặc sửa trực tiếp số lượng trên lưới, bấm Lưu để `adapter.Update()`
- [x] Quản lý menu dịch vụ trên DataTable (thêm/sửa/xóa rồi đồng bộ bằng adapter)
- [x] Đồng hồ thời gian chơi và tạm tính theo thời gian thực
- [x] Thanh toán: số giờ = GioKetThuc − GioBatDau, **làm tròn theo block 15 phút**; `TienGioChoi = Số giờ × GiaTheoGio`
- [x] **SqlTransaction**: (1) cập nhật GioKetThuc, TienGioChoi, TienDichVu, TrangThai = 1 → (2) trừ TonKho (**bỏ qua dịch vụ thuê gậy/găng** có TonKho = NULL) → (3) bàn về "Trống" → (4) `trans.Commit()`
- [x] Báo cáo doanh thu theo ngày, theo loại bàn, lịch sử hóa đơn

> Gợi ý chụp ảnh lỗi (ảnh 05): gọi 5 gói *Thuốc lá Vinataba* (kho còn 2) → hệ thống chặn và báo lỗi.

