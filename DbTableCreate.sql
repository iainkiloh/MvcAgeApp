--Step 1 create new localdb in VS SeverExplorer called MvcAgeApp
--Step 2 run this script against new db to create the table

USE [MvcAgeApp]
GO

/****** Object: Table [dbo].[LoginAttempts] Script Date: 7/13/2019 6:39:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoginAttempts] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [Name]             VARCHAR (50)  NOT NULL,
    [Email]            VARCHAR (150) NOT NULL,
    [LoginAttemptTime] DATETIME      NOT NULL,
    [LoginSuccess]     BIT           NOT NULL
);

