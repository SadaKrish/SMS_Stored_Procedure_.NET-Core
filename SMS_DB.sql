USE [SMS_DB]
GO
/****** Object:  StoredProcedure [dbo].[usp_Update_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_Update_Student]
GO
/****** Object:  StoredProcedure [dbo].[usp_ToggleStudentEnable]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_ToggleStudentEnable]
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchStudents]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_SearchStudents]
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteStudent]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_DeleteStudent]
GO
/****** Object:  StoredProcedure [dbo].[usp_Delete_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_Delete_Student]
GO
/****** Object:  StoredProcedure [dbo].[usp_CheckStudentAllocation]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_CheckStudentAllocation]
GO
/****** Object:  StoredProcedure [dbo].[usp_AddOrEdit_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_AddOrEdit_Student]
GO
/****** Object:  StoredProcedure [dbo].[usp_Add_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[usp_Add_Student]
GO
/****** Object:  StoredProcedure [dbo].[Get_Students]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[Get_Students]
GO
/****** Object:  StoredProcedure [dbo].[Get_StudentById]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[Get_StudentById]
GO
/****** Object:  StoredProcedure [dbo].[Delete_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[Delete_Student]
GO
/****** Object:  StoredProcedure [dbo].[AddOrEdit_Student]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP PROCEDURE [dbo].[AddOrEdit_Student]
GO
ALTER TABLE [dbo].[Teacher_Subject_Allocation] DROP CONSTRAINT [FK__Teacher_S__Teach__59063A47]
GO
ALTER TABLE [dbo].[Teacher_Subject_Allocation] DROP CONSTRAINT [FK__Teacher_S__Subje__5812160E]
GO
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation] DROP CONSTRAINT [FK__Student_S__Subje__571DF1D5]
GO
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation] DROP CONSTRAINT [FK__Student_S__Stude__5629CD9C]
GO
ALTER TABLE [dbo].[Teacher] DROP CONSTRAINT [DF__Teacher__IsEnabl__5535A963]
GO
ALTER TABLE [dbo].[Subject] DROP CONSTRAINT [DF__Subject__IsEnabl__5441852A]
GO
ALTER TABLE [dbo].[Student] DROP CONSTRAINT [DF__Student__IsEnabl__534D60F1]
GO
/****** Object:  Index [UQ__Teacher___7733E37D6E34C0C5]    Script Date: 6/28/2024 1:33:20 PM ******/
ALTER TABLE [dbo].[Teacher_Subject_Allocation] DROP CONSTRAINT [UQ__Teacher___7733E37D6E34C0C5]
GO
/****** Object:  Index [UQ__Student___58646DF955A4BE85]    Script Date: 6/28/2024 1:33:20 PM ******/
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation] DROP CONSTRAINT [UQ__Student___58646DF955A4BE85]
GO
/****** Object:  Table [dbo].[Teacher_Subject_Allocation]    Script Date: 6/28/2024 1:33:20 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Teacher_Subject_Allocation]') AND type in (N'U'))
DROP TABLE [dbo].[Teacher_Subject_Allocation]
GO
/****** Object:  Table [dbo].[Teacher]    Script Date: 6/28/2024 1:33:20 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Teacher]') AND type in (N'U'))
DROP TABLE [dbo].[Teacher]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 6/28/2024 1:33:20 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subject]') AND type in (N'U'))
DROP TABLE [dbo].[Subject]
GO
/****** Object:  Table [dbo].[Student_Subject_Teacher_Allocation]    Script Date: 6/28/2024 1:33:20 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Student_Subject_Teacher_Allocation]') AND type in (N'U'))
DROP TABLE [dbo].[Student_Subject_Teacher_Allocation]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 6/28/2024 1:33:20 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Student]') AND type in (N'U'))
DROP TABLE [dbo].[Student]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckStudentRegNoExists]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP FUNCTION [dbo].[fn_CheckStudentRegNoExists]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckStudentAllocation]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP FUNCTION [dbo].[fn_CheckStudentAllocation]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckEmailExists]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP FUNCTION [dbo].[fn_CheckEmailExists]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckDisplayNameExists]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP FUNCTION [dbo].[fn_CheckDisplayNameExists]
GO
USE [master]
GO
/****** Object:  Database [SMS_DB]    Script Date: 6/28/2024 1:33:20 PM ******/
DROP DATABASE [SMS_DB]
GO
/****** Object:  Database [SMS_DB]    Script Date: 6/28/2024 1:33:20 PM ******/
CREATE DATABASE [SMS_DB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SMS_DB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\SMS_DB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SMS_DB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\SMS_DB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [SMS_DB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SMS_DB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SMS_DB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SMS_DB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SMS_DB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SMS_DB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SMS_DB] SET ARITHABORT OFF 
GO
ALTER DATABASE [SMS_DB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SMS_DB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SMS_DB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SMS_DB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SMS_DB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SMS_DB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SMS_DB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SMS_DB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SMS_DB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SMS_DB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SMS_DB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SMS_DB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SMS_DB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SMS_DB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SMS_DB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SMS_DB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SMS_DB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SMS_DB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SMS_DB] SET  MULTI_USER 
GO
ALTER DATABASE [SMS_DB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SMS_DB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SMS_DB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SMS_DB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SMS_DB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SMS_DB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SMS_DB] SET QUERY_STORE = ON
GO
ALTER DATABASE [SMS_DB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [SMS_DB]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckDisplayNameExists]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--check displayname exists
CREATE FUNCTION [dbo].[fn_CheckDisplayNameExists](@DisplayName NVARCHAR(20))
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT;

    IF EXISTS (SELECT 1 FROM dbo.Student WHERE DisplayName = @DisplayName)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;

    RETURN @Exists;
END;
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckEmailExists]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--check emailid exists
CREATE FUNCTION [dbo].[fn_CheckEmailExists](@Email NVARCHAR(30))
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT;

    IF EXISTS (SELECT 1 FROM dbo.Student WHERE Email = @Email)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;

    RETURN @Exists;
END;
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckStudentAllocation]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_CheckStudentAllocation](
    @StudentID BIGINT
)
RETURNS BIT
AS
BEGIN
    DECLARE @IsAllocated BIT;

    -- Check if the student is allocated in any way, modify as needed
    IF EXISTS (
        SELECT 1
        FROM dbo.Student_Subject_Teacher_Allocation AS SC
        WHERE SC.StudentID = @StudentID
        -- Add other allocation conditions as needed
    )
    BEGIN
        SET @IsAllocated = 1;
    END
    ELSE
    BEGIN
        SET @IsAllocated = 0;
    END

    RETURN @IsAllocated;
END;
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CheckStudentRegNoExists]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_CheckStudentRegNoExists](@StudentRegNo NVARCHAR(10))
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT;

    IF EXISTS (SELECT 1 FROM dbo.Student WHERE StudentRegNo = @StudentRegNo)
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;

    RETURN @Exists;
END;
GO
/****** Object:  Table [dbo].[Student]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[StudentID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentRegNo] [nvarchar](10) NOT NULL,
	[FirstName] [nvarchar](20) NOT NULL,
	[MiddleName] [nvarchar](20) NULL,
	[LastName] [nvarchar](20) NOT NULL,
	[DisplayName] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](30) NOT NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[DOB] [date] NOT NULL,
	[Address] [nvarchar](50) NOT NULL,
	[ContactNo] [nvarchar](15) NOT NULL,
	[IsEnable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student_Subject_Teacher_Allocation]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student_Subject_Teacher_Allocation](
	[StudentAllocationID] [bigint] IDENTITY(1,1) NOT NULL,
	[StudentID] [bigint] NOT NULL,
	[SubjectAllocationID] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[SubjectID] [bigint] IDENTITY(1,1) NOT NULL,
	[SubjectCode] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsEnable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teacher]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teacher](
	[TeacherID] [bigint] IDENTITY(1,1) NOT NULL,
	[TeacherRegNo] [nvarchar](10) NOT NULL,
	[FirstName] [nvarchar](20) NOT NULL,
	[MiddleName] [nvarchar](20) NULL,
	[LastName] [nvarchar](20) NOT NULL,
	[DisplayName] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](30) NOT NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[DOB] [date] NOT NULL,
	[Address] [nvarchar](50) NOT NULL,
	[ContactNo] [nvarchar](15) NOT NULL,
	[IsEnable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TeacherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teacher_Subject_Allocation]    Script Date: 6/28/2024 1:33:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teacher_Subject_Allocation](
	[SubjectAllocationID] [bigint] IDENTITY(1,1) NOT NULL,
	[TeacherID] [bigint] NOT NULL,
	[SubjectID] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubjectAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (1, N'ST001', N'Michael', N'James', N'Anderson', N'Michael Anderson', N'michael.anderson@example.com', N'Male', CAST(N'1990-02-28' AS Date), N'789 Elm St', N'555-234-5678', 1)
INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (8, N'ST006', N'Ashwin', N'kurian', N'Nesana', N'Ashwin Kurian', N'ashwinnesan@gmail.com', N'Male', CAST(N'1993-01-12' AS Date), N'1st Ln', N'68986235', 1)
INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (33, N'ST002', N'Emma', NULL, N'Watson', N'Emma', N'emmawatson@gmail.ocm', N'Male', CAST(N'1996-06-13' AS Date), N'hjhghfhg', N'65465', 1)
INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (37, N'ST003', N'Mehala', NULL, N'Manoj', N'Meha', N'mehamanoj@gmail.com', N'Male', CAST(N'2003-11-21' AS Date), N'hgfhgjhg', N'65465413', 1)
INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (39, N'ST009', N'Moana', NULL, N'Royce', N'Moana', N'moanaroyce@gmail.com', N'Female', CAST(N'2000-07-17' AS Date), N'hjahdjhad', N'465431231', 1)
INSERT [dbo].[Student] ([StudentID], [StudentRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (40, N'ST004', N'Jana', NULL, N'Rajan', N'Jana', N'janarajan@gmail.com', N'Female', CAST(N'2005-05-17' AS Date), N'kjhgjgjfsd', N'6456546', 1)
SET IDENTITY_INSERT [dbo].[Student] OFF
GO
SET IDENTITY_INSERT [dbo].[Student_Subject_Teacher_Allocation] ON 

INSERT [dbo].[Student_Subject_Teacher_Allocation] ([StudentAllocationID], [StudentID], [SubjectAllocationID]) VALUES (40, 1, 3)
INSERT [dbo].[Student_Subject_Teacher_Allocation] ([StudentAllocationID], [StudentID], [SubjectAllocationID]) VALUES (70, 8, 3)
INSERT [dbo].[Student_Subject_Teacher_Allocation] ([StudentAllocationID], [StudentID], [SubjectAllocationID]) VALUES (66, 8, 6)
INSERT [dbo].[Student_Subject_Teacher_Allocation] ([StudentAllocationID], [StudentID], [SubjectAllocationID]) VALUES (57, 8, 11)
INSERT [dbo].[Student_Subject_Teacher_Allocation] ([StudentAllocationID], [StudentID], [SubjectAllocationID]) VALUES (68, 33, 6)
SET IDENTITY_INSERT [dbo].[Student_Subject_Teacher_Allocation] OFF
GO
SET IDENTITY_INSERT [dbo].[Subject] ON 

INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (1, N'101', N'Mathematics', 1)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (3, N'EN001', N'English', 1)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (4, N'LI001', N'Literature1', 1)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (5, N'HI003', N'History', 1)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (6, N'DR002', N'Drama', 1)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (17, N'EL001', N'Electronics', 0)
INSERT [dbo].[Subject] ([SubjectID], [SubjectCode], [Name], [IsEnable]) VALUES (18, N'LI005', N'Literature', 0)
SET IDENTITY_INSERT [dbo].[Subject] OFF
GO
SET IDENTITY_INSERT [dbo].[Teacher] ON 

INSERT [dbo].[Teacher] ([TeacherID], [TeacherRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (2, N'TR002', N'Emily', N'Rose', N'Johnson', N'Emily Johnson', N'emily.johnson@example.com', N'Female', CAST(N'1980-05-20' AS Date), N'456 Oak Ave', N'555-987-6543', 1)
INSERT [dbo].[Teacher] ([TeacherID], [TeacherRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (3, N'TR003', N'Leena', N'Maria', N'Sundar', N'Leena', N'leena@gmail.com', N'Male', CAST(N'1997-01-15' AS Date), N'1st Ln', N'5465465465', 1)
INSERT [dbo].[Teacher] ([TeacherID], [TeacherRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (5, N'TR005', N'Mary', NULL, N'John', N'Maryjo', N'mary@gmail.com', N'Female', CAST(N'1998-05-14' AS Date), N'1st Ln', N'46454654', 1)
INSERT [dbo].[Teacher] ([TeacherID], [TeacherRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (17, N'TR001', N'John', NULL, N'Smith', N'John Smith', N'johnsmith@gmail.com', N'Male', CAST(N'1998-09-18' AS Date), N'jbjhhgfhgf', N'65465465', 0)
INSERT [dbo].[Teacher] ([TeacherID], [TeacherRegNo], [FirstName], [MiddleName], [LastName], [DisplayName], [Email], [Gender], [DOB], [Address], [ContactNo], [IsEnable]) VALUES (18, N'TR008', N'Emilton', NULL, N'Rotrigo', N'Emil', N'emitonrotrigo@gmail.com', N'Male', CAST(N'2001-05-24' AS Date), N'jhghgfhdtfdg', N'65465464', 1)
SET IDENTITY_INSERT [dbo].[Teacher] OFF
GO
SET IDENTITY_INSERT [dbo].[Teacher_Subject_Allocation] ON 

INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (14, 2, 4)
INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (3, 3, 3)
INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (6, 3, 5)
INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (8, 5, 1)
INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (13, 5, 3)
INSERT [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID], [TeacherID], [SubjectID]) VALUES (11, 5, 6)
SET IDENTITY_INSERT [dbo].[Teacher_Subject_Allocation] OFF
GO
/****** Object:  Index [UQ__Student___58646DF955A4BE85]    Script Date: 6/28/2024 1:33:21 PM ******/
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation] ADD UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[SubjectAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Teacher___7733E37D6E34C0C5]    Script Date: 6/28/2024 1:33:21 PM ******/
ALTER TABLE [dbo].[Teacher_Subject_Allocation] ADD UNIQUE NONCLUSTERED 
(
	[TeacherID] ASC,
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Student] ADD  DEFAULT ((1)) FOR [IsEnable]
GO
ALTER TABLE [dbo].[Subject] ADD  DEFAULT ((1)) FOR [IsEnable]
GO
ALTER TABLE [dbo].[Teacher] ADD  DEFAULT ((1)) FOR [IsEnable]
GO
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation]  WITH CHECK ADD FOREIGN KEY([StudentID])
REFERENCES [dbo].[Student] ([StudentID])
GO
ALTER TABLE [dbo].[Student_Subject_Teacher_Allocation]  WITH CHECK ADD FOREIGN KEY([SubjectAllocationID])
REFERENCES [dbo].[Teacher_Subject_Allocation] ([SubjectAllocationID])
GO
ALTER TABLE [dbo].[Teacher_Subject_Allocation]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subject] ([SubjectID])
GO
ALTER TABLE [dbo].[Teacher_Subject_Allocation]  WITH CHECK ADD FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teacher] ([TeacherID])
GO
/****** Object:  StoredProcedure [dbo].[AddOrEdit_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddOrEdit_Student]
    @StudentID BIGINT = NULL,
    @StudentRegNo NVARCHAR(10),
    @FirstName NVARCHAR(20),
    @MiddleName NVARCHAR(20),
    @LastName NVARCHAR(20),
    @DisplayName NVARCHAR(20),
    @Email NVARCHAR(30),
    @Gender NVARCHAR(10),
    @DOB DATE,
    @Address NVARCHAR(50),
    @ContactNo NVARCHAR(15),
    @IsEnable BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF @StudentID IS NULL OR (SELECT COUNT(1) FROM dbo.Student WHERE StudentID = @StudentID) = 0
    BEGIN
        -- Insert new student
        INSERT INTO dbo.Student (
            StudentRegNo,
            FirstName,
            MiddleName,
            LastName,
            DisplayName,
            Email,
            Gender,
            DOB,
            Address,
            ContactNo,
            IsEnable
        ) VALUES (
            @StudentRegNo,
            @FirstName,
            @MiddleName,
            @LastName,
            @DisplayName,
            @Email,
            @Gender,
            @DOB,
            @Address,
            @ContactNo,
            @IsEnable
        );
    END
    ELSE
    BEGIN
        -- Update existing student
        BEGIN TRY
            BEGIN TRANSACTION;
            
            UPDATE dbo.Student
            SET
                StudentRegNo = @StudentRegNo,
                FirstName = @FirstName,
                MiddleName = @MiddleName,
                LastName = @LastName,
                DisplayName = @DisplayName,
                Email = @Email,
                Gender = @Gender,
                DOB = @DOB,
                Address = @Address,
                ContactNo = @ContactNo,
                IsEnable = @IsEnable
            WHERE StudentID = @StudentID;
            
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[Delete_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[Delete_Student]
	
	@StudentID bigint
	
as
begin 
Declare @RowCount int=0
begin try
	set @RowCount=(select count(1) from dbo.Student with (Nolock) where StudentID=@StudentID)
	if (@RowCount>0)
		begin
			begin tran
				delete from dbo.Student
					where StudentID=@StudentID
				commit tran
			end
	end try
begin catch
	rollback tran
end catch
end
GO
/****** Object:  StoredProcedure [dbo].[Get_StudentById]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Get_StudentById]
    @StudentID bigint
AS
BEGIN
    SELECT * FROM dbo.Student WITH (NOLOCK)
    WHERE StudentID = @StudentID;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Students]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Students]
    @IsEnable BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        StudentID,
        StudentRegNo,
        FirstName,
        MiddleName,
        LastName,
        DisplayName,
        Email,
        Gender,
        DOB,
        Address,
        ContactNo,
        IsEnable
    FROM dbo.Student WITH (NOLOCK)
    WHERE (@IsEnable IS NULL OR IsEnable = @IsEnable)
END

GO
/****** Object:  StoredProcedure [dbo].[usp_Add_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usp_Add_Student]
	
	@StudentRegNo nvarchar(10),
	@FirstName nvarchar(20),
	@MiddleName nvarchar(20),
	@LastName nvarchar(20),
	@DisplayName nvarchar(20),
	@Email nvarchar(30),
	@Gender nvarchar(10),
	@DOB date,
	@Address nvarchar(50),
	@ContactNo nvarchar(15),
	@IsEnable bit
as
begin 
	insert into DBO.Student(StudentRegNo,FirstName,MiddleName,LastName,DisplayName,Email,Gender,DOB,Address,ContactNo,IsEnable)
		values
		(
			@StudentRegNo,
			@FirstName,
			@MiddleName,
			@LastName,
			@DisplayName,
			@Email,
			@Gender,
			@DOB,
			@Address,
			@ContactNo,
			@IsEnable
		)
	end
GO
/****** Object:  StoredProcedure [dbo].[usp_AddOrEdit_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_AddOrEdit_Student]
    @StudentID BIGINT = NULL,
    @StudentRegNo NVARCHAR(10),
    @FirstName NVARCHAR(20),
    @MiddleName NVARCHAR(20) = NULL,
    @LastName NVARCHAR(20),
    @DisplayName NVARCHAR(20),
    @Email NVARCHAR(30),
    @Gender NVARCHAR(10),
    @DOB DATE,
    @Address NVARCHAR(50),
    @ContactNo NVARCHAR(15),
    @IsEnable BIT,
    @Result INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RowsAffected INT = 0;

    -- Check if StudentRegNo, DisplayName, or Email already exists
    DECLARE @RegNoExists BIT, @DisplayNameExists BIT, @EmailExists BIT;

    SET @RegNoExists = dbo.fn_CheckStudentRegNoExists(@StudentRegNo);
    SET @DisplayNameExists = dbo.fn_CheckDisplayNameExists(@DisplayName);
    SET @EmailExists = dbo.fn_CheckEmailExists(@Email);

    IF @StudentID IS NULL AND (@RegNoExists = 1 OR @DisplayNameExists = 1 OR @EmailExists = 1)
    BEGIN
        DECLARE @InsertErrorMessage NVARCHAR(200);
        SET @InsertErrorMessage = 'Cannot insert. ';

        IF @RegNoExists = 1
            SET @InsertErrorMessage += 'StudentRegNo already exists. ';
        
        IF @DisplayNameExists = 1
            SET @InsertErrorMessage += 'DisplayName already exists. ';
        
        IF @EmailExists = 1
            SET @InsertErrorMessage += 'Email already exists. ';

        SET @Result = 0;
        THROW 51000, @InsertErrorMessage, 1;
    END
    ELSE IF @StudentID IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Student WHERE (StudentRegNo = @StudentRegNo OR DisplayName = @DisplayName OR Email = @Email) AND StudentID != @StudentID)
        BEGIN
            DECLARE @UpdateErrorMessage NVARCHAR(200);
            SET @UpdateErrorMessage = 'Cannot update. ';

            IF EXISTS (SELECT 1 FROM dbo.Student WHERE StudentRegNo = @StudentRegNo AND StudentID != @StudentID)
                SET @UpdateErrorMessage += 'Another student with the same StudentRegNo exists. ';
            
            IF EXISTS (SELECT 1 FROM dbo.Student WHERE DisplayName = @DisplayName AND StudentID != @StudentID)
                SET @UpdateErrorMessage += 'Another student with the same DisplayName exists. ';
            
            IF EXISTS (SELECT 1 FROM dbo.Student WHERE Email = @Email AND StudentID != @StudentID)
                SET @UpdateErrorMessage += 'Another student with the same Email exists. ';

            SET @Result = 0;
            THROW 51000, @UpdateErrorMessage, 1;
        END;
    END;

    IF @StudentID IS NULL
    BEGIN
        INSERT INTO dbo.Student (
            StudentRegNo,
            FirstName,
            MiddleName,
            LastName,
            DisplayName,
            Email,
            Gender,
            DOB,
            Address,
            ContactNo,
            IsEnable
        ) VALUES (
            @StudentRegNo,
            @FirstName,
            @MiddleName,
            @LastName,
            @DisplayName,
            @Email,
            @Gender,
            @DOB,
            @Address,
            @ContactNo,
            @IsEnable
        );
        SET @RowsAffected = @@ROWCOUNT;
    END
    ELSE
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;
            
            UPDATE dbo.Student
            SET
                StudentRegNo = @StudentRegNo,
                FirstName = @FirstName,
                MiddleName = @MiddleName,
                LastName = @LastName,
                DisplayName = @DisplayName,
                Email = @Email,
                Gender = @Gender,
                DOB = @DOB,
                Address = @Address,
                ContactNo = @ContactNo,
                IsEnable = @IsEnable
            WHERE StudentID = @StudentID;
            
            SET @RowsAffected = @@ROWCOUNT;
            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH;
    END;

    SET @Result = @RowsAffected;
END;


GO
/****** Object:  StoredProcedure [dbo].[usp_CheckStudentAllocation]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_CheckStudentAllocation]
    @StudentID BIGINT,
    @Result BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Student_Subject_Teacher_Allocation WHERE StudentID = @StudentID)
    BEGIN
        SET @Result = 1;
    END
    ELSE
    BEGIN
        SET @Result = 0;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Delete_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_Delete_Student]
    @StudentID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAllocated BIT;

    -- check if the student is allocated
    SET @IsAllocated = dbo.fn_CheckStudentAllocation(@StudentID);

    -- If student is allocated, prevent deletion
    IF @IsAllocated = 1
    BEGIN
        THROW 51000, 'Cannot delete student because they are allocated.', 1;
    END
    ELSE
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            -- Delete from the main Student table
            DELETE FROM dbo.Student WHERE StudentID = @StudentID;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH;
    END;
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteStudent]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[usp_DeleteStudent]
    @StudentID BIGINT,
    @Message NVARCHAR(200) OUTPUT,
    @RequiresConfirmation BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @ReferencedInOtherTables BIT;

        -- Check if the student is referenced in other tables
        SET @ReferencedInOtherTables = 
            CASE 
                
                WHEN EXISTS (SELECT 1 FROM dbo.Student_Subject_Teacher_Allocation WHERE StudentID = @StudentID) THEN 1
                ELSE 0 
            END;

        IF @ReferencedInOtherTables = 1
        BEGIN
            SET @RequiresConfirmation = 1;
            SET @Message = 'The student is referenced in other tables and cannot be deleted.';
            RETURN;
        END

        BEGIN TRANSACTION
            DELETE FROM dbo.Student WHERE StudentID = @StudentID;
        COMMIT TRANSACTION;

        SET @RequiresConfirmation = 0;
        SET @Message = 'Student deleted successfully.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @RequiresConfirmation = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[usp_SearchStudents]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Search category
CREATE   PROCEDURE [dbo].[usp_SearchStudents]
    @SearchText NVARCHAR(50),
    @SearchCategory NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Query NVARCHAR(MAX);

    SET @SearchText = '%' + @SearchText + '%';

    -- Case-insensitive search
    SET @Query = '
    SELECT StudentID, StudentRegNo, FirstName, MiddleName, LastName, DisplayName, Email, Gender, DOB, Address, ContactNo, IsEnable
    FROM dbo.Student
    WHERE ' + QUOTENAME(@SearchCategory) + ' COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS';

    -- Execute
    EXEC sp_executesql @Query, N'@SearchText NVARCHAR(50)', @SearchText;
END
GO
/****** Object:  StoredProcedure [dbo].[usp_ToggleStudentEnable]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_ToggleStudentEnable]
    @StudentID BIGINT,
    @Message NVARCHAR(200) OUTPUT,
    @Success BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsEnable BIT;
    DECLARE @ReferencedInOtherTables BIT;

    BEGIN TRY
        -- Check if the student exists
        IF NOT EXISTS (SELECT 1 FROM dbo.Student WHERE StudentID = @StudentID)
        BEGIN
            SET @Message = 'Student not found.';
            SET @Success = 0;
            RETURN;
        END

        -- Get current status
        SELECT @IsEnable = IsEnable FROM dbo.Student WHERE StudentID = @StudentID;

        -- Check if the student is referenced in other tables if currently enabled
        IF @IsEnable = 1
        BEGIN
            SET @ReferencedInOtherTables = 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM dbo.Student_Subject_Teacher_Allocation WHERE StudentID = @StudentID) THEN 1
                    ELSE 0 
                END;

            IF @ReferencedInOtherTables = 1
            BEGIN
                SET @Message = 'Status changed successfully.';
                SET @Success = 1;
                RETURN;
            END
        END

        -- Toggle the enable status
        UPDATE dbo.Student
        SET IsEnable = CASE WHEN @IsEnable = 1 THEN 0 ELSE 1 END
        WHERE StudentID = @StudentID;

        SET @Message = CASE WHEN @IsEnable = 1 THEN 'Disabled Successfully' ELSE 'Enabled Successfully' END;
        SET @Success = 1;
    END TRY
    BEGIN CATCH
        SET @Message = ERROR_MESSAGE();
        SET @Success = 0;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[usp_Update_Student]    Script Date: 6/28/2024 1:33:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usp_Update_Student]
	
	@StudentID bigint,
	@StudentRegNo nvarchar(10),
	@FirstName nvarchar(20),
	@MiddleName nvarchar(20),
	@LastName nvarchar(20),
	@DisplayName nvarchar(20),
	@Email nvarchar(30),
	@Gender nvarchar(10),
	@DOB date,
	@Address nvarchar(50),
	@ContactNo nvarchar(15),
	@IsEnable bit
as
begin 
Declare @RowCount int=0
begin try
	set @RowCount=(select count(1) from dbo.Student with (Nolock) where StudentID=@StudentID)
	if (@RowCount>0)
		begin
			begin tran
				update dbo.Student
					set
						StudentRegNo=@StudentRegNo,
						FirstName =@FirstName,
						MiddleName =@MiddleName,
						LastName=@LastName,
						DisplayName =@DisplayName,
						Email =@Email,
						Gender =@Gender,
						DOB =@DOB,
						Address =@Address,
						ContactNo =@ContactNo,
						IsEnable =@IsEnable
					where StudentID=@StudentID
				commit tran
			end
	end try
begin catch
	rollback tran
end catch
end
GO
USE [master]
GO
ALTER DATABASE [SMS_DB] SET  READ_WRITE 
GO
