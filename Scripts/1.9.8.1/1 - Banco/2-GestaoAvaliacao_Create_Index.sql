USE [GestaoAvaliacao]
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

CREATE NONCLUSTERED INDEX [IX_TestSectionStatusCorrection_Test_Id_tur_id>]
ON [dbo].[TestSectionStatusCorrection] ([Test_Id],[tur_id])

GO

CREATE NONCLUSTERED INDEX [IX_TestSectionStatusCorrection_Test_Id_tur_id>]
ON [dbo].[StudentTestAbsenceReason] ([Test_Id],[tur_id])

GO

CREATE NONCLUSTERED INDEX [IX_AnswerSheetBatchLog_State]
ON [dbo].[AnswerSheetBatchLog] ([State])
INCLUDE ([AnswerSheetBatchFile_Id],[Situation])

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