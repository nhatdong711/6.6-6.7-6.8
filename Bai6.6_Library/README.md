# BÀI 6.6: HỆ THỐNG QUẢN LÝ MƯỢN TRẢ SÁCH THƯ VIỆN (LIBRARY MANAGEMENT SYSTEM)

> Bài tập 6.6 — Môn Lập trình .NET (D19QTANM1)

## MỤC 1: THÔNG TIN SINH VIÊN

- **Họ và tên sinh viên:** Trần Nhật Đông
- **Mã sinh viên:** 24810320177
- **Lớp / Khóa:** D19QTANM1

## MỤC 2: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

1. **Màn hình Chính / Tổng quan hệ thống:**
![Màn hình chính](images/01_main_screen.png)

2. **Màn hình Quản lý Danh mục (CRUD - Thêm/Sửa/Xóa):**
![Màn hình CRUD](images/02_crud_screen.png)

3. **Màn hình Thực thi Nghiệp vụ chính (Lập phiếu mượn / Trả sách):**
![Màn hình Nghiệp vụ](images/03_transaction_screen.png)

4. **Màn hình Báo cáo - Thống kê:**
![Màn hình Thống kê](images/04_report_screen.png)

5. **Minh họa An toàn Dữ liệu (Phòng chống SQL Injection / Báo lỗi Transaction Rollback):**
![Minh họa Xử lý lỗi](images/05_error_handling.png)

## MỤC 3: HƯỚNG DẪN CÀI ĐẶT & CHẠY

**Yêu cầu:** Visual Studio 2022 trở lên (workload *.NET desktop development*), .NET 8 SDK, SQL Server (Developer/Express/LocalDB).

1. Clone repository và mở file `LibraryManagement.sln` bằng Visual Studio.
2. **Tạo CSDL:** ở lần chạy đầu, chương trình tự tìm SQL Server (`.`, `.\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`) và chạy `Database/ScriptDB.sql`. Cũng có thể chạy thủ công script này trong SSMS.
3. Nếu cần, sửa chuỗi kết nối trong `LibraryManagement/App.config`.
4. Nhấn **F5** để chạy (NuGet tự khôi phục `Microsoft.Data.SqlClient`, `System.Configuration.ConfigurationManager`).

## MỤC 4: KIẾN TRÚC 3-TIER

```
Bai6.6_Library/
├── Database/
│   └── ScriptDB.sql          # Script khởi tạo CSDL SQL Server và chèn dữ liệu mẫu
├── LibraryManagement/
│   ├── DTO/                  # DocGiaDTO, DauSachDTO, PhieuMuonDTO / ChiTietMuonDTO
│   ├── DAL/                  # DBConnect, DocGiaDAL, DauSachDAL, PhieuMuonDAL, BaoCaoDAL
│   ├── BUS/                  # DocGiaBUS, DauSachBUS, PhieuMuonBUS, BaoCaoBUS
│   ├── GUI/                  # FrmMain (Tổng quan, Đầu sách, Độc giả, Lập phiếu, Phiếu mượn & Trả sách, Quá hạn & Thống kê)
│   ├── App.config / Program.cs
├── images/                   # Ảnh chụp màn hình
├── README.md
└── LibraryManagement.sln
```

| Tầng | Vai trò |
|------|---------|
| **DTO** | Đối tượng truyền dữ liệu giữa các tầng |
| **DAL** | Truy vấn ADO.NET: SqlConnection, SqlCommand + SqlParameter, SqlDataReader, SqlDataAdapter, SqlTransaction |
| **BUS** | Kiểm tra ràng buộc, tính toán nghiệp vụ |
| **GUI** | Giao diện WinForms, chỉ gọi tầng BUS |

## MỤC 5: CHỨC NĂNG ĐÃ HOÀN THÀNH
- [x] CSDL `LibraryDB` 4 bảng: DocGia, DauSach, PhieuMuon, ChiTietMuon (+ dữ liệu mẫu)
- [x] Thêm / sửa / xóa Đầu sách và Độc giả qua đủ các lớp DTO / BUS / DAL; gia hạn thẻ độc giả
- [x] Kiểm tra hạn thẻ (`NgayHetHanThe >= GETDATE()`) trước khi lập phiếu mượn mới (hiển thị trạng thái thẻ ngay khi chọn độc giả)
- [x] Lập phiếu mượn nhiều sách, trừ `SoLuongCon` trong SqlTransaction (hết sách thì Rollback)
- [x] Lọc phiếu mượn theo **Độc giả** và **Trạng thái** ("Đang mượn", "Đã trả", "Quá hạn") bằng Parameterized SQL; tự chuyển phiếu quá hạn sang "Quá hạn"
- [x] Trả sách: Parameterized SQL **UPDATE** chuyển trạng thái sang "Đã trả" và cộng lại `SoLuongCon` cho bảng DauSach
- [x] Cảnh báo quá hạn: **SqlDataReader** lấy các phiếu có `HanTra < GETDATE()` và `TrangThai != N'Đã trả'`
- [x] Thống kê lượt mượn theo **TheLoai** bằng **GROUP BY** và tổng lượt mượn bằng **ExecuteScalar**

> Gợi ý chụp ảnh lỗi (ảnh 05): chọn độc giả *Lê Minh Cường* (thẻ đã hết hạn) để lập phiếu, hoặc mượn 3 cuốn *Số đỏ* (chỉ còn 1).

