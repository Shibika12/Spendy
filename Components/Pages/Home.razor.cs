using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShibikaKhatri22068731CW1.Services;
using ShibikaKhatri22068731CW1.Model;


namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Home : ComponentBase
{
    
    private int Index = -1;
    public ChartOptions Options = new ChartOptions();

    public List<ChartSeries> Series = new List<ChartSeries>()
    {
        new ChartSeries() { Name = "Income", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
        new ChartSeries() { Name = "Expense", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
    };

    public string[] XAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

    private bool _hidePosition;
    private bool _loading;
    private decimal _totalInflows;
    private decimal _totalOutflows;
    private decimal _totalDebt;
    private decimal _totalClearedDebt;
    private decimal _remainingDebt;
    private decimal _balance;
    private bool _isSufficientBalance;

    private IEnumerable<Transaction> _elements = new List<Transaction>();
    private string _loggedInUsername = string.Empty;
    private string _message = string.Empty;

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

        var transactions = await TransactionService.LoadTransactionsAsync();
        var sums = TransactionService.CalculateTransactionSums(transactions);

        _totalInflows = sums.totalInflows;
        _totalOutflows = sums.totalOutflows;
        _totalDebt = sums.totalDebt;
        _totalClearedDebt = sums.totalClearedDebt;
        _remainingDebt = sums.remainingDebt;
        _balance = sums.balance;
        _isSufficientBalance = sums.isSufficientBalance;
        _elements = transactions.OrderByDescending(t => t.Amount).Take(4);





    }
}