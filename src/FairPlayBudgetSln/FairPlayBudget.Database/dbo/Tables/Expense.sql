CREATE TABLE [dbo].[Expense]
(
	[ExpenseId] BIGINT NOT NULL CONSTRAINT PK_Expense PRIMARY KEY IDENTITY, 
    [ExpenseDateTime] DATETIMEOFFSET NOT NULL, 
    [Description] NVARCHAR(50) NOT NULL, 
    [AmountInUSD] MONEY NOT NULL, 
    [OwnerId] NVARCHAR(450) NOT NULL, 
    [MonthlyBudgetInfoId] BIGINT NULL, 
    CONSTRAINT [FK_Expense_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id]), 
    CONSTRAINT [FK_Expense_MonthlyBudgetInfo] FOREIGN KEY ([MonthlyBudgetInfoId]) REFERENCES [MonthlyBudgetInfo]([MonthlyBudgetInfoId])
)
