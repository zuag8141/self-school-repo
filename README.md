# PRN222 - Bộ bài tập ASM01 → ASM04 (QuangTT)

Bộ 4 assignment môn **PRN222**, dùng chung một cơ sở dữ liệu `PRN222` và cùng domain
`MembershipFeeQuangTT` (khoản phí hội viên) + `FeePackageQuangTT` (gói phí).

> Tên bảng SQL giữ nguyên theo schema gốc: bảng chính `dbo.MembershipFeeQuangTT`,
> bảng tham chiếu `dbo.FeePackageQuangTT`. Prefix `zMembershipFee` chỉ áp dụng cho
> tên project, namespace và cookie; không đổi tên bảng trong database.

| Bài | Công nghệ | SignalR | Chức năng chính |
| --- | --- | --- | --- |
| ASM01 | ASP.NET Core MVC | Không | Đăng nhập cookie, CRUD MVC, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM02 | ASP.NET Core Razor Pages | **Có** | Đăng nhập cookie, CRUD + CRUD SignalR, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM03 | ASP.NET Core Blazor Server | Không | Đăng nhập cookie, CRUD tương tác, tìm kiếm 3 tiêu chí, phân trang jQuery |
| ASM04 | .NET Worker Service | Không | Đăng nhập khi khởi động, job nền tự động xử lý khoản phí hết hạn và ghi log thống kê |

> Cả 4 bài đều có bước xác thực. ASM01/02/03 dùng cookie đăng nhập với tài khoản
> `admin / 123456`; ASM04 (Worker Service, không có UI) kiểm tra tài khoản khi khởi động.
>
> Riêng ASM02 có các nút **SignalR Create / Edit / Delete** nằm ngay cạnh các nút
> CRUD Razor Pages thông thường. ASM01, ASM03 và ASM04 **không** tham chiếu SignalR.

---

## 1. Yêu cầu môi trường

- .NET SDK **8.0** trở lên.
- SQL Server **LocalDB** (có sẵn khi cài Visual Studio 2022, workload ASP.NET and web development).
- Visual Studio 2022 hoặc VS Code (tuỳ chọn).
- `sqlcmd` (tuỳ chọn, dùng để chạy script nhanh).
- **PowerShell 7 (`pwsh`)** nếu máy bật Smart App Control / Application Control
  (xem mục 8). Không dùng `powershell.exe` 5.1 để ký output.

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
│   ├── zMembershipFee.Entities.QuangTT/     # Entity dùng chung
│   ├── zMembershipFee.Repositories.QuangTT/ # EF Core DbContext + repository
│   ├── zMembershipFee.Services.QuangTT/     # Business service
│   └── zMembershipFee.MVCWebApp.QuangTT/    # ASP.NET Core MVC
├── FA26_PRN222_SE1905_ASM02_QuangTT/
│   └── zMembershipFee.RazorWebApp.QuangTT/  # Razor Pages + cookie login + SignalR
├── FA26_PRN222_SE1905_ASM03_QuangTT/
│   └── zMembershipFee.BlazorWebApp.QuangTT/ # Blazor Server + cookie login
└── FA26_PRN222_SE1905_ASM04_QuangTT/
    └── zMembershipFee.WorkerService.QuangTT/ # Background worker + startup login
```

Mỗi bài đều có bản copy của 3 project dùng chung (Entities / Repositories / Services)
để có thể mở và chạy độc lập.

---

## 4. Cách chạy từng bài

Mở terminal tại thư mục gốc repository.

### ASM01 - MVC WebApp (cổng 5171)

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM01_QuangTT\zMembershipFee.MVCWebApp.QuangTT" --urls "http://localhost:5171"
```

Mở: <http://localhost:5171/MembershipFees>

- Trang CRUD được bảo vệ bởi `[Authorize]`; nếu chưa đăng nhập, ứng dụng chuyển về
  `/Account/Login`. Đăng nhập bằng `admin / 123456` (hoặc `staff / 123456`).
- Nút `Create`, `Edit`, `Delete`: CRUD bằng MVC (form POST + antiforgery token).
- Nút `Logout` ở thanh điều hướng để xoá cookie đăng nhập.
- Bài này **không dùng SignalR**; toàn bộ CRUD đi qua controller và Razor View.

### ASM02 - Razor Pages (cổng 5294)

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM02_QuangTT\zMembershipFee.RazorWebApp.QuangTT" --urls "http://localhost:5294"
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
dotnet run --project "FA26_PRN222_SE1905_ASM03_QuangTT\zMembershipFee.BlazorWebApp.QuangTT" --urls "http://localhost:5104"
```

Mở: <http://localhost:5104/membership-fees>

- Trang `/membership-fees` có `[Authorize]`; nếu chưa đăng nhập, ứng dụng chuyển về
  `/login?ReturnUrl=%2Fmembership-fees`. Đăng nhập bằng `admin / 123456`.
- Form login là static SSR, POST tới `/account/login` kèm antiforgery token; nút
  `Logout` POST tới `/account/logout`.
- Trang chạy chế độ `InteractiveServer`; bảng dùng jQuery để phân trang và có ô chọn
  số dòng/trang (5 / 10 / 20 / 50).

### ASM04 - Worker Service

```powershell
dotnet run --project "FA26_PRN222_SE1905_ASM04_QuangTT\zMembershipFee.WorkerService.QuangTT"
```

Worker chạy liên tục, mỗi chu kỳ sẽ:

1. Tìm các khoản phí đang `IsActive = true` nhưng `EndDate < DateTime.Now`.
2. Chuyển chúng sang `IsActive = false`, `Status = 0` (hết hạn).
3. Ghi log tổng số khoản phí, số active/inactive/hết hạn, tổng doanh thu,
   doanh thu trong tháng và số gói phí.

Worker Service không có giao diện web, nên bước đăng nhập được thực hiện **khi khởi
động**: chương trình kiểm tra `Worker:Login:UserName` / `Password` với bảng
`System.UserAccount` trước khi chạy job. Nếu sai, worker ghi log
`Worker login failed` và thoát với mã `1`; nếu đúng, log
`Worker login successful: admin (Quang TT).` rồi mới bắt đầu vòng lặp.

Cấu hình trong `appsettings.json`:

```json
"Worker": {
  "IntervalSeconds": 30,
  "DeactivateExpiredFees": true,
  "Login": {
    "RequireLogin": true,
    "UserName": "admin",
    "Password": "123456"
  }
}
```

- `IntervalSeconds`: chu kỳ chạy (tối thiểu 5 giây).
- `DeactivateExpiredFees`: bật/tắt việc tự động chuyển khoản phí hết hạn.
- `Login:RequireLogin`: bật/tắt bước đăng nhập khi khởi động (mặc định `true`).
- `Login:UserName`, `Login:Password`: tài khoản dùng để đăng nhập, mặc định
  `admin / 123456`.

Có thể override khi chạy mà không sửa file, ví dụ:

```powershell
$env:Worker__Login__Password = "123456"
dotnet run --project "FA26_PRN222_SE1905_ASM04_QuangTT\zMembershipFee.WorkerService.QuangTT"
```

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

Cả 4 bài đều yêu cầu xác thực trước khi dùng chức năng chính:

- **ASM01 (MVC)**: cookie `zMembershipFee.MVC.Session`, trang login `/Account/Login`.
  `MembershipFeesController` được bảo vệ bằng `[Authorize]`; chưa đăng nhập sẽ bị
  chuyển về trang login.
- **ASM02 (Razor Pages)**: cookie `zMembershipFee.Razor.Session`, trang login
  `/Account/Login`; trang CRUD và hub SignalR `/membershipFeeHub` đều yêu cầu
  `[Authorize]`.
- **ASM03 (Blazor Server)**: cookie `zMembershipFee.Blazor.Session`, trang login
  `/login`. Form login là static SSR, POST tới `/account/login` kèm antiforgery token;
  `Routes.razor` dùng `AuthorizeRouteView`, trang `/membership-fees` có `[Authorize]`.
  Nút logout POST tới `/account/logout`.
- **ASM04 (Worker Service)**: không có UI; worker kiểm tra tài khoản trong
  `Worker:Login` với bảng `System.UserAccount` ngay khi khởi động. Sai tài khoản thì
  ghi log lỗi và thoát với mã `1`, đúng tài khoản mới chạy job nền.

Tài khoản mẫu dùng chung: `admin / 123456` hoặc `staff / 123456`.

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

Tài khoản đăng nhập dùng chung cho ASM01/02/03 và bước kiểm tra khi khởi động của
ASM04: `UserName`, `Password`, `Email`, `RoleId`, `IsActive`, ...

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

---

## 8. Smart App Control / Application Control trên Windows

Nếu Windows bật **Smart App Control** (hoặc WDAC ở chế độ enforce), binary vừa build
có thể bị chặn với mã `0x800711C7`:

```text
An Application Control policy has blocked this file.
```

Đây **không phải lỗi C#**. Chính sách kiểm tra file tại thời điểm nạp (load time),
trước khi code chạy. File build mới có hash mới nên bị coi là chưa tin cậy, trong khi
binary cũ trong repo đã có reputation/hash được máy chấp nhận.

Repo có sẵn build target `Directory.Build.targets` để chạy local mà **không tắt
Smart App Control**:

1. Ký output bằng chứng chỉ dev local trong `Cert:\CurrentUser\My`
   (subject `CN=zMembershipFee Local Dev Code Signing`; tự tạo ở lần build đầu).
2. Dùng lại 3 assembly dùng chung đã được ký từ output **ASM02** cho ASM01/03/04.
   Ba project `Entities` / `Repositories` / `Services` giống nhau về source nhưng
   build ở 4 thư mục khác nhau sẽ ra 4 hash khác nhau; dùng chung bản ASM02 giúp
   các bài còn lại dùng đúng hash đã được máy chấp nhận.

Build cả 4 bài theo đúng thứ tự (ASM02 trước):

```powershell
pwsh -File tools\Build-All.ps1
```

Sau khi build, chạy từng bài như mục 4. Nếu build lẻ một solution, hãy chắc chắn
output ASM02 đã tồn tại (chạy `Build-All.ps1` ít nhất một lần).

Tắt phần ký khi build trên máy không bật Application Control:

```powershell
dotnet build "FA26_PRN222_SE1905_ASM01_QuangTT\FA26_PRN222_SE1905_ASM01_QuangTT.sln" -p:EnableLocalCodeSigning=false
```

hoặc:

```powershell
pwsh -File tools\Build-All.ps1 -NoSign
```

Chứng chỉ dev chỉ dùng cho môi trường local; không dùng cho production. Nếu muốn
gỡ, mở `certmgr.msc` → `Personal` → `Certificates` và xoá chứng chỉ
`zMembershipFee Local Dev Code Signing`.
