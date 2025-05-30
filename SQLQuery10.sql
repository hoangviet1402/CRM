USE [banca]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeesId] [int] NULL,
	[Email] [nvarchar](100) NOT NULL,
	[PasswordHash] [nvarchar](256) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Role] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[LastLogin] [datetime] NULL,
 CONSTRAINT [PK__Users__3214EC07C612EC41] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Users__A9D10534A367AD57] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountTokens]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountTokens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[AccessToken] [nvarchar](200) NOT NULL,
	[RefreshToken] [nvarchar](200) NOT NULL,
	[Expires] [datetime] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatedByIp] [nvarchar](45) NULL,
 CONSTRAINT [PK__RefreshT__3214EC07A31ADC14] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Educations]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Educations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[EducationsLevelId] [int] NULL,
	[MajorId] [int] NULL,
	[Institution] [nvarchar](200) NULL,
	[IssuedDate] [date] NULL,
	[Note] [nvarchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EducationsLevel]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EducationsLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[BirthDate] [datetime] NULL,
	[Gender] [int] NULL,
	[EmployeeCode] [varchar](50) NULL,
	[DisplayOrder] [int] NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[ContactAddress] [nvarchar](200) NULL,
	[Skype] [nvarchar](50) NULL,
	[Facebook] [nvarchar](100) NULL,
	[EmergencyName] [nvarchar](100) NULL,
	[EmergencyMobile] [nvarchar](20) NULL,
	[EmergencyLandline] [nvarchar](20) NULL,
	[EmergencyRelation] [nvarchar](50) NULL,
	[EmergencyAddress] [nvarchar](200) NULL,
	[Country] [nvarchar](100) NULL,
	[Province] [nvarchar](100) NULL,
	[District] [nvarchar](100) NULL,
	[Ward] [nvarchar](100) NULL,
	[PermanentAddress] [nvarchar](200) NULL,
	[Hometown] [nvarchar](200) NULL,
	[CurrentAddress] [nvarchar](200) NULL,
	[IdentityCard] [varchar](20) NULL,
	[IdentityCardCreateDate] [datetime] NULL,
	[IdentityCardPlace] [nvarchar](200) NULL,
	[PassportID] [nvarchar](20) NULL,
	[PassporCreateDate] [datetime] NULL,
	[PassporExp] [datetime] NULL,
	[PassporPlace] [nvarchar](200) NULL,
	[BankHolder] [nvarchar](100) NULL,
	[BankAccount] [nvarchar](50) NULL,
	[BankName] [nvarchar](100) NULL,
	[BankBranch] [nvarchar](100) NULL,
	[TaxIdentification] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Majors]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Majors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Users__CompanyId__4222D4EF]  DEFAULT ((0)) FOR [CompanyId]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Users__Role__4316F928]  DEFAULT ((0)) FOR [Role]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Users__IsActive__440B1D61]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Users__CreatedAt__44FF419A]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[AccountTokens] ADD  CONSTRAINT [DF__RefreshTo__Creat__47DBAE45]  DEFAULT (getdate()) FOR [CreatedAt]
GO
/****** Object:  StoredProcedure [dbo].[Ins_Account_CreateForEmployees]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Viet>
-- Create date: <2025-04-21>
-- Description:	tạo tài khoản accout cho nhân viên 
-- exec [Out_Account_CreateForEmployees] 'bbbb','23232',1,0,0
-- =============================================
CREATE PROCEDURE [dbo].[Ins_Account_CreateForEmployees]	
	@Email NVARCHAR(100) = '',
	@Password NVARCHAR(256) = '',
	@CompanyId int  = 0,
	@Role int = 0 , 
	@AccountId int = 0 out
AS
BEGIN
		-- Kiểm tra thông tin 
		IF EXISTS(Select top(1) Id from [dbo].[Account] with(nolock) where @Email = Email and @CompanyId = [CompanyId])
		BEGIN
			SET @AccountId = -1 -- tài khoản đã tồn tại
			RETURN
		END

		INSERT INTO [dbo].[Account]
			(
					[Email]
				,[PasswordHash]
				,[CompanyId]
				,[Role]
				,[IsActive]
				,[CreatedAt]
			)
		VALUES
			(
				 @Email
				,@Password
				,@CompanyId
				,@Role
				,1
				,GETDATE()
			)

		SET @AccountId = SCOPE_IDENTITY(); 
		print @AccountId
 END
GO
/****** Object:  StoredProcedure [dbo].[Ins_Account_Login]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Viet>
-- Create date: <2025-04-21>
-- Description:	check login
-- =============================================
CREATE PROCEDURE [dbo].[Ins_Account_Login]	
	@Email NVARCHAR(100) = '',
	@Password NVARCHAR(256) = ''
AS
BEGIN
		-- Khai báo table variable
		DECLARE @TempAccount TABLE (
			[Id] INT,
			[Email] NVARCHAR(100),
			[CompanyId] INT,
			[Role] INT,
			[IsActive] BIT,
			[CreatedAt] DATETIME
		);

		-- Chèn dữ liệu vào table variable
		INSERT INTO @TempAccount
		SELECT TOP(1)
			[EmployeesId],
			[Email],
			[CompanyId],
			[Role],
			[IsActive],
			[CreatedAt]
		FROM [dbo].[Account] WITH(NOLOCK)
		WHERE [Email] = @Email AND [PasswordHash] = @Password;

		-- Kiểm tra sự tồn tại của dữ liệu trong table variable
		IF EXISTS (SELECT 1 FROM @TempAccount)
		BEGIN
			-- Cập nhật thông tin
			UPDATE [dbo].[Account]
			SET LastLogin = GETDATE()
			WHERE [Email] = @Email AND [PasswordHash] = @Password;
		END

		SELECT
			[Id],
			[Email],
			[CompanyId],
			[Role],
			[IsActive],
			[CreatedAt]
		FROM @TempAccount
 END
GO
/****** Object:  StoredProcedure [dbo].[Ins_AccountTokens_GetByUserID]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Viet>
-- Create date: <2025-04-21>
-- Description:	update hoặc tạo AccountTokens nếu chưa có
-- =============================================
CREATE PROCEDURE [dbo].[Ins_AccountTokens_GetByUserID]	
	@UserId int = 0
AS
BEGIN
		SELECT top(1) 
		    [Id], 
			[UserId],
			[RefreshToken],
			[AccessToken],
			[Expires]
			[CreatedAt],
			[CreatedByIp]
		FROM [AccountTokens] with(nolock) 
		where [UserId] = @UserId
 END
GO
/****** Object:  StoredProcedure [dbo].[Ins_AccountTokens_UpdateOrInsert]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Viet>
-- Create date: <2025-04-21>
-- Description:	update hoặc tạo AccountTokens nếu chưa có
-- =============================================
CREATE PROCEDURE [dbo].[Ins_AccountTokens_UpdateOrInsert]	
	@UserId int = 0,
	@AccessToken Nvarchar(258)  = '',
	@RefreshToken Nvarchar(258) = '',
	@LifeTime Int = 30, 
	@Ip varchar(100),
	@Imie varchar(100),
	@OutResult int out
AS
BEGIN
		DECLARE @Now datetime = CONVERT(DATE,GETDATE())
		DECLARE @Expires datetime = DATEADD(DAY,@LifeTime,@Now)
		set @OutResult = 0
		IF NOT EXISTS (SELECT top(1) 1 FROM [AccountTokens] with(nolock) where [UserId] = @UserId)
		BEGIN
			INSERT INTO [dbo].[AccountTokens]
				   (
					[UserId]
				   ,[AccessToken]
				   ,[RefreshToken]
				   ,[Expires]
				   ,[CreatedByIp]
				   )
			 VALUES
				   (
						@UserId,
						@AccessToken,
						@RefreshToken,
						@Expires,
						@Ip
				   )
			 set @OutResult = @@IDENTITY
		END
		ELSE
		BEGIN	
			UPDATE [dbo].[AccountTokens]
			SET [AccessToken] = @AccessToken,
				[RefreshToken] = @RefreshToken,
				[Expires] = @Expires,
				[CreatedByIp] = @Ip
			set @OutResult = @@ROWCOUNT
		END
 END
GO
/****** Object:  StoredProcedure [dbo].[Ins_Employee_InsertSimple]    Script Date: 4/23/2025 12:56:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Ins_Employee_InsertSimple]
	@FullName NVARCHAR(100) = '',
	@EmployeeCode NVARCHAR(50) = '',
	@Email NVARCHAR(100) = '',
	@Phone NVARCHAR(100) = '',
	@EmployeeId int = 0 OUT 
AS
BEGIN

	INSERT INTO [dbo].[Employees]
           (
		   [FullName]
           ,[EmployeeCode]
           ,[Email]
           ,[Phone]
		   )
     VALUES
           (
		   @FullName          
           ,@EmployeeCode
           ,@Email
           ,@Phone
		   )

	SET @EmployeeId = @@IDENTITY
END
GO
