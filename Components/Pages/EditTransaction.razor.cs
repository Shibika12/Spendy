using Microsoft.AspNetCore.Components;
using ShibikaKhatri22068731CW1.Model;
namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class EditTransaction : ComponentBase
{
    [Parameter]
    public int TransactionId { get; set; }

    private Transaction transaction;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var transactions = await TransactionService.LoadTransactionsAsync();
            transaction = transactions.FirstOrDefault(t => t.TransactionId == TransactionId);

            if (transaction == null)
            {
                Nav.NavigateTo("/transactions"); // Redirect if not found
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transaction: {ex.Message}");
        }
    }

    private async Task SaveTransaction()
    {
        try
        {
            await TransactionService.SaveTransactionAsync(transaction);
            Nav.NavigateTo("/transactions"); // Redirect to the transactions page
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transaction: {ex.Message}");
        }
    }

    private void Cancel()
    {
        Nav.NavigateTo("/transactions");
    }
}