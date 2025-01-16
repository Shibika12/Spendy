using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShibikaKhatri22068731CW1.Services;
using ShibikaKhatri22068731CW1.Model;


namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Home : ComponentBase
{
    
    
    private DateTime? startDate;
    private DateTime? endDate;
    
    private List<Transaction> filteredTransactions = new List<Transaction>();
    public string[] Colors { get; set; }
    public List<ChartSeries> Series { get; set; } = new List<ChartSeries>();
    public string[] XAxisLabels { get; set; } = Array.Empty<string>();
    private bool _hidePosition;
    private bool _loading;
    private List<Transaction> TopInflows { get; set; } = new();
    private List<Transaction> LowestInflows { get; set; } = new();
    private List<Transaction> TopOutflows { get; set; } = new();
    private List<Transaction> LowestOutflows { get; set; } = new();
    
    private List<Transaction> PendingDebts { get; set; } = new();
    private List<Transaction> ClearedDebts { get; set; } = new();

    private decimal _totalInflows;
    private decimal _totalOutflows;
    private float _totalDebt;
    private float _totalClearedDebt;
    private float _remainingDebt;
    private decimal _balance;
    private bool _isSufficientBalance;
    
    private string _loggedInUsername = string.Empty;
    private string _message = string.Empty;
    
    // Accessing the preferred currency from UserService
    private string PreferredCurrency => UserService.PreferredCurrency;

    protected override async Task OnInitializedAsync()
    {
        if (UserService.IsLoggedIn)
        {
            _loggedInUsername = UserService.LoggedInUser;
            _message = $"Welcome,Shibika {_loggedInUsername}!";
        }
        else
        {
            _message = "You are not logged in.";
        }
        try
        {
            var transactions = await TransactionService.GetTransactionsAsync();
            // Calculate transaction summaries
            var sums = TransactionService.CalculateTransactionSums(transactions);
            _totalInflows = sums.totalInflows;
            _totalOutflows = sums.totalOutflows;
            _totalDebt = (float)sums.totalDebt;
            _totalClearedDebt = (float)sums.totalClearedDebt;
            _remainingDebt = (float)sums.remainingDebt;
            _balance = sums.balance;
            // Fetch transaction extremes
            var (topInflows, lowestInflows, topOutflows, lowestOutflows) =
                TransactionService.GetTransactionExtremes(transactions, 5);
            TopInflows = topInflows;
            LowestInflows = lowestInflows;
            TopOutflows = topOutflows;
            LowestOutflows = lowestOutflows;
            // Prepare debts
            PendingDebts = await TransactionService.GetPendingDebtsAsync();
            ClearedDebts = await TransactionService.GetClearedDebtsAsync();
            // Prepare chart data
            XAxisLabels = transactions
                .Select(t => t.Date?.ToString("MMM") ?? string.Empty)
                .Distinct()
                .ToArray();
            Series = new List<ChartSeries>
            {
                new ChartSeries
                {
                    Name = "Credit",
                    Data = TopInflows.Select(t => (double)t.Amount).ToArray()
                },
                new ChartSeries
                {
                    Name = "Debit",
                    Data = TopOutflows.Select(t => (double)t.Amount).ToArray()
                }
            };
            filteredTransactions = transactions; // Initially, show all transactions
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing Home component: {ex.Message}");
        }
        finally
        {
            _loading = false;
        }
    }
    // Method to apply date range filtering
    private async Task FilterTransactions()
    {
        var filtered = await TransactionService.FilterTransactionsByDateAsync(startDate, endDate);
        // Update the lists with the filtered transactions
        UpdateDashboardData(filtered);
    }
    // Method to clear the date filters and show all transactions again
    private async Task ClearFilter()
    {
        startDate = null;
        endDate = null;
        // Reload all transactions without filtering
        var allTransactions = await TransactionService.GetTransactionsAsync();
        UpdateDashboardData(allTransactions);
    }
    // Method to update the dashboard data with filtered/complete transactions
    private async Task UpdateDashboardData(List<Transaction> transactions)
    {

        var sums = TransactionService.CalculateTransactionSums(transactions);
        _totalInflows = sums.totalInflows;
        _totalOutflows = sums.totalOutflows;
        _totalDebt = (float)sums.totalDebt;
        _totalClearedDebt = (float)sums.totalClearedDebt;
        _remainingDebt = (float)sums.remainingDebt;
        _balance = sums.balance;
        var (topInflows, lowestInflows, topOutflows, lowestOutflows) =
            TransactionService.GetTransactionExtremes(transactions, 5);
        TopInflows = topInflows;
        LowestInflows = lowestInflows;
        TopOutflows = topOutflows;
        LowestOutflows = lowestOutflows;
        
        PendingDebts = await TransactionService.GetPendingDebtsAsync();
        ClearedDebts = await TransactionService.GetClearedDebtsAsync();
        // Prepare chart data
        XAxisLabels = transactions
            .Select(t => t.Date?.ToString("MMM") ?? string.Empty)
            .Distinct()
            .ToArray();
        Series = new List<ChartSeries>
        {
            new ChartSeries
            {
                Name = "Credit",
                Data = TopInflows.Select(t => (double)t.Amount).ToArray()
            },
            new ChartSeries
            {
                Name = "Debit",
                Data = TopOutflows.Select(t => (double)t.Amount).ToArray()
            }
        };
    }
    // Method to get the correct currency symbol based on the preferred currency
    private string GetCurrencySymbol()
    {
        switch (PreferredCurrency)
        {
            case "USD":
                return "$";
            case "EUR":
                return "€";
            case "NRS":
                return "Rs";
            default:
                return "$";  // Default to USD if no currency is selected
        }
    }
    

    // Helper to format amounts with the currency symbol
    private string FormatAmount(decimal amount)
    {
        return $"{GetCurrencySymbol()} {amount:N2}";
    }
}
