USE [calculations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Calculations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Expression] [varchar](255) NOT NULL,
	[Result] [float] NOT NULL,
	[IsDeprecated] [bit] NOT NULL,
 CONSTRAINT [PK_Calculations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


