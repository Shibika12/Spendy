using ShibikaKhatri22068731CW1.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShibikaKhatri22068731CW1.Services;
namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Transactions : ComponentBase
{
    private List<Transaction> transactions = new List<Transaction>();
    private readonly TransactionService transactionService = new TransactionService();

    protected override async Task OnInitializedAsync()
    {
        // Get all transactions from the service
        try
        {
            // Load transactions from the CSV file
            transactions = await transactionService.LoadTransactionsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions: {ex.Message}");
        }
        
    }
    private void NavigateToEditPage(int transactionId)
    {
        Nav.NavigateTo($"/edit-transaction/{transactionId}");
    }

    private async Task DeleteTransaction(int transactionId)
    {
        var confirmed = await JS.InvokeAsync<bool>("confirm", "Are you sure you want to delete this transaction?");
        if (confirmed)
        {
            try
            {
                await TransactionService.DeleteTransactionAsync(transactionId);
                // Reload the transactions list after deletion
                transactions = await TransactionService.LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting transaction: {ex.Message}");
            }
        }
    }
}