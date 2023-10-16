CREATE TABLE [dbo].[MonthlyBudgetInfo]
(
	[MonthlyBudgetInfoId] BIGINT NOT NULL CONSTRAINT PK_MonthlyBudgetInfo PRIMARY KEY IDENTITY, 
    [Description] NVARCHAR(150) NOT NULL
)

GO

CREATE UNIQUE INDEX [UI_MonthlyBudgetInfo_Description] ON [dbo].[MonthlyBudgetInfo] ([Description])
