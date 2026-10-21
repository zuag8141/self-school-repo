-- =============================================================
-- 1. KhoiNBD - Quản lý thành viên
-- =============================================================

-- Reference Table
CREATE TABLE [dbo].[MembershipPackageKhoiNBD] (
    [MembershipPackageKhoiNBDId]    INT              IDENTITY(1,1) NOT NULL,
    [PackageName]                    NVARCHAR(200)    NOT NULL,
    [Description]                    NVARCHAR(MAX)    NULL,
    [DurationInDays]                 INT              NULL,
    [Price]                          DECIMAL(18, 2)   NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    CONSTRAINT [PK_MembershipPackageKhoiNBD] PRIMARY KEY ([MembershipPackageKhoiNBDId])
);

-- Main Table
CREATE TABLE [dbo].[MemberKhoiNBD] (
    [MemberKhoiNBDId]                INT              IDENTITY(1,1) NOT NULL,
    [FullName]                       NVARCHAR(250)    NOT NULL,         -- ✅ nvarchar(250)
    [Email]                          NVARCHAR(250)    NULL,
    [Phone]                          NVARCHAR(20)     NULL,
    [DateOfBirth]                    DATE             NULL,
    [Gender]                         NVARCHAR(10)     NULL,
    [Address]                        NVARCHAR(500)    NULL,
    [ImageUrl]                       NVARCHAR(1000)   NULL,
    [Notes]                          NVARCHAR(MAX)    NULL,
    [RegistrationFee]                DECIMAL(18, 2)   NULL,             -- ✅ decimal(18,2)
    [JoinDate]                       DATETIME         NOT NULL,         -- ✅ datetime
    [ExpiryDate]                     DATETIME         NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    [UpdateAt]                       DATETIME         NULL,
    [MembershipPackageKhoiNBDId]     INT              NULL,
    [IsActive]                       BIT              NOT NULL DEFAULT 1, -- ✅ bit
    CONSTRAINT [PK_MemberKhoiNBD] PRIMARY KEY ([MemberKhoiNBDId]),
    CONSTRAINT [FK_MemberKhoiNBD_MembershipPackageKhoiNBD]
        FOREIGN KEY ([MembershipPackageKhoiNBDId])
        REFERENCES [dbo].[MembershipPackageKhoiNBD]([MembershipPackageKhoiNBDId])
);

-- =============================================================
-- 2. BacNV - Quản lý lớp học & lịch tập
-- =============================================================

-- Reference Table
CREATE TABLE [dbo].[ClassTypeBacNV] (
    [ClassTypeBacNVId]               INT              IDENTITY(1,1) NOT NULL,
    [TypeName]                       NVARCHAR(150)    NOT NULL,
    [Description]                    NVARCHAR(MAX)    NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    CONSTRAINT [PK_ClassTypeBacNV] PRIMARY KEY ([ClassTypeBacNVId])
);

-- Main Table
CREATE TABLE [dbo].[ClassBacNV] (
[ClassBacNVId]                   INT              IDENTITY(1,1) NOT NULL,
    [ClassName]                      NVARCHAR(250)    NOT NULL,         -- ✅ nvarchar(250)
    [Description]                    NVARCHAR(MAX)    NULL,
    [MaxStudents]                    INT              NULL,
    [Schedule]                       NVARCHAR(500)    NULL,
    [RoomName]                       NVARCHAR(100)    NULL,
    [Fee]                            DECIMAL(18, 2)   NULL,             -- ✅ decimal(18,2)
    [StartDate]                      DATETIME         NULL,             -- ✅ datetime
    [EndDate]                        DATETIME         NULL,
    [ImageUrl]                       NVARCHAR(1000)   NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    [UpdateAt]                       DATETIME         NULL,
    [ClassTypeBacNVId]               INT              NULL,
    [IsActive]                       BIT              NOT NULL DEFAULT 1, -- ✅ bit
    CONSTRAINT [PK_ClassBacNV] PRIMARY KEY ([ClassBacNVId]),
    CONSTRAINT [FK_ClassBacNV_ClassTypeBacNV]
        FOREIGN KEY ([ClassTypeBacNVId])
        REFERENCES [dbo].[ClassTypeBacNV]([ClassTypeBacNVId])
);

-- =============================================================
-- 3. QuangTT - Quản lý gói thành viên & học phí
-- =============================================================

-- Reference Table
CREATE TABLE [dbo].[FeePackageQuangTT] (
    [FeePackageQuangTTId]            INT              IDENTITY(1,1) NOT NULL,
    [PackageName]                    NVARCHAR(200)    NOT NULL,
    [Description]                    NVARCHAR(MAX)    NULL,
    [DurationInDays]                 INT              NULL,
    [Price]                          DECIMAL(18, 2)   NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    CONSTRAINT [PK_FeePackageQuangTT] PRIMARY KEY ([FeePackageQuangTTId])
);

-- Main Table
CREATE TABLE [dbo].[MembershipFeeQuangTT] (
    [MembershipFeeQuangTTId]         INT              IDENTITY(1,1) NOT NULL,
    [MemberName]                     NVARCHAR(250)    NOT NULL,         -- ✅ nvarchar(250)
    [Amount]                         DECIMAL(18, 2)   NOT NULL,         -- ✅ decimal(18,2)
    [PaymentDate]                    DATETIME         NOT NULL,         -- ✅ datetime
    [PaymentMethod]                  NVARCHAR(100)    NULL,
    [TransactionCode]                NVARCHAR(100)    NULL,
    [StartDate]                      DATETIME         NULL,
    [EndDate]                        DATETIME         NULL,
    [Notes]                          NVARCHAR(MAX)    NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    [UpdateAt]                       DATETIME         NULL,
    [FeePackageQuangTTId]            INT              NULL,
[IsActive]                       BIT              NOT NULL DEFAULT 1, -- ✅ bit
    CONSTRAINT [PK_MembershipFeeQuangTT] PRIMARY KEY ([MembershipFeeQuangTTId]),
    CONSTRAINT [FK_MembershipFeeQuangTT_FeePackageQuangTT]
        FOREIGN KEY ([FeePackageQuangTTId])
        REFERENCES [dbo].[FeePackageQuangTT]([FeePackageQuangTTId])
);

-- =============================================================
-- 4. ThaiNH - Quản lý huấn luyện viên & kế hoạch tập
-- =============================================================

-- Reference Table
CREATE TABLE [dbo].[SpecializationThaiNH] (
    [SpecializationThaiNHId]         INT              IDENTITY(1,1) NOT NULL,
    [SpecializationName]             NVARCHAR(200)    NOT NULL,
    [Description]                    NVARCHAR(MAX)    NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    CONSTRAINT [PK_SpecializationThaiNH] PRIMARY KEY ([SpecializationThaiNHId])
);

-- Main Table
CREATE TABLE [dbo].[CoachThaiNH] (
    [CoachThaiNHId]                  INT              IDENTITY(1,1) NOT NULL,
    [FullName]                       NVARCHAR(250)    NOT NULL,         -- ✅ nvarchar(250)
    [Email]                          NVARCHAR(250)    NULL,
    [Phone]                          NVARCHAR(20)     NULL,
    [DateOfBirth]                    DATE             NULL,
    [Gender]                         NVARCHAR(10)     NULL,
    [Address]                        NVARCHAR(500)    NULL,
    [ImageUrl]                       NVARCHAR(1000)   NULL,
    [Experience]                     NVARCHAR(500)    NULL,
    [Certification]                  NVARCHAR(500)    NULL,
    [Salary]                         DECIMAL(18, 2)   NULL,             -- ✅ decimal(18,2)
    [HireDate]                       DATETIME         NULL,             -- ✅ datetime
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    [UpdateAt]                       DATETIME         NULL,
    [SpecializationThaiNHId]         INT              NULL,
    [IsActive]                       BIT              NOT NULL DEFAULT 1, -- ✅ bit
    CONSTRAINT [PK_CoachThaiNH] PRIMARY KEY ([CoachThaiNHId]),
    CONSTRAINT [FK_CoachThaiNH_SpecializationThaiNH]
        FOREIGN KEY ([SpecializationThaiNHId])
        REFERENCES [dbo].[SpecializationThaiNH]([SpecializationThaiNHId])
);

-- =============================================================
-- 5. NhaLT - Điểm danh & kết quả tập luyện
-- =============================================================

-- Reference Table
CREATE TABLE [dbo].[SessionTypeNhaLT] (
    [SessionTypeNhaLTId]             INT              IDENTITY(1,1) NOT NULL,
    [SessionName]                    NVARCHAR(200)    NOT NULL,
    [Description]                    NVARCHAR(MAX)    NULL,
    [Status]                         INT              NULL,
[PublishDate]                     DATETIME         NULL,
    CONSTRAINT [PK_SessionTypeNhaLT] PRIMARY KEY ([SessionTypeNhaLTId])
);

-- Main Table
CREATE TABLE [dbo].[AttendanceNhaLT] (
    [AttendanceNhaLTId]              INT              IDENTITY(1,1) NOT NULL,
    [MemberName]                     NVARCHAR(250)    NOT NULL,         -- ✅ nvarchar(250)
    [ClassName]                      NVARCHAR(200)    NULL,
    [AttendanceDate]                 DATETIME         NOT NULL,         -- ✅ datetime
    [CheckInTime]                    DATETIME         NULL,
    [CheckOutTime]                   DATETIME         NULL,
    [SessionFee]                     DECIMAL(18, 2)   NULL,             -- ✅ decimal(18,2)
    [TrainingResult]                 NVARCHAR(MAX)    NULL,
    [Notes]                          NVARCHAR(MAX)    NULL,
    [Status]                         INT              NULL,
    [PublishDate]                     DATETIME         NULL,
    [UpdateAt]                       DATETIME         NULL,
    [SessionTypeNhaLTId]             INT              NULL,
    [IsActive]                       BIT              NOT NULL DEFAULT 1, -- ✅ bit
    CONSTRAINT [PK_AttendanceNhaLT] PRIMARY KEY ([AttendanceNhaLTId]),
    CONSTRAINT [FK_AttendanceNhaLT_SessionTypeNhaLT]
        FOREIGN KEY ([SessionTypeNhaLTId])
        REFERENCES [dbo].[SessionTypeNhaLT]([SessionTypeNhaLTId])
);
