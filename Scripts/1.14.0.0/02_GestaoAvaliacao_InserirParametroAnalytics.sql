USE [GestaoAvaliacao];

GO 

SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION

GO

IF NOT EXISTS ( SELECT  Id
				FROM    Parameter
				WHERE   [Key] = 'ID_GOOGLE_ANALYTICS') 
BEGIN
	INSERT INTO  [Parameter] 
			([Key], [Value], [Description], [EntityId], [StartDate], [ParameterPage_Id], 
			 [CreateDate], [UpdateDate], [State], 
			 [ParameterCategory_Id], [ParameterType_Id])
	 VALUES ('ID_GOOGLE_ANALYTICS', 'UA-93381246-1', 'Id da conta para utilização do Google Analytics', '6CF424DC-8EC3-E011-9B36-00155D033206',
			 GETDATE(), (SELECT TOP 1 Id FROM ParameterPage WHERE [Description] = 'Geral'),  
			 GETDATE(), GETDATE(), 1, 
			 (SELECT TOP 1 Id FROM ParameterCategory WHERE [Description] = 'Geral'), (SELECT TOP 1 Id FROM ParameterType WHERE [Description] = 'Input'))
END


GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
IF EXISTS (SELECT * FROM #tmpErrors) 
  ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 
  BEGIN
    PRINT 'The database update succeeded'
    COMMIT TRANSACTION
  END
ELSE 
  PRINT 'The database update failed - ROLLBACK aplied'
GO
DROP TABLE #tmpErrors 
GO