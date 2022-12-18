﻿CREATE DATABASE [BinanceDB]
GO

USE [BinanceDB]
GO

CREATE TABLE [BinanceDB].[dbo].[Symbols](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](10) NOT NULL,
	[Price] [MONEY] NULL,
	[Timestamp] [DATETIME] NOT NULL, 
	CONSTRAINT [PK_Symbols] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO