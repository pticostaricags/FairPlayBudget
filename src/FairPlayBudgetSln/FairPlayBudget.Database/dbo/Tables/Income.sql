CREATE TABLE [dbo].[Income]
(
	[IncomeId] BIGINT NOT NULL CONSTRAINT PK_Income PRIMARY KEY IDENTITY,
    [IncomeDateTime] DATETIMEOFFSET NOT NULL, 
    [Description] NVARCHAR(50) NOT NULL, 
    [AmountInUSD] MONEY NOT NULL, 
    [OwnerId] NVARCHAR(450) NOT NULL, 
    [MonthlyBudgetInfoId] BIGINT NULL, 
    CONSTRAINT [FK_Income_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_Income_MonthlyBudgetInfo] FOREIGN KEY ([MonthlyBudgetInfoId]) REFERENCES [MonthlyBudgetInfo]([MonthlyBudgetInfoId]), 
)
