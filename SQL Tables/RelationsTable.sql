USE [calculations]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Relations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OriginCalculationId] [int] NOT NULL,
	[DestinationCalculationId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Relations]  WITH CHECK ADD  CONSTRAINT [FK__Relations__Desti__628FA481] FOREIGN KEY([DestinationCalculationId])
REFERENCES [dbo].[Calculations] ([Id])
GO

ALTER TABLE [dbo].[Relations] CHECK CONSTRAINT [FK__Relations__Desti__628FA481]
GO

ALTER TABLE [dbo].[Relations]  WITH CHECK ADD  CONSTRAINT [FK__Relations__Desti__6EF57B66] FOREIGN KEY([DestinationCalculationId])
REFERENCES [dbo].[Calculations] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Relations] CHECK CONSTRAINT [FK__Relations__Desti__6EF57B66]
GO

ALTER TABLE [dbo].[Relations]  WITH CHECK ADD  CONSTRAINT [FK__Relations__Desti__71D1E811] FOREIGN KEY([DestinationCalculationId])
REFERENCES [dbo].[Calculations] ([Id])
GO

ALTER TABLE [dbo].[Relations] CHECK CONSTRAINT [FK__Relations__Desti__71D1E811]
GO

ALTER TABLE [dbo].[Relations]  WITH CHECK ADD  CONSTRAINT [FK__Relations__Desti__74AE54BC] FOREIGN KEY([DestinationCalculationId])
REFERENCES [dbo].[Calculations] ([Id])
GO

ALTER TABLE [dbo].[Relations] CHECK CONSTRAINT [FK__Relations__Desti__74AE54BC]
GO

ALTER TABLE [dbo].[Relations]  WITH CHECK ADD  CONSTRAINT [FK__Relations__Origi__619B8048] FOREIGN KEY([OriginCalculationId])
REFERENCES [dbo].[Calculations] ([Id])
GO

ALTER TABLE [dbo].[Relations] CHECK CONSTRAINT [FK__Relations__Origi__619B8048]
GO


