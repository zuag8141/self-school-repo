# PRN222 - Bộ bài tập ASM01 → ASM04 (QuangTT)

Bộ 4 assignment môn **PRN222**, dùng chung một cơ sở dữ liệu `PRN222` và cùng domain
`MembershipFeeQuangTT` (khoản phí hội viên) + `FeePackageQuangTT` (gói phí).

| Bài | Công nghệ | SignalR | Chức năng chính |
| --- | --- | --- | --- |
| ASM01 | ASP.NET Core MVC | Không | CRUD MVC, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM02 | ASP.NET Core Razor Pages | **Có** | Đăng nhập cookie, CRUD + CRUD SignalR, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM03 | ASP.NET Core Blazor Server | Không | CRUD tương tác, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM04 | .NET Worker Service | Không | Job nền tự động xử lý khoản phí hết hạn và ghi log thống kê |

> Riêng ASM02 có các nút **SignalR Create / Edit / Delete** nằm ngay cạnh các nút
> CRUD Razor Pages thông thường. ASM01, ASM03 và ASM04 **không** tham chiếu SignalR.

---

## 1. Yêu cầu môi trường

- .NET SDK **8.0** trở lên.
- SQL Server **LocalDB** (có sẵn khi cài Visual Studio 2022, workload ASP.NET and web development).
- Visual Studio 2022 hoặc VS Code (tuỳ chọn).
- `sqlcmd` (tuỳ chọn, dùng để chạy script nhanh).

Kiểm tra LocalDB:

```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

---

## 2. Tạo cơ sở dữ liệu

Script duy nhất: **`PRN222_Data.sql`** ở thư mục gốc repository.

Script này:

- Tạo database `PRN222` nếu chưa có.
- Tạo các bảng của cả 5 nhóm bài tập (bao gồm `MembershipFeeQuangTT`,
  `FeePackageQuangTT`) và bảng `[System].[UserAccount]`.
- Chỉ seed dữ liệu mẫu khi bảng tương ứng đang rỗng, nên chạy lại nhiều lần vẫn an toàn.
- Có sẵn **35 khoản phí**, **6 gói phí**, 3 dòng điểm danh và 2 tài khoản đăng nhập.

### Cách 1: dùng sqlcmd

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -f 65001 -i "PRN222_Data.sql"
```

Tham số `-f 65001` giúp giữ đúng tiếng Việt (UTF-8).

### Cách 2: dùng SQL Server Management Studio

1. Mở SSMS, kết nối `(localdb)\MSSQLLocalDB`.
2. Mở file `PRN222_Data.sql`.
3. Bấm **Execute**.

### Kiểm tra nhanh

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -d PRN222 -Q "SELECT COUNT(*) AS Fees FROM dbo.MembershipFeeQuangTT;"
```

Kết quả mong đợi: `35`.

### Connection string dùng chung

Tất cả project đều đọc `ConnectionStrings:DefaultConnection` trong `appsettings.json`:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=PRN222;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Nếu máy dùng SQL Server Express thay cho LocalDB, đổi thành:

```json
"Server=.\\SQLEXPRESS;Database=PRN222;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Reset dữ liệu về trạng thái ban đầu

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "ALTER DATABASE PRN222 SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE PRN222;"
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -b -f 65001 -i "PRN222_Data.sql"
```

---

## 3. Cấu trúc repository

```
.
├── PRN222_Data.sql                         # Script tạo DB + seed dữ liệu
├── PRN222.sql                              # Script gốc (chỉ schema, dùng tham khảo)
├── FA26_PRN222_SE1905_ASM01_QuangTT/
│   ├── zSubscription.Entities.QuangTT/     # Entity dùng chung
│   ├── zSubscription.Repositories.QuangTT/ # EF Core DbContext + repository
│   ├── zSubscription.Services.QuangTT/     # Business service
│   └── zSubscription.MVCWebApp.QuangTT/    # ASP.NET Core MVC
├── FA26_PRN222_SE1905_ASM02_QuangTT/
│   └── zSubscription.RazorWebApp.QuangTT/  # Razor Pages + cookie login + SignalR
├── FA26_PRN222_SE1905_ASM03_QuangTT/
│   └── zSubscription.BlazorWebApp.QuangTT/ # Blazor Server
└── FA26_PRN222_SE1905_ASM04_QuangTT/
    └── zSubscription.WorkerService.QuangTT/ # Background worker
```

Mỗi bài đều có bản copy của 3 project dùng chung (Entities / Repositories / Services)
để có thể mở và chạy độc lập.

---

## 4. Cách chạy từng bài

Mở terminal tại thư mục gốc repository.

### ASM01 - MVC WebApp (cổng 5171)

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM01_QuangTT\zSubscription.MVCWebApp.QuangTT" --urls "http://localhost:5171"
```

Mở: <http://localhost:5171/MembershipFees>

- Nút `Create`, `Edit`, `Delete`: CRUD bằng MVC (form POST + antiforgery token).
- Bài này **không dùng SignalR**; toàn bộ CRUD đi qua controller và Razor View.

### ASM02 - Razor Pages (cổng 5294)

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM02_QuangTT\zSubscription.RazorWebApp.QuangTT" --urls "http://localhost:5294"
```

Mở: <http://localhost:5294>

Tài khoản mẫu:

| Username | Password | Role |
| --- | --- | --- |
| `admin` | `123456` | Admin |
| `staff` | `123456` | Staff |

Sau khi đăng nhập, trang CRUD nằm tại `/MembershipFeesQuangTt`.

- Nút `Create New`, `Details`, `Edit`, `Delete`: CRUD bằng Razor Pages PageModel.
- Nút `SignalR Create`, `SignalR Edit`, `SignalR Delete`: CRUD qua hub `/membershipFeeHub`.
- Hai nhóm nút nằm cạnh nhau trên cùng một trang; thao tác SignalR tự động tải lại danh sách.

### ASM03 - Blazor Server (cổng 5104)

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM03_QuangTT\zSubscription.BlazorWebApp.QuangTT" --urls "http://localhost:5104"
```

Mở: <http://localhost:5104/membership-fees>

Trang chạy chế độ `InteractiveServer`; bảng dùng jQuery để phân trang và có ô chọn
số dòng/trang (5 / 10 / 20 / 50).

### ASM04 - Worker Service

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM04_QuangTT\zSubscription.WorkerService.QuangTT"
```

Worker chạy liên tục, mỗi chu kỳ sẽ:

1. Tìm các khoản phí đang `IsActive = true` nhưng `EndDate < DateTime.Now`.
2. Chuyển chúng sang `IsActive = false`, `Status = 0` (hết hạn).
3. Ghi log tổng số khoản phí, số active/inactive/hết hạn, tổng doanh thu,
   doanh thu trong tháng và số gói phí.

Cấu hình trong `appsettings.json`:

```json
"Worker": {
  "IntervalSeconds": 30,
  "DeactivateExpiredFees": true
}
```

- `IntervalSeconds`: chu kỳ chạy (tối thiểu 5 giây).
- `DeactivateExpiredFees`: bật/tắt việc tự động chuyển khoản phí hết hạn.

Dừng worker bằng `Ctrl + C`.

---

## 5. Chức năng chi tiết

### Tìm kiếm 3 tiêu chí

Cả 3 web app đều tìm theo:

1. **Tên hội viên** - tìm gần đúng (`Contains`).
2. **Phương thức thanh toán** - `Cash`, `Banking`, `Momo`, ...
3. **Tên gói** - `Basic`, `Premium`, `Student`, `PT`, ...

Các tiêu chí kết hợp với nhau bằng `AND`; bỏ trống tiêu chí nào thì bỏ qua tiêu chí đó.

### Phân trang jQuery

- ASM01 và ASM02: 10 dòng/trang, có nút `« 1 2 3 4 »`.
- ASM03: chọn 5 / 10 / 20 / 50 dòng mỗi trang.
- Dữ liệu được render đầy đủ từ server, sau đó plugin jQuery ẩn/hiện dòng để phân trang.

### CRUD

- **ASM01**: MVC controller (`MembershipFeesController`), chỉ dùng CRUD MVC.
- **ASM02**: Razor PageModel (`Pages/MembershipFeesQuangTt/*`), có `Details`, `Create`,
  `Edit`, `Delete`; thêm SignalR hub (`Hubs/MembershipFeeHub.cs`) cho các nút CRUD SignalR.
- **ASM03**: component `Components/Pages/MembershipFees.razor`, dùng `EditForm` +
  `DataAnnotationsValidator`.
- **ASM04**: không có UI; worker dùng chung tầng service/repository để bảo trì dữ liệu.

### Xác thực

Chỉ ASM02 có đăng nhập (cookie authentication). Các trang CRUD được bảo vệ bởi
`[Authorize]`; người chưa đăng nhập sẽ được chuyển về `/Account/Login`.

---

## 6. Mô hình dữ liệu chính

### `dbo.MembershipFeeQuangTT`

| Cột | Kiểu | Ghi chú |
| --- | --- | --- |
| `MembershipFeeQuangTTId` | int, identity | Khóa chính |
| `MemberName` | nvarchar(250) | Tên hội viên |
| `Amount` | decimal(18,2) | Số tiền |
| `PaymentDate` | datetime | Ngày thanh toán |
| `PaymentMethod` | nvarchar(100) | Cash / Banking / Momo... |
| `TransactionCode` | nvarchar(100) | Mã giao dịch |
| `FeePackageQuangTTId` | int, nullable | FK → `FeePackageQuangTT` |
| `StartDate`, `EndDate` | datetime, nullable | Thời hạn gói |
| `Status` | int, nullable | 1 = active, 0 = hết hạn |
| `IsActive` | bit | Trạng thái hiện tại |
| `PublishDate`, `UpdateAt`, `Notes` | | Thông tin bổ sung |

### `dbo.FeePackageQuangTT`

Gồm `FeePackageQuangTTId`, `PackageName`, `Price`, `DurationInDays`, `Description`,
`PublishDate`, `Status`.

### `[System].[UserAccount]`

Tài khoản đăng nhập cho ASM02: `UserName`, `Password`, `Email`, `RoleId`, `IsActive`, ...

---

## 7. Ghi chú kỹ thuật

- Toàn bộ project target `net8.0`.
- DbContext đặt `QueryTrackingBehavior.NoTracking` cho thao tác đọc; repository
  `UpdateAsync` / `DeleteAsync` xoá change tracker trước khi thao tác để tránh lỗi
  trùng key khi chạy Blazor Server (DbContext sống theo circuit).
- Service `UpdateAsync` cập nhật trực tiếp trên entity đã truy vấn, không đính kèm
  navigation `FeePackageQuangTt`, tránh EF cập nhật lan sang bảng gói phí.
- Bảng `System.UserAccount` được map bằng `ToTable("UserAccount", "System")`.
- SignalR chỉ được dùng ở ASM02 (`/membershipFeeHub`), được bảo vệ bằng `[Authorize]`;
  ASM01, ASM03 và ASM04 không tham chiếu SignalR.
- Worker dùng `IServiceScopeFactory` để tạo scope mới cho mỗi chu kỳ, an toàn với
  `DbContext` scoped.
