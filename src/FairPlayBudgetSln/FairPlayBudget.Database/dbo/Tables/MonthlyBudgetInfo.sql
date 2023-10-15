CREATE TABLE [dbo].[MonthlyBudgetInfo]
(
	[MonthlyBudgetInfoId] BIGINT NOT NULL CONSTRAINT PK_MonthlyBudgetInfo PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(150) NOT NULL
)
