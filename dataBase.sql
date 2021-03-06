CREATE DATABASE [DBRockPaperScissors]
GO
USE [DBRockPaperScissors]
GO
/****** Object:  Table [dbo].[Players]    Script Date: 22/03/2015 21:31:54 ******/
CREATE TABLE [dbo].[Players](
	[idPlayer] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](50) NOT NULL,
	[Points] [int] NOT NULL,
 CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
(
	[idPlayer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO