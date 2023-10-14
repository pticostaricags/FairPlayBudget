CREATE VIEW [dbo].[vwBalance]
AS 
SELECT 
E.OwnerId AS OwnerId, 
E.AmountInUSD AS AmountInUSD,
CAST('Debit' AS NVARCHAR(10)) AS TransactionType,
E.ExpenseDateTime AS [DateTime],
E.[Description] AS [Description]
FROM [Expense] E
UNION
SELECT 
I.OwnerId AS OwnerId, 
I.AmountInUSD AS AmountInUSD,
CAST('Credit' AS NVARCHAR(10)) AS TransactionType,
I.IncomeDateTime AS [DateTime],
I.[Description] AS [Description]
FROM [Income] I