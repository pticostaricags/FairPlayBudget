namespace FairPlayBudget.Common
{
    public class Constants
    {
        public class AppRoutes
        {
            public const string CreateExpense = $"/Expenses/{nameof(CreateExpense)}";
            public const string MyExpenses = $"/Expenses/{nameof(MyExpenses)}";

            public const string CreateIncome = $"/Expenses/{nameof(CreateIncome)}";

            public const string MyBalance = $"Balance/{nameof(MyBalance)}";
        }
    }
}
