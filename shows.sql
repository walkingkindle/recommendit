USE [master]
GO
/****** Object:  Table [dbo].[Shows]    Script Date: 15/03/2024 10:46:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[FinalEpisodeAired] [datetime2](7) NULL,
	[Score] [int] NULL,
	[Status] [nvarchar](max) NULL,
	[OriginalCountry] [nvarchar](max) NULL,
	[OriginalLanguage] [nvarchar](max) NULL,
	[VectorDouble] [nvarchar](max) NULL,
	[ReleaseYear] [int] NULL,
 CONSTRAINT [PK_Shows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
