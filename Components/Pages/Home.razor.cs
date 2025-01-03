using Microsoft.AspNetCore.Components;

namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Home : ComponentBase
{
    /*private decimal _totalInflows;
    private decimal _totalOutflows;
    private decimal _totalDebt;
    private decimal _totalClearedDebt;
    private decimal _remainingDebt;
    private decimal _balance;*/
    
    private string _loggedInUsername = string.Empty;

    private string _message = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        
        if (UserService.IsLoggedIn)
        {
            _loggedInUsername = UserService.LoggedInUser;
            _message = $"Welcome, {_loggedInUsername}!";
        }
        
        // Display a welcome message for the single user
        _message = "You are not logged in.";
        /* var transactions = await transactionService.LoadTransactionsAsync();
         var sums = transactionService.CalculateTransactionSums(transactions);
         _totalInflows = sums.totalInflows;
         _totalOutflows = sums.totalOutflows;
         _totalDebt = sums.totalDebt;
         _totalClearedDebt = sums.totalClearedDebt;
         _remainingDebt = sums.remainingDebt;
         _balance = sums.balance;*/
        
    }
    

    

    
}