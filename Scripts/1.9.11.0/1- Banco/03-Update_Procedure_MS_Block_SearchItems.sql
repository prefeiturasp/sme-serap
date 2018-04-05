USE GestaoAvaliacao
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



-- =============================================
-- Author:		Guilherme Mendonca
-- Create date: 22/05/2015
-- Description:	Busca itens para inclusão no bloco
-- =============================================
-- =============================================
-- Alter by:		Gabriel Moreli
-- Date: 01/11/2016
-- Description:	Campo ItemCode foi alterado de INT para VARCHAR,
--				sendo assim necessário a modificação da consulta
-- =============================================

ALTER PROCEDURE [dbo].[MS_Block_SearchItems]
    @ItemCode VARCHAR(32),    
    @ProficiencyStart INT,
    @ProficiencyEnd INT,
    @Keywords VARCHAR(MAX),
    @DisciplineId BIGINT, 
    @EvaluationMatrixId BIGINT,
    @Skills VARCHAR(MAX),
	@ItemLevel VARCHAR(MAX),
    @TypeCurriculumGrades VARCHAR(MAX),
	@Global BIT,
    @pageSize INT,
    @pageNumber INT,
	@ItemTypeID BIGINT = NULL,
    @totalRecords INT OUTPUT
AS 
    BEGIN
    
    IF OBJECT_ID('tempdb..#tmp') > 0 DROP TABLE #tmp

    SELECT  ItemId,
			ItemCode, 
			ItemVersion, 
			Revoked,
			BaseTextDescription, 
			[Statement],
			MatrixDescription, 
			DescriptorSentence,
			BaseTextId,
			MatrixId,
			LastVersion,
			ItemLevelDescription,
			ItemLevelValue,
			TypeCurriculumGradeId,
			[Order],
			ROW_NUMBER() OVER ( ORDER BY ItemCode, ItemVersion) AS RowNumber
	INTO #tmp
	FROM    (
							SELECT  i.Id AS ItemId,
									i.ItemCode, 
									i.ItemVersion, 
									i.Revoked,
									bt.[Description] AS BaseTextDescription, 
									i.[Statement],
									em.[Description] AS MatrixDescription, 
									i.descriptorSentence AS DescriptorSentence,
									bt.Id AS BaseTextId,
									em.Id AS MatrixId,
									i.ItemSituation_Id,
									i.proficiency,
									i.LastVersion,
									i.Keywords,
									em.Discipline_Id,
									i.EvaluationMatrix_Id,
									ik.Skill_Id,
									icg.TypeCurriculumGradeId,
									it.[Description] AS ItemLevelDescription,
									it.Value AS ItemLevelValue,
									bi.[Order],
									ROW_NUMBER() OVER ( PARTITION BY i.Id ORDER BY i.Id ) AS RowNumber2
							FROM    Item i WITH ( NOLOCK )
									LEFT JOIN BaseText bt WITH ( NOLOCK ) ON bt.Id = i.BaseText_Id AND bt.[State] = 1
									INNER JOIN ItemSkill ik WITH ( NOLOCK ) ON ik.Item_Id = i.Id 
									INNER JOIN ItemCurriculumGrade icg WITH ( NOLOCK ) ON icg.Item_Id = i.Id
									INNER JOIN EvaluationMatrix em WITH ( NOLOCK ) ON em.Id = i.EvaluationMatrix_Id
									LEFT JOIN ItemLevel it WITH ( NOLOCK ) ON i.ItemLevel_Id = it.Id AND it.[State] = 1
									INNER JOIN ItemSituation its WITH ( NOLOCK ) ON i.ItemSituation_Id = its.Id
									LEFT JOIN BlockItem bi WITH ( NOLOCK ) ON bi.Item_Id = i.Id AND bi.[State] = 1
							WHERE   i.[State] = 1 AND em.[State] = 1 AND ik.[State] = 1 AND  icg.[State] = 1 AND its.[State] = 1
							AND (i.ItemNarrated IS NULL OR i.ItemNarrated = 0)
							AND i.ItemCode = ISNULL(UPPER(LTRIM(RTRIM(@ItemCode))), UPPER(LTRIM(RTRIM(i.ItemCode))))

							AND (i.IsRestrict = 0 OR (i.IsRestrict = 1 AND @Global = 1))

							and i.ItemType_Id = ISNULL(@ItemTypeID, i.ItemType_Id)
							AND ((@ProficiencyStart IS NULL AND @ProficiencyEnd IS NULL) 
								OR (@ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NULL AND proficiency >= @ProficiencyStart)
								OR (@ProficiencyStart IS NULL AND @ProficiencyEnd IS NOT NULL AND proficiency <= @ProficiencyEnd)
								OR (@ProficiencyStart IS NOT NULL AND @ProficiencyEnd IS NOT NULL AND proficiency BETWEEN @ProficiencyStart AND @ProficiencyEnd)
							)
							AND (@Keywords IS NULL 
								OR (@Keywords IS NOT NULL AND EXISTS (SELECT SI.Items FROM dbo.SplitString(Keywords,';') SI INNER JOIN dbo.SplitString(@Keywords,',') I ON SI.Items = I.Items))
							)
							AND Discipline_Id = ISNULL(@DisciplineId, Discipline_Id)
							AND EvaluationMatrix_Id = ISNULL(@EvaluationMatrixId, EvaluationMatrix_Id)
							AND (@TypeCurriculumGrades IS NULL 
								OR (@TypeCurriculumGrades IS NOT NULL AND TypeCurriculumGradeId IN (SELECT Items FROM dbo.SplitString(@TypeCurriculumGrades,',')))
							)
							AND (@Skills IS NULL 
								OR Skill_Id IN (SELECT Items FROM dbo.SplitString(@Skills,','))
							)
							AND (@ItemLevel IS NULL 
								OR i.ItemLevel_Id IN (SELECT Items from dbo.SplitString(@ItemLevel,','))
							)
							AND LastVersion = 1
							AND its.[Description] = 'Aceito'
														
					) as R
	WHERE @Skills IS NOT NULL OR (@Skills IS NULL AND RowNumber2 = 1)
	ORDER BY ItemCode, ItemVersion

	SELECT @totalRecords = COUNT(ItemId) from #tmp
		
	SELECT	TOP (@pageSize) 
			ItemId, 
			ItemCode, 
			ItemVersion, 
			Revoked,
			BaseTextDescription, 
			[Statement], 
			MatrixDescription, 
			DescriptorSentence, 
			BaseTextId, 
			MatrixId,  
			LastVersion, 
			ItemLevelDescription, 
			ItemLevelValue,
			TypeCurriculumGradeId,  
			[Order], 
			RowNumber
	FROM	#tmp
	WHERE   RowNumber > ( @pageSize * ( @pageNumber ) )
	ORDER BY RowNumber
	
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



