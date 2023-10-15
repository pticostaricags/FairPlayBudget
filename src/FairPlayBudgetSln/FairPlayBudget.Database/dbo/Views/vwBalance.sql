CREATE VIEW [dbo].[vwBalance]
AS 
SELECT 
E.OwnerId AS OwnerId, 
E.Amount AS Amount,
CAST('Debit' AS NVARCHAR(10)) AS TransactionType,
E.ExpenseDateTime AS [DateTime],
E.[Description] AS [Description],
C.CurrencyId AS CurrencyId,
C.[Description] AS Currency,
MBI.[Description] AS MonthlyBudgetDescription
FROM [Expense] E
LEFT JOIN MonthlyBudgetInfo MBI ON MBI.MonthlyBudgetInfoId = E.MonthlyBudgetInfoId
INNER JOIN Currency C ON C.CurrencyId = E.CurrencyId
UNION
SELECT 
I.OwnerId AS OwnerId, 
I.Amount AS Amount,
CAST('Credit' AS NVARCHAR(10)) AS TransactionType,
I.IncomeDateTime AS [DateTime],
I.[Description] AS [Description],
I.CurrencyId AS CurrencyId,
C.[Description] AS Currency,
MBI.[Description] AS MonthlyBudgetDescription
FROM [Income] I
LEFT JOIN MonthlyBudgetInfo MBI ON MBI.MonthlyBudgetInfoId = I.MonthlyBudgetInfoId
INNER JOIN Currency C ON C.CurrencyId = I.CurrencyId