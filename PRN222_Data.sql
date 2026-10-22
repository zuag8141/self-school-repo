/* =============================================================================
   PRN222 - Script tạo database + dữ liệu mẫu (self-contained)
   -----------------------------------------------------------------------------
   Cách chạy:
     1. Mở SQL Server Management Studio / Azure Data Studio / Visual Studio
     2. Mở file này và Execute (F5)
     3. Script tự tạo database [PRN222] nếu chưa có, tạo bảng nếu chưa có
        và chỉ chèn dữ liệu khi bảng đang rỗng (chạy lại nhiều lần vẫn an toàn).

   Dữ liệu phục vụ 4 assignment:
     - ASM1: MVC WebApp (cookie login)
     - ASM2: Razor Pages + SignalR (cookie login)
     - ASM3: Blazor WebApp (cookie login)
     - ASM4: Worker Service (login khi khởi động)
   Bảng chính dùng trong ASM: dbo.MembershipFeeQuangTT
   Bảng tham chiếu:           dbo.FeePackageQuangTT
   Đăng nhập ASM1/ASM2/ASM3:  admin / 123456
   Đăng nhập ASM4:            Worker:Login trong appsettings.json
   ============================================================================= */

IF DB_ID(N'PRN222') IS NULL
BEGIN
    CREATE DATABASE [PRN222];
END;
GO

USE [PRN222];
GO

IF SCHEMA_ID(N'System') IS NULL
    EXEC(N'CREATE SCHEMA [System]');
GO

/* =============================================================================
   1. NHÓM QUANGTT - GÓI PHÍ & HỌC PHÍ (dùng cho ASM1, ASM2, ASM3, ASM4)
   ============================================================================= */

IF OBJECT_ID(N'dbo.FeePackageQuangTT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FeePackageQuangTT
    (
        FeePackageQuangTTId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_FeePackageQuangTT PRIMARY KEY,
        PackageName         NVARCHAR(200) NOT NULL,
        Description         NVARCHAR(MAX) NULL,
        DurationInDays      INT NULL,
        Price               DECIMAL(18,2) NULL,
        Status              INT NULL,
        PublishDate         DATETIME NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.MembershipFeeQuangTT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MembershipFeeQuangTT
    (
        MembershipFeeQuangTTId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_MembershipFeeQuangTT PRIMARY KEY,
        MemberName             NVARCHAR(250) NOT NULL,
        Amount                 DECIMAL(18,2) NOT NULL,
        PaymentDate            DATETIME NOT NULL,
        PaymentMethod          NVARCHAR(100) NULL,
        TransactionCode        NVARCHAR(100) NULL,
        StartDate              DATETIME NULL,
        EndDate                DATETIME NULL,
        Notes                  NVARCHAR(MAX) NULL,
        Status                 INT NULL,
        PublishDate            DATETIME NULL,
        UpdateAt               DATETIME NULL,
        FeePackageQuangTTId    INT NULL,
        IsActive               BIT NOT NULL CONSTRAINT DF_MembershipFeeQuangTT_IsActive DEFAULT (1),
        CONSTRAINT FK_MembershipFeeQuangTT_FeePackageQuangTT
            FOREIGN KEY (FeePackageQuangTTId)
            REFERENCES dbo.FeePackageQuangTT (FeePackageQuangTTId)
    );
END;
GO

/* =============================================================================
   2. SYSTEM USER ACCOUNT (dùng cho đăng nhập ASM1/ASM2/ASM3 và ASM4 khi khởi động)
   ============================================================================= */

IF OBJECT_ID(N'[System].[UserAccount]', N'U') IS NULL
BEGIN
    CREATE TABLE [System].[UserAccount]
    (
        UserAccountID   INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_System_UserAccount PRIMARY KEY,
        UserName        NVARCHAR(50)  NOT NULL,
        Password        NVARCHAR(100) NOT NULL,
        FullName        NVARCHAR(100) NOT NULL,
        Email           NVARCHAR(150) NOT NULL,
        Phone           NVARCHAR(50)  NOT NULL,
        EmployeeCode    NVARCHAR(50)  NOT NULL,
        RoleId          INT           NOT NULL,
        RequestCode     NVARCHAR(50)  NULL,
        CreatedDate     DATETIME      NULL,
        ApplicationCode NVARCHAR(50)  NULL,
        CreatedBy       NVARCHAR(50)  NULL,
        ModifiedDate    DATETIME      NULL,
        ModifiedBy      NVARCHAR(50)  NULL,
        IsActive        BIT           NOT NULL CONSTRAINT DF_System_UserAccount_IsActive DEFAULT (1)
    );
END;
GO

/* =============================================================================
   3. CÁC NHÓM KHÁC (giữ đủ schema + một ít dữ liệu mẫu cho database PRN222)
   ============================================================================= */

-- 3.1 KhoiNBD - Quản lý thành viên
IF OBJECT_ID(N'dbo.MembershipPackageKhoiNBD', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MembershipPackageKhoiNBD
    (
        MembershipPackageKhoiNBDId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_MembershipPackageKhoiNBD PRIMARY KEY,
        PackageName       NVARCHAR(200) NOT NULL,
        Description       NVARCHAR(MAX) NULL,
        DurationInDays    INT NULL,
        Price             DECIMAL(18,2) NULL,
        Status            INT NULL,
        PublishDate       DATETIME NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.MemberKhoiNBD', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MemberKhoiNBD
    (
        MemberKhoiNBDId             INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_MemberKhoiNBD PRIMARY KEY,
        FullName                    NVARCHAR(250) NOT NULL,
        Email                       NVARCHAR(250) NULL,
        Phone                       NVARCHAR(20) NULL,
        DateOfBirth                 DATE NULL,
        Gender                      NVARCHAR(10) NULL,
        Address                     NVARCHAR(500) NULL,
        ImageUrl                    NVARCHAR(1000) NULL,
        Notes                       NVARCHAR(MAX) NULL,
        RegistrationFee             DECIMAL(18,2) NULL,
        JoinDate                    DATETIME NOT NULL,
        ExpiryDate                  DATETIME NULL,
        Status                      INT NULL,
        PublishDate                 DATETIME NULL,
        UpdateAt                    DATETIME NULL,
        MembershipPackageKhoiNBDId  INT NULL,
        IsActive                    BIT NOT NULL CONSTRAINT DF_MemberKhoiNBD_IsActive DEFAULT (1),
        CONSTRAINT FK_MemberKhoiNBD_MembershipPackageKhoiNBD
            FOREIGN KEY (MembershipPackageKhoiNBDId)
            REFERENCES dbo.MembershipPackageKhoiNBD (MembershipPackageKhoiNBDId)
    );
END;
GO

-- 3.2 BacNV - Quản lý lớp học & lịch tập
IF OBJECT_ID(N'dbo.ClassTypeBacNV', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClassTypeBacNV
    (
        ClassTypeBacNVId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ClassTypeBacNV PRIMARY KEY,
        TypeName         NVARCHAR(150) NOT NULL,
        Description      NVARCHAR(MAX) NULL,
        Status           INT NULL,
        PublishDate      DATETIME NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.ClassBacNV', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClassBacNV
    (
        ClassBacNVId       INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_ClassBacNV PRIMARY KEY,
        ClassName          NVARCHAR(250) NOT NULL,
        Description        NVARCHAR(MAX) NULL,
        MaxStudents        INT NULL,
        Schedule           NVARCHAR(500) NULL,
        RoomName           NVARCHAR(100) NULL,
        Fee                DECIMAL(18,2) NULL,
        StartDate          DATETIME NULL,
        EndDate            DATETIME NULL,
        ImageUrl           NVARCHAR(1000) NULL,
        Status             INT NULL,
        PublishDate        DATETIME NULL,
        UpdateAt           DATETIME NULL,
        ClassTypeBacNVId   INT NULL,
        IsActive           BIT NOT NULL CONSTRAINT DF_ClassBacNV_IsActive DEFAULT (1),
        CONSTRAINT FK_ClassBacNV_ClassTypeBacNV
            FOREIGN KEY (ClassTypeBacNVId)
            REFERENCES dbo.ClassTypeBacNV (ClassTypeBacNVId)
    );
END;
GO

-- 3.3 ThaiNH - Huấn luyện viên & kế hoạch tập
IF OBJECT_ID(N'dbo.SpecializationThaiNH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SpecializationThaiNH
    (
        SpecializationThaiNHId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_SpecializationThaiNH PRIMARY KEY,
        SpecializationName     NVARCHAR(200) NOT NULL,
        Description            NVARCHAR(MAX) NULL,
        Status                 INT NULL,
        PublishDate            DATETIME NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.CoachThaiNH', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CoachThaiNH
    (
        CoachThaiNHId             INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_CoachThaiNH PRIMARY KEY,
        FullName                  NVARCHAR(250) NOT NULL,
        Email                     NVARCHAR(250) NULL,
        Phone                     NVARCHAR(20) NULL,
        DateOfBirth               DATE NULL,
        Gender                    NVARCHAR(10) NULL,
        Address                   NVARCHAR(500) NULL,
        ImageUrl                  NVARCHAR(1000) NULL,
        Experience                NVARCHAR(500) NULL,
        Certification             NVARCHAR(500) NULL,
        Salary                    DECIMAL(18,2) NULL,
        HireDate                  DATETIME NULL,
        Status                    INT NULL,
        PublishDate               DATETIME NULL,
        UpdateAt                  DATETIME NULL,
        SpecializationThaiNHId    INT NULL,
        IsActive                  BIT NOT NULL CONSTRAINT DF_CoachThaiNH_IsActive DEFAULT (1),
        CONSTRAINT FK_CoachThaiNH_SpecializationThaiNH
            FOREIGN KEY (SpecializationThaiNHId)
            REFERENCES dbo.SpecializationThaiNH (SpecializationThaiNHId)
    );
END;
GO

-- 3.4 NhaLT - Điểm danh & kết quả tập luyện
IF OBJECT_ID(N'dbo.SessionTypeNhaLT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SessionTypeNhaLT
    (
        SessionTypeNhaLTId INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_SessionTypeNhaLT PRIMARY KEY,
        SessionName        NVARCHAR(200) NOT NULL,
        Description        NVARCHAR(MAX) NULL,
        Status             INT NULL,
        PublishDate        DATETIME NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.AttendanceNhaLT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AttendanceNhaLT
    (
        AttendanceNhaLTId   INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_AttendanceNhaLT PRIMARY KEY,
        MemberName          NVARCHAR(250) NOT NULL,
        ClassName           NVARCHAR(200) NULL,
        AttendanceDate      DATETIME NOT NULL,
        CheckInTime         DATETIME NULL,
        CheckOutTime        DATETIME NULL,
        SessionFee          DECIMAL(18,2) NULL,
        TrainingResult      NVARCHAR(MAX) NULL,
        Notes               NVARCHAR(MAX) NULL,
        Status              INT NULL,
        PublishDate         DATETIME NULL,
        UpdateAt            DATETIME NULL,
        SessionTypeNhaLTId  INT NULL,
        IsActive            BIT NOT NULL CONSTRAINT DF_AttendanceNhaLT_IsActive DEFAULT (1),
        CONSTRAINT FK_AttendanceNhaLT_SessionTypeNhaLT
            FOREIGN KEY (SessionTypeNhaLTId)
            REFERENCES dbo.SessionTypeNhaLT (SessionTypeNhaLTId)
    );
END;
GO

/* =============================================================================
   4. DỮ LIỆU MẪU - QUANGTT
   ============================================================================= */

IF NOT EXISTS (SELECT 1 FROM dbo.FeePackageQuangTT)
BEGIN
    SET IDENTITY_INSERT dbo.FeePackageQuangTT ON;

    INSERT INTO dbo.FeePackageQuangTT
        (FeePackageQuangTTId, PackageName, Description, DurationInDays, Price, Status, PublishDate)
    VALUES
        (1, N'Basic 1 tháng',   N'Gói tập cơ bản, phù hợp người mới bắt đầu.',       30,   500000,  1, DATEADD(day, -60, GETDATE())),
        (2, N'Basic 3 tháng',   N'Tiết kiệm hơn khi đăng ký 3 tháng.',                90,   1350000, 1, DATEADD(day, -60, GETDATE())),
        (3, N'Premium 6 tháng', N'Bao gồm lớp group và 2 buổi PT mỗi tháng.',         180,  3600000, 1, DATEADD(day, -45, GETDATE())),
        (4, N'Premium 12 tháng',N'Gói dài hạn, tặng kèm 1 tháng và 4 buổi PT.',       365,  6500000, 1, DATEADD(day, -30, GETDATE())),
        (5, N'Student 3 tháng', N'Ưu đãi dành cho học sinh, sinh viên.',              90,   990000,  1, DATEADD(day, -20, GETDATE())),
        (6, N'PT 1 kèm 1',      N'Gói tập cùng huấn luyện viên cá nhân.',              30,   2500000, 1, DATEADD(day, -10, GETDATE()));

    SET IDENTITY_INSERT dbo.FeePackageQuangTT OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MembershipFeeQuangTT)
BEGIN
    SET IDENTITY_INSERT dbo.MembershipFeeQuangTT ON;

    INSERT INTO dbo.MembershipFeeQuangTT
        (MembershipFeeQuangTTId, MemberName, Amount, PaymentDate, PaymentMethod, TransactionCode,
         StartDate, EndDate, Notes, Status, PublishDate, UpdateAt, FeePackageQuangTTId, IsActive)
    VALUES
        (1,  N'Nguyễn Văn An',     500000,  DATEADD(day, -50, GETDATE()), N'Cash',    N'GD-2026-001', DATEADD(day, -50, GETDATE()), DATEADD(day, -20, GETDATE()), N'Đã hết hạn, cần gia hạn.', 0, DATEADD(day, -50, GETDATE()), GETDATE(), 1, 0),
        (2,  N'Trần Thị Bình',     1350000, DATEADD(day, -100, GETDATE()),N'Banking', N'GD-2026-002', DATEADD(day, -100, GETDATE()),DATEADD(day, -10, GETDATE()), N'Đã hết hạn.',             0, DATEADD(day, -100, GETDATE()),GETDATE(), 2, 0),
        (3,  N'Lê Hoàng Cường',    3600000, DATEADD(day, -20, GETDATE()), N'Momo',    N'GD-2026-003', DATEADD(day, -20, GETDATE()), DATEADD(day, 160, GETDATE()),N'Đang hoạt động.',          1, DATEADD(day, -20, GETDATE()), GETDATE(), 3, 1),
        (4,  N'Phạm Thu Dung',     6500000, DATEADD(day, -15, GETDATE()), N'Banking', N'GD-2026-004', DATEADD(day, -15, GETDATE()), DATEADD(day, 350, GETDATE()),N'Gói 12 tháng.',            1, DATEADD(day, -15, GETDATE()), GETDATE(), 4, 1),
        (5,  N'Hoàng Minh Đức',    990000,  DATEADD(day, -12, GETDATE()), N'Cash',    N'GD-2026-005', DATEADD(day, -12, GETDATE()), DATEADD(day, 78, GETDATE()), N'Gói sinh viên.',           1, DATEADD(day, -12, GETDATE()), GETDATE(), 5, 1),
        (6,  N'Vũ Thanh Hà',       2500000, DATEADD(day, -10, GETDATE()), N'Momo',    N'GD-2026-006', DATEADD(day, -10, GETDATE()), DATEADD(day, 20, GETDATE()), N'PT 1 kèm 1.',              1, DATEADD(day, -10, GETDATE()), GETDATE(), 6, 1),
        (7,  N'Đặng Quốc Huy',     500000,  DATEADD(day, -9,  GETDATE()), N'Cash',    N'GD-2026-007', DATEADD(day, -9,  GETDATE()), DATEADD(day, 21, GETDATE()), N'Gói cơ bản.',              1, DATEADD(day, -9,  GETDATE()), GETDATE(), 1, 1),
        (8,  N'Bùi Thị Lan',       1350000, DATEADD(day, -8,  GETDATE()), N'Banking', N'GD-2026-008', DATEADD(day, -8,  GETDATE()), DATEADD(day, 82, GETDATE()), N'Gói 3 tháng.',             1, DATEADD(day, -8,  GETDATE()), GETDATE(), 2, 1),
        (9,  N'Ngô Văn Khánh',     3600000, DATEADD(day, -7,  GETDATE()), N'Banking', N'GD-2026-009', DATEADD(day, -7,  GETDATE()), DATEADD(day, 173, GETDATE()),N'Gói Premium.',             1, DATEADD(day, -7,  GETDATE()), GETDATE(), 3, 1),
        (10, N'Lý Thị Mai',        500000,  DATEADD(day, -6,  GETDATE()), N'Cash',    N'GD-2026-010', DATEADD(day, -6,  GETDATE()), DATEADD(day, 24, GETDATE()), N'Gói cơ bản.',              1, DATEADD(day, -6,  GETDATE()), GETDATE(), 1, 1),
        (11, N'Trịnh Văn Nam',     6500000, DATEADD(day, -5,  GETDATE()), N'Banking', N'GD-2026-011', DATEADD(day, -5,  GETDATE()), DATEADD(day, 360, GETDATE()),N'Gói 12 tháng.',            1, DATEADD(day, -5,  GETDATE()), GETDATE(), 4, 1),
        (12, N'Phan Thị Oanh',     990000,  DATEADD(day, -4,  GETDATE()), N'Momo',    N'GD-2026-012', DATEADD(day, -4,  GETDATE()), DATEADD(day, 86, GETDATE()), N'Gói sinh viên.',           1, DATEADD(day, -4,  GETDATE()), GETDATE(), 5, 1),
        (13, N'Đỗ Minh Phúc',      2500000, DATEADD(day, -3,  GETDATE()), N'Cash',    N'GD-2026-013', DATEADD(day, -3,  GETDATE()), DATEADD(day, 27, GETDATE()), N'PT 1 kèm 1.',              1, DATEADD(day, -3,  GETDATE()), GETDATE(), 6, 1),
        (14, N'Nguyễn Thị Quỳnh',  1350000, DATEADD(day, -2,  GETDATE()), N'Banking', N'GD-2026-014', DATEADD(day, -2,  GETDATE()), DATEADD(day, 88, GETDATE()), N'Gói 3 tháng.',             1, DATEADD(day, -2,  GETDATE()), GETDATE(), 2, 1),
        (15, N'Võ Văn Sơn',        3600000, DATEADD(day, -1,  GETDATE()), N'Momo',    N'GD-2026-015', DATEADD(day, -1,  GETDATE()), DATEADD(day, 179, GETDATE()),N'Gói Premium.',             1, DATEADD(day, -1,  GETDATE()), GETDATE(), 3, 1),
        (16, N'Lâm Thanh Tâm',     500000,  GETDATE(),                    N'Cash',    N'GD-2026-016', GETDATE(),                    DATEADD(day, 30, GETDATE()), N'Thanh toán hôm nay.',      1, GETDATE(),                    GETDATE(), 1, 1),
        (17, N'Trương Ngọc Thảo',  990000,  GETDATE(),                    N'Banking', N'GD-2026-017', GETDATE(),                    DATEADD(day, 90, GETDATE()), N'Gói sinh viên.',           1, GETDATE(),                    GETDATE(), 5, 1),
        (18, N'Cao Minh Tuấn',     2500000, DATEADD(day, -25, GETDATE()), N'Momo',    N'GD-2026-018', DATEADD(day, -25, GETDATE()), DATEADD(day, 5,   GETDATE()), N'Sắp hết hạn.',             1, DATEADD(day, -25, GETDATE()), GETDATE(), 6, 1),
        (19, N'Đinh Thị Uyên',     1350000, DATEADD(day, -40, GETDATE()), N'Banking', N'GD-2026-019', DATEADD(day, -40, GETDATE()), DATEADD(day, 50, GETDATE()), N'Đang hoạt động.',          1, DATEADD(day, -40, GETDATE()), GETDATE(), 2, 1),
        (20, N'Nguyễn Hoài Việt',  3600000, DATEADD(day, -35, GETDATE()), N'Cash',    N'GD-2026-020', DATEADD(day, -35, GETDATE()), DATEADD(day, 145,GETDATE()), N'Gói Premium.',             1, DATEADD(day, -35, GETDATE()), GETDATE(), 3, 1),
        (21, N'Phạm Gia Bảo',      500000,  DATEADD(day, -80, GETDATE()), N'Cash',    N'GD-2026-021', DATEADD(day, -80, GETDATE()), DATEADD(day, -50,GETDATE()), N'Đã hết hạn.',              0, DATEADD(day, -80, GETDATE()), GETDATE(), 1, 0),
        (22, N'Trần Khánh Chi',    1350000, DATEADD(day, -70, GETDATE()), N'Momo',    N'GD-2026-022', DATEADD(day, -70, GETDATE()), DATEADD(day, 20, GETDATE()), N'Gia hạn sớm.',             1, DATEADD(day, -70, GETDATE()), GETDATE(), 2, 1),
        (23, N'Lê Bảo Duy',        6500000, DATEADD(day, -60, GETDATE()), N'Banking', N'GD-2026-023', DATEADD(day, -60, GETDATE()), DATEADD(day, 305,GETDATE()), N'Gói 12 tháng.',            1, DATEADD(day, -60, GETDATE()), GETDATE(), 4, 1),
        (24, N'Hoàng Mỹ Duyên',    990000,  DATEADD(day, -55, GETDATE()), N'Cash',    N'GD-2026-024', DATEADD(day, -55, GETDATE()), DATEADD(day, 35, GETDATE()), N'Gói sinh viên.',           1, DATEADD(day, -55, GETDATE()), GETDATE(), 5, 1),
        (25, N'Vũ Hải Đăng',       2500000, DATEADD(day, -45, GETDATE()), N'Banking', N'GD-2026-025', DATEADD(day, -45, GETDATE()), DATEADD(day, -15,GETDATE()), N'Đã hết hạn.',              0, DATEADD(day, -45, GETDATE()), GETDATE(), 6, 0),
        (26, N'Đặng Thu Hằng',     500000,  DATEADD(day, -30, GETDATE()), N'Momo',    N'GD-2026-026', DATEADD(day, -30, GETDATE()), DATEADD(day, 0,   GETDATE()), N'Hết hạn hôm nay.',         1, DATEADD(day, -30, GETDATE()), GETDATE(), 1, 1),
        (27, N'Bùi Quốc Hưng',     1350000, DATEADD(day, -28, GETDATE()), N'Cash',    N'GD-2026-027', DATEADD(day, -28, GETDATE()), DATEADD(day, 62, GETDATE()), N'Đang hoạt động.',          1, DATEADD(day, -28, GETDATE()), GETDATE(), 2, 1),
        (28, N'Ngô Thanh Hương',   3600000, DATEADD(day, -22, GETDATE()), N'Banking', N'GD-2026-028', DATEADD(day, -22, GETDATE()), DATEADD(day, 158,GETDATE()), N'Gói Premium.',             1, DATEADD(day, -22, GETDATE()), GETDATE(), 3, 1),
        (29, N'Lý Văn Khải',       6500000, DATEADD(day, -18, GETDATE()), N'Momo',    N'GD-2026-029', DATEADD(day, -18, GETDATE()), DATEADD(day, 347,GETDATE()), N'Gói 12 tháng.',            1, DATEADD(day, -18, GETDATE()), GETDATE(), 4, 1),
        (30, N'Trịnh Thị Kim',     990000,  DATEADD(day, -14, GETDATE()), N'Cash',    N'GD-2026-030', DATEADD(day, -14, GETDATE()), DATEADD(day, 76, GETDATE()), N'Gói sinh viên.',           1, DATEADD(day, -14, GETDATE()), GETDATE(), 5, 1),
        (31, N'Phan Văn Long',     2500000, DATEADD(day, -11, GETDATE()), N'Banking', N'GD-2026-031', DATEADD(day, -11, GETDATE()), DATEADD(day, 19, GETDATE()), N'PT 1 kèm 1.',              1, DATEADD(day, -11, GETDATE()), GETDATE(), 6, 1),
        (32, N'Đỗ Thị Ngọc',       500000,  DATEADD(day, -7,  GETDATE()), N'Momo',    N'GD-2026-032', DATEADD(day, -7,  GETDATE()), DATEADD(day, 23, GETDATE()), N'Gói cơ bản.',              1, DATEADD(day, -7,  GETDATE()), GETDATE(), 1, 1),
        (33, N'Nguyễn Bảo Trâm',   1350000, DATEADD(day, -5,  GETDATE()), N'Cash',    N'GD-2026-033', DATEADD(day, -5,  GETDATE()), DATEADD(day, 85, GETDATE()), N'Gói 3 tháng.',             1, DATEADD(day, -5,  GETDATE()), GETDATE(), 2, 1),
        (34, N'Võ Minh Triết',     3600000, DATEADD(day, -3,  GETDATE()), N'Banking', N'GD-2026-034', DATEADD(day, -3,  GETDATE()), DATEADD(day, 177,GETDATE()), N'Gói Premium.',             1, DATEADD(day, -3,  GETDATE()), GETDATE(), 3, 1),
        (35, N'Lâm Ngọc Vân',      6500000, DATEADD(day, -1,  GETDATE()), N'Momo',    N'GD-2026-035', DATEADD(day, -1,  GETDATE()), DATEADD(day, 364,GETDATE()), N'Gói 12 tháng.',            1, DATEADD(day, -1,  GETDATE()), GETDATE(), 4, 1);

    SET IDENTITY_INSERT dbo.MembershipFeeQuangTT OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM [System].[UserAccount])
BEGIN
    INSERT INTO [System].[UserAccount]
        (UserName, Password, FullName, Email, Phone, EmployeeCode, RoleId,
         CreatedDate, ApplicationCode, CreatedBy, IsActive)
    VALUES
        (N'admin', N'123456', N'Quang TT', N'admin@prn222.local', N'0901234567', N'EMP001', 1,
         GETDATE(), N'PRN222', N'seed', 1),
        (N'staff', N'123456', N'Staff Demo', N'staff@prn222.local', N'0907654321', N'EMP002', 2,
         GETDATE(), N'PRN222', N'seed', 1);
END;
GO

/* =============================================================================
   5. DỮ LIỆU MẪU - CÁC NHÓM KHÁC
   ============================================================================= */

IF NOT EXISTS (SELECT 1 FROM dbo.MembershipPackageKhoiNBD)
BEGIN
    SET IDENTITY_INSERT dbo.MembershipPackageKhoiNBD ON;
    INSERT INTO dbo.MembershipPackageKhoiNBD
        (MembershipPackageKhoiNBDId, PackageName, Description, DurationInDays, Price, Status, PublishDate)
    VALUES
        (1, N'Standard', N'Gói thành viên tiêu chuẩn', 30, 400000, 1, DATEADD(day, -30, GETDATE())),
        (2, N'Gold',     N'Gói thành viên Gold',       90, 1100000, 1, DATEADD(day, -30, GETDATE())),
        (3, N'Diamond',  N'Gói thành viên Diamond',    180, 2000000, 1, DATEADD(day, -30, GETDATE()));
    SET IDENTITY_INSERT dbo.MembershipPackageKhoiNBD OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MemberKhoiNBD)
BEGIN
    SET IDENTITY_INSERT dbo.MemberKhoiNBD ON;
    INSERT INTO dbo.MemberKhoiNBD
        (MemberKhoiNBDId, FullName, Email, Phone, DateOfBirth, Gender, Address, RegistrationFee,
         JoinDate, ExpiryDate, Status, PublishDate, UpdateAt, MembershipPackageKhoiNBDId, IsActive)
    VALUES
        (1, N'Nguyễn Văn An',  N'an@example.com',  N'0901111111', '1998-05-12', N'Nam',  N'Hà Nội',  400000,  DATEADD(day, -20, GETDATE()), DATEADD(day, 10, GETDATE()),  1, DATEADD(day, -20, GETDATE()), GETDATE(), 1, 1),
        (2, N'Trần Thị Bình',  N'binh@example.com', N'0902222222', '2000-08-23', N'Nữ',   N'Đà Nẵng', 1100000, DATEADD(day, -40, GETDATE()), DATEADD(day, 50, GETDATE()),  1, DATEADD(day, -40, GETDATE()), GETDATE(), 2, 1),
        (3, N'Lê Hoàng Cường', N'cuong@example.com',N'0903333333', '1995-02-02', N'Nam',  N'TP.HCM',  2000000, DATEADD(day, -60, GETDATE()), DATEADD(day, 120, GETDATE()), 1, DATEADD(day, -60, GETDATE()), GETDATE(), 3, 1);
    SET IDENTITY_INSERT dbo.MemberKhoiNBD OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ClassTypeBacNV)
BEGIN
    SET IDENTITY_INSERT dbo.ClassTypeBacNV ON;
    INSERT INTO dbo.ClassTypeBacNV (ClassTypeBacNVId, TypeName, Description, Status, PublishDate)
    VALUES
        (1, N'Yoga',      N'Lớp yoga cơ bản và nâng cao', 1, DATEADD(day, -30, GETDATE())),
        (2, N'Gym',       N'Lớp gym theo nhóm',            1, DATEADD(day, -30, GETDATE())),
        (3, N'Boxing',    N'Lớp boxing',                   1, DATEADD(day, -30, GETDATE()));
    SET IDENTITY_INSERT dbo.ClassTypeBacNV OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.ClassBacNV)
BEGIN
    SET IDENTITY_INSERT dbo.ClassBacNV ON;
    INSERT INTO dbo.ClassBacNV
        (ClassBacNVId, ClassName, Description, MaxStudents, Schedule, RoomName, Fee,
         StartDate, EndDate, Status, PublishDate, UpdateAt, ClassTypeBacNVId, IsActive)
    VALUES
        (1, N'Yoga Morning', N'Yoga buổi sáng',  20, N'Thứ 2-4-6 06:00', N'Room A', 600000,  DATEADD(day, -10, GETDATE()), DATEADD(day, 50, GETDATE()), 1, DATEADD(day, -10, GETDATE()), GETDATE(), 1, 1),
        (2, N'Gym Evening',  N'Gym buổi tối',    25, N'Thứ 3-5-7 18:00', N'Room B', 800000,  DATEADD(day, -10, GETDATE()), DATEADD(day, 50, GETDATE()), 1, DATEADD(day, -10, GETDATE()), GETDATE(), 2, 1),
        (3, N'Boxing Intro', N'Boxing nhập môn', 15, N'Thứ 2-4 19:00',    N'Room C', 700000,  DATEADD(day, -5,  GETDATE()), DATEADD(day, 55, GETDATE()), 1, DATEADD(day, -5,  GETDATE()), GETDATE(), 3, 1);
    SET IDENTITY_INSERT dbo.ClassBacNV OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SpecializationThaiNH)
BEGIN
    SET IDENTITY_INSERT dbo.SpecializationThaiNH ON;
    INSERT INTO dbo.SpecializationThaiNH (SpecializationThaiNHId, SpecializationName, Description, Status, PublishDate)
    VALUES
        (1, N'Strength & Conditioning', N'Huấn luyện sức mạnh', 1, DATEADD(day, -30, GETDATE())),
        (2, N'Yoga & Mobility',         N'Yoga và linh hoạt',   1, DATEADD(day, -30, GETDATE())),
        (3, N'Boxing',                  N'Boxing và self-defense', 1, DATEADD(day, -30, GETDATE()));
    SET IDENTITY_INSERT dbo.SpecializationThaiNH OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.CoachThaiNH)
BEGIN
    SET IDENTITY_INSERT dbo.CoachThaiNH ON;
    INSERT INTO dbo.CoachThaiNH
        (CoachThaiNHId, FullName, Email, Phone, DateOfBirth, Gender, Address, Experience, Certification,
         Salary, HireDate, Status, PublishDate, UpdateAt, SpecializationThaiNHId, IsActive)
    VALUES
        (1, N'Phạm Minh Quân', N'quan.coach@example.com', N'0911111111', '1990-01-15', N'Nam', N'Hà Nội',
         N'8 năm kinh nghiệm', N'PT Quốc tế', 15000000, DATEADD(year, -3, GETDATE()), 1, DATEADD(day, -30, GETDATE()), GETDATE(), 1, 1),
        (2, N'Nguyễn Thu Hà',  N'ha.coach@example.com',   N'0922222222', '1993-06-20', N'Nữ',  N'TP.HCM',
         N'6 năm kinh nghiệm', N'Yoga RYT-200', 13000000, DATEADD(year, -2, GETDATE()), 1, DATEADD(day, -30, GETDATE()), GETDATE(), 2, 1),
        (3, N'Trần Văn Sỹ',    N'sy.coach@example.com',   N'0933333333', '1988-11-05', N'Nam', N'Đà Nẵng',
         N'10 năm kinh nghiệm', N'Boxing Coach', 16000000, DATEADD(year, -4, GETDATE()), 1, DATEADD(day, -30, GETDATE()), GETDATE(), 3, 1);
    SET IDENTITY_INSERT dbo.CoachThaiNH OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SessionTypeNhaLT)
BEGIN
    SET IDENTITY_INSERT dbo.SessionTypeNhaLT ON;
    INSERT INTO dbo.SessionTypeNhaLT (SessionTypeNhaLTId, SessionName, Description, Status, PublishDate)
    VALUES
        (1, N'Group Class', N'Buổi tập nhóm', 1, DATEADD(day, -30, GETDATE())),
        (2, N'PT Session',  N'Buổi tập với PT', 1, DATEADD(day, -30, GETDATE())),
        (3, N'Yoga',        N'Buổi yoga', 1, DATEADD(day, -30, GETDATE()));
    SET IDENTITY_INSERT dbo.SessionTypeNhaLT OFF;
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.AttendanceNhaLT)
BEGIN
    SET IDENTITY_INSERT dbo.AttendanceNhaLT ON;
    INSERT INTO dbo.AttendanceNhaLT
        (AttendanceNhaLTId, MemberName, ClassName, AttendanceDate, CheckInTime, CheckOutTime,
         SessionFee, TrainingResult, Notes, Status, PublishDate, UpdateAt, SessionTypeNhaLTId, IsActive)
    VALUES
        (1, N'Nguyễn Văn An',  N'Yoga Morning', DATEADD(day, -3, GETDATE()), DATEADD(hour, 6, CAST(GETDATE() AS datetime)), DATEADD(hour, 7, CAST(GETDATE() AS datetime)), 50000, N'Tốt', N'Đúng giờ', 1, DATEADD(day, -3, GETDATE()), GETDATE(), 1, 1),
        (2, N'Trần Thị Bình',  N'Gym Evening',  DATEADD(day, -2, GETDATE()), DATEADD(hour, 18, CAST(GETDATE() AS datetime)), DATEADD(hour, 19, CAST(GETDATE() AS datetime)), 70000, N'Khá', N'Đi muộn 10 phút', 1, DATEADD(day, -2, GETDATE()), GETDATE(), 2, 1),
        (3, N'Lê Hoàng Cường', N'Boxing Intro', DATEADD(day, -1, GETDATE()), DATEADD(hour, 19, CAST(GETDATE() AS datetime)), DATEADD(hour, 20, CAST(GETDATE() AS datetime)), 80000, N'Tốt', N'Hoàn thành tốt', 1, DATEADD(day, -1, GETDATE()), GETDATE(), 3, 1);
    SET IDENTITY_INSERT dbo.AttendanceNhaLT OFF;
END;
GO

PRINT N'PRN222 database đã sẵn sàng với dữ liệu mẫu.';
PRINT N'Đăng nhập ASM2: admin / 123456';
GO
