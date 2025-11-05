USE [master]
GO
/****** Object:  Database [PlannerDB]    Script Date: 05.11.2025 21:33:11 ******/
CREATE DATABASE [PlannerDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PlannerDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\PlannerDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PlannerDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\PlannerDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PlannerDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PlannerDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PlannerDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PlannerDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PlannerDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PlannerDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PlannerDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [PlannerDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [PlannerDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PlannerDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PlannerDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PlannerDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PlannerDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PlannerDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PlannerDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PlannerDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PlannerDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PlannerDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PlannerDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PlannerDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PlannerDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PlannerDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PlannerDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PlannerDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PlannerDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PlannerDB] SET  MULTI_USER 
GO
ALTER DATABASE [PlannerDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PlannerDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PlannerDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PlannerDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PlannerDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PlannerDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [PlannerDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [PlannerDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PlannerDB]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 05.11.2025 21:33:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[attachment_id] [int] IDENTITY(1,1) NOT NULL,
	[event_id] [int] NOT NULL,
	[file_name] [nvarchar](255) NOT NULL,
	[file_path] [nvarchar](max) NOT NULL,
	[upload_date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[attachment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventCategories]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventCategories](
	[category_id] [int] IDENTITY(1,1) NOT NULL,
	[category_name] [nvarchar](100) NOT NULL,
	[color_hex] [nvarchar](7) NULL,
	[user_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[category_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventParticipants]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventParticipants](
	[event_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[event_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[event_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](200) NOT NULL,
	[description] [nvarchar](max) NULL,
	[start_time] [datetime] NOT NULL,
	[end_time] [datetime] NOT NULL,
	[location] [nvarchar](200) NULL,
	[image_path] [nvarchar](max) NULL,
	[creator_user_id] [int] NOT NULL,
	[category_id] [int] NULL,
	[status_id] [int] NOT NULL,
	[reminder_enabled] [bit] NOT NULL,
	[reminder_minutes_before] [int] NULL,
	[assignee_user_id] [int] NULL,
	[assignment_notified] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[event_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EventStatuses]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventStatuses](
	[status_id] [int] IDENTITY(1,1) NOT NULL,
	[status_name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecurrencePatterns]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecurrencePatterns](
	[pattern_id] [int] IDENTITY(1,1) NOT NULL,
	[event_id] [int] NOT NULL,
	[frequency_type] [nvarchar](20) NOT NULL,
	[interval] [int] NOT NULL,
	[end_date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[pattern_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reminders]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reminders](
	[reminder_id] [int] IDENTITY(1,1) NOT NULL,
	[event_id] [int] NOT NULL,
	[reminder_time] [datetime] NOT NULL,
	[is_sent] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[reminder_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfiles](
	[profile_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[first_name] [nvarchar](100) NULL,
	[last_name] [nvarchar](100) NULL,
	[email] [nvarchar](150) NULL,
	[profile_image_path] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[profile_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05.11.2025 21:33:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](100) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[registration_date] [datetime] NOT NULL,
	[role_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Events] ON 
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (10, N'f', N'd', CAST(N'2025-10-31T19:23:00.000' AS DateTime), CAST(N'2025-10-31T20:23:00.000' AS DateTime), N'f', NULL, 1006, NULL, 1, 0, 15, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (13, N'пи', N' ми', CAST(N'2025-10-31T19:40:00.000' AS DateTime), CAST(N'2025-10-31T20:34:00.000' AS DateTime), N'и', NULL, 1003, NULL, 1, 0, 15, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (15, N'h', N'h', CAST(N'2025-11-01T20:07:00.000' AS DateTime), CAST(N'2025-11-02T21:07:00.000' AS DateTime), N'hh', N'C:\Users\Nastyauuu\Desktop\dr\5.jpg', 1003, NULL, 1, 0, 15, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (16, N'bf', N'gdbf', CAST(N'2025-10-28T20:07:00.000' AS DateTime), CAST(N'2025-10-29T21:07:00.000' AS DateTime), N'fgbf', NULL, 1003, NULL, 1, 0, 15, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (21, N'new', N'jjjj', CAST(N'2025-10-31T21:12:00.000' AS DateTime), CAST(N'2025-10-31T22:09:00.000' AS DateTime), N'b', NULL, 1007, NULL, 1, 1, 1, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (22, N'df', N'f', CAST(N'2025-10-31T21:10:00.000' AS DateTime), CAST(N'2025-10-31T22:10:00.000' AS DateTime), N'f', NULL, 1007, NULL, 1, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (23, N'hhg', N'u', CAST(N'2025-10-31T13:59:00.000' AS DateTime), CAST(N'2025-10-31T16:40:00.000' AS DateTime), N'gg', NULL, 1007, NULL, 1, 1, 15, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (24, N'grtg', N'tgrtghgnghn', CAST(N'2025-10-31T22:17:00.000' AS DateTime), CAST(N'2025-10-31T23:17:00.000' AS DateTime), N'gt', NULL, 1009, NULL, 1, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (25, N'g', N'g', CAST(N'2025-11-01T13:15:00.000' AS DateTime), CAST(N'2025-11-01T14:13:00.000' AS DateTime), N'g', NULL, 1007, NULL, 1, 1, 1, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (1025, N'vf', N'vffvf', CAST(N'2025-11-05T16:05:00.000' AS DateTime), CAST(N'2025-11-05T17:02:00.000' AS DateTime), N'fvv', N'C:\Users\Nastyauuu\Desktop\dr\2.jpg', 1006, NULL, 1, 1, 1, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (1026, N'sdvsd', N'', CAST(N'2025-11-05T16:34:00.000' AS DateTime), CAST(N'2025-11-05T17:34:00.000' AS DateTime), N'vdsvdsv', N'C:\Users\Nastyauuu\Desktop\dr\2.jpg', 1009, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (1027, N'назнач', N'уауа', CAST(N'2025-11-05T17:02:00.000' AS DateTime), CAST(N'2025-11-05T18:02:00.000' AS DateTime), N'ппп', N'C:\Users\Nastyauuu\Desktop\dr\6.jpg', 1009, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2026, N'нр', N'рн', CAST(N'2025-11-05T19:55:00.000' AS DateTime), CAST(N'2025-11-05T20:55:00.000' AS DateTime), N'рнр', NULL, 1009, NULL, 1, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2027, N'yuiky', N'yiui', CAST(N'2025-11-05T19:57:00.000' AS DateTime), CAST(N'2025-11-05T20:57:00.000' AS DateTime), N'iyu', NULL, 1007, NULL, 1, 0, NULL, NULL, 0)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2028, N'new nazn', N'bgf', CAST(N'2025-11-05T20:03:00.000' AS DateTime), CAST(N'2025-11-05T21:03:00.000' AS DateTime), N'gbfbg', NULL, 1006, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2029, N'ttt', N'mjm', CAST(N'2025-11-05T20:05:00.000' AS DateTime), CAST(N'2025-11-05T21:05:00.000' AS DateTime), N'jmjm', NULL, 1009, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2030, N'fff', N'fff', CAST(N'2025-11-06T21:09:00.000' AS DateTime), CAST(N'2025-11-07T22:09:00.000' AS DateTime), N'fff', NULL, 1009, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2031, N'nnnnn', N'nnnnnnn', CAST(N'2025-11-08T21:09:00.000' AS DateTime), CAST(N'2025-11-17T22:09:00.000' AS DateTime), N'nnnnnnnn', NULL, 1009, NULL, 1, 0, NULL, 1007, 1)
GO
INSERT [dbo].[Events] ([event_id], [title], [description], [start_time], [end_time], [location], [image_path], [creator_user_id], [category_id], [status_id], [reminder_enabled], [reminder_minutes_before], [assignee_user_id], [assignment_notified]) VALUES (2032, N'gggggg', N'tggttg', CAST(N'2025-11-05T21:25:00.000' AS DateTime), CAST(N'2025-11-05T22:25:00.000' AS DateTime), N'gggg', NULL, 1009, NULL, 1, 0, NULL, 1007, 1)
GO
SET IDENTITY_INSERT [dbo].[Events] OFF
GO
SET IDENTITY_INSERT [dbo].[EventStatuses] ON 
GO
INSERT [dbo].[EventStatuses] ([status_id], [status_name]) VALUES (2, N'В процессе')
GO
INSERT [dbo].[EventStatuses] ([status_id], [status_name]) VALUES (3, N'Выполнено')
GO
INSERT [dbo].[EventStatuses] ([status_id], [status_name]) VALUES (1, N'Запланировано')
GO
INSERT [dbo].[EventStatuses] ([status_id], [status_name]) VALUES (4, N'Отменено')
GO
SET IDENTITY_INSERT [dbo].[EventStatuses] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([role_id], [role_name]) VALUES (1, N'Administrator')
GO
INSERT [dbo].[Roles] ([role_id], [role_name]) VALUES (4, N'Director')
GO
INSERT [dbo].[Roles] ([role_id], [role_name]) VALUES (3, N'Manager')
GO
INSERT [dbo].[Roles] ([role_id], [role_name]) VALUES (2, N'User')
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[UserProfiles] ON 
GO
INSERT [dbo].[UserProfiles] ([profile_id], [user_id], [first_name], [last_name], [email], [profile_image_path]) VALUES (1003, 1003, N'Иван', N'Иванов', NULL, NULL)
GO
INSERT [dbo].[UserProfiles] ([profile_id], [user_id], [first_name], [last_name], [email], [profile_image_path]) VALUES (1006, 1006, N'Админ', N'Глав', NULL, NULL)
GO
INSERT [dbo].[UserProfiles] ([profile_id], [user_id], [first_name], [last_name], [email], [profile_image_path]) VALUES (1007, 1007, N'Петр', N'Петров', NULL, NULL)
GO
INSERT [dbo].[UserProfiles] ([profile_id], [user_id], [first_name], [last_name], [email], [profile_image_path]) VALUES (1008, 1008, N'Директор', N'Крутой', NULL, NULL)
GO
INSERT [dbo].[UserProfiles] ([profile_id], [user_id], [first_name], [last_name], [email], [profile_image_path]) VALUES (1009, 1009, N'Менеджер', N'Гига', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[UserProfiles] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([user_id], [username], [password_hash], [registration_date], [role_id]) VALUES (1003, N'testus', N'71fb0eb2817861ed811d6b253d5e5813692dd317da5dfac63d98e7ce5b6c6539', CAST(N'2025-10-30T14:16:48.353' AS DateTime), 2)
GO
INSERT [dbo].[Users] ([user_id], [username], [password_hash], [registration_date], [role_id]) VALUES (1006, N'admin', N'13ef24b4dd646e30c6884242a2bb26349ccc5295e12dec63d084096f017ed131', CAST(N'2025-10-31T19:23:19.970' AS DateTime), 1)
GO
INSERT [dbo].[Users] ([user_id], [username], [password_hash], [registration_date], [role_id]) VALUES (1007, N'user123', N'e606e38b0d8c19b24cf0ee3808183162ea7cd63ff7912dbb22b5e803286b4446', CAST(N'2025-10-31T20:40:26.973' AS DateTime), 2)
GO
INSERT [dbo].[Users] ([user_id], [username], [password_hash], [registration_date], [role_id]) VALUES (1008, N'director', N'ef7d74898c22a4bf0be437d30987bc945310dcdf1ddb48a6bce085f376b302ae', CAST(N'2025-10-31T21:57:08.807' AS DateTime), 3)
GO
INSERT [dbo].[Users] ([user_id], [username], [password_hash], [registration_date], [role_id]) VALUES (1009, N'manager', N'6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17', CAST(N'2025-10-31T21:58:01.433' AS DateTime), 4)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__EventSta__501B375392098B89]    Script Date: 05.11.2025 21:33:12 ******/
ALTER TABLE [dbo].[EventStatuses] ADD UNIQUE NONCLUSTERED 
(
	[status_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__783254B1EBACFE35]    Script Date: 05.11.2025 21:33:12 ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[role_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__UserProf__B9BE370E26D47D91]    Script Date: 05.11.2025 21:33:12 ******/
ALTER TABLE [dbo].[UserProfiles] ADD UNIQUE NONCLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__F3DBC572CC25BD86]    Script Date: 05.11.2025 21:33:12 ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attachments] ADD  DEFAULT (getdate()) FOR [upload_date]
GO
ALTER TABLE [dbo].[EventCategories] ADD  DEFAULT ('#FFFFFF') FOR [color_hex]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [reminder_enabled]
GO
ALTER TABLE [dbo].[Events] ADD  DEFAULT ((0)) FOR [assignment_notified]
GO
ALTER TABLE [dbo].[RecurrencePatterns] ADD  DEFAULT ((1)) FOR [interval]
GO
ALTER TABLE [dbo].[Reminders] ADD  DEFAULT ((0)) FOR [is_sent]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [registration_date]
GO
ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD  CONSTRAINT [FK_Attachments_Events] FOREIGN KEY([event_id])
REFERENCES [dbo].[Events] ([event_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attachments] CHECK CONSTRAINT [FK_Attachments_Events]
GO
ALTER TABLE [dbo].[EventCategories]  WITH CHECK ADD  CONSTRAINT [FK_EventCategories_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([user_id])
GO
ALTER TABLE [dbo].[EventCategories] CHECK CONSTRAINT [FK_EventCategories_Users]
GO
ALTER TABLE [dbo].[EventParticipants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_Events] FOREIGN KEY([event_id])
REFERENCES [dbo].[Events] ([event_id])
GO
ALTER TABLE [dbo].[EventParticipants] CHECK CONSTRAINT [FK_Participants_Events]
GO
ALTER TABLE [dbo].[EventParticipants]  WITH CHECK ADD  CONSTRAINT [FK_Participants_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([user_id])
GO
ALTER TABLE [dbo].[EventParticipants] CHECK CONSTRAINT [FK_Participants_Users]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Assignee] FOREIGN KEY([assignee_user_id])
REFERENCES [dbo].[Users] ([user_id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Assignee]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Categories] FOREIGN KEY([category_id])
REFERENCES [dbo].[EventCategories] ([category_id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Categories]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Statuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[EventStatuses] ([status_id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Statuses]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Users] FOREIGN KEY([creator_user_id])
REFERENCES [dbo].[Users] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Users]
GO
ALTER TABLE [dbo].[RecurrencePatterns]  WITH CHECK ADD  CONSTRAINT [FK_Recurrence_Events] FOREIGN KEY([event_id])
REFERENCES [dbo].[Events] ([event_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RecurrencePatterns] CHECK CONSTRAINT [FK_Recurrence_Events]
GO
ALTER TABLE [dbo].[Reminders]  WITH CHECK ADD  CONSTRAINT [FK_Reminders_Events] FOREIGN KEY([event_id])
REFERENCES [dbo].[Events] ([event_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reminders] CHECK CONSTRAINT [FK_Reminders_Events]
GO
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_UserProfiles_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserProfiles] CHECK CONSTRAINT [FK_UserProfiles_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([role_id])
REFERENCES [dbo].[Roles] ([role_id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
USE [master]
GO
ALTER DATABASE [PlannerDB] SET  READ_WRITE 
GO
