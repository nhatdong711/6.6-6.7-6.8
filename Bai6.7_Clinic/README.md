# BÀI 6.7: HỆ THỐNG QUẢN LÝ PHÒNG KHÁM & KÊ ĐƠN THUỐC (CLINIC & PHARMACY MANAGEMENT SYSTEM)

> Bài tập 6.7 — Môn Lập trình .NET (D19QTANM1)

## MỤC 1: THÔNG TIN SINH VIÊN

- **Họ và tên sinh viên:** Trần Nhật Đông
- **Mã sinh viên:** 24810320177
- **Lớp / Khóa:** D19QTANM1

## MỤC 2: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

1. **Màn hình Chính / Tổng quan hệ thống:**
![Màn hình chính](images/01_main_screen.png)

2. **Màn hình Quản lý Danh mục (CRUD - Thêm/Sửa/Xóa):**
![Màn hình CRUD](images/02_crud_screen.png)

3. **Màn hình Thực thi Nghiệp vụ chính (Kê đơn thuốc & xuất kho bằng SqlTransaction):**
![Màn hình Nghiệp vụ](images/03_transaction_screen.png)

4. **Màn hình Báo cáo - Thống kê:**
![Màn hình Thống kê](images/04_report_screen.png)

5. **Minh họa An toàn Dữ liệu (Phòng chống SQL Injection / Báo lỗi Transaction Rollback):**
![Minh họa Xử lý lỗi](images/05_error_handling.png)

## MỤC 3: HƯỚNG DẪN CÀI ĐẶT & CHẠY

**Yêu cầu:** Visual Studio 2022 trở lên (workload *.NET desktop development*), .NET 8 SDK, SQL Server (Developer/Express/LocalDB).

1. Clone repository và mở file `ClinicManagement.sln` bằng Visual Studio.
2. **Tạo CSDL:** ở lần chạy đầu, chương trình tự tìm SQL Server (`.`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`) và chạy `Database/ScriptDB.sql`. Cũng có thể chạy thủ công script này trong SSMS.
3. Nếu cần, sửa chuỗi kết nối trong `ClinicManagement/App.config`.
4. Nhấn **F5** để chạy (NuGet tự khôi phục `Microsoft.Data.SqlClient`, `System.Configuration.ConfigurationManager`).

## MỤC 4: KIẾN TRÚC 3-TIER

```
Bai6.7_Clinic/
├── Database/
│   └── ScriptDB.sql          # Script khởi tạo CSDL SQL Server và chèn dữ liệu mẫu
├── ClinicManagement/
│   ├── DTO/                  # BenhNhanDTO, HoSoKhamDTO, ThuocDTO, DonThuocDTO / ChiTietDonThuocDTO
│   ├── DAL/                  # DBConnect, BenhNhanDAL, ThuocDAL, DonThuocDAL
│   ├── BUS/                  # BenhNhanBUS, ThuocBUS, DonThuocBUS
│   ├── GUI/                  # FrmMain (Tổng quan, Tiếp nhận, Danh mục thuốc, Kê đơn, Đơn thuốc & Thanh toán, Báo cáo)
│   ├── App.config / Program.cs
├── images/                   # Ảnh chụp màn hình
├── README.md
└── ClinicManagement.sln
```

| Tầng | Vai trò |
|------|---------|
| **DTO** | Đối tượng truyền dữ liệu giữa các tầng |
| **DAL** | Truy vấn ADO.NET: SqlConnection, SqlCommand + SqlParameter, SqlDataReader, SqlDataAdapter, SqlTransaction |
| **BUS** | Kiểm tra ràng buộc, tính toán nghiệp vụ |
| **GUI** | Giao diện WinForms, chỉ gọi tầng BUS |

## MỤC 5: CHỨC NĂNG ĐÃ HOÀN THÀNH
- [x] CSDL `ClinicDB` 5 bảng: BenhNhan, HoSoKham, Thuoc, DonThuoc, ChiTietDonThuoc (+ dữ liệu mẫu)
- [x] Tra cứu bệnh nhân theo **SoDienThoai** bằng **SqlParameter**; bệnh nhân mới → nhập thông tin và tạo **HoSoKham** (cùng 1 transaction); xem lịch sử khám
- [x] Danh mục thuốc: thêm thuốc mới, cập nhật đơn giá, **nhập thêm tồn kho**; tô đỏ thuốc sắp hết
- [x] Bác sĩ nhập chẩn đoán, chọn thuốc kèm số lượng và **liều dùng**
- [x] "Lưu & Xuất đơn" chạy **SqlTransaction**: (1) tạo DonThuoc → (2) tạo ChiTietDonThuoc → (3) `SoLuongTon -= SoLuong` → (4) thiếu thuốc thì `trans.Rollback()` và cảnh báo
- [x] Tổng chi phí = `TienKham + Σ(SoLuong × DonGia)`; thanh toán chuyển trạng thái sang "Đã thanh toán" và in hóa đơn
- [x] Báo cáo doanh thu theo ngày, danh sách thuốc sắp hết

> Gợi ý chụp ảnh lỗi (ảnh 05): kê 5 vỉ *Ginkgo Biloba 120mg* (kho chỉ còn 3) → giao dịch Rollback.

