using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class AddTransaction : ComponentBase
{
    private Transaction newTransaction = new Transaction();
    private readonly TransactionService transactionService = new TransactionService();

    private string errorMessage;

    private async Task AddNewTransaction()
    {
        try
        {
            var existingTransactions = await transactionService.LoadTransactionsAsync();
            int nextTransactionId = existingTransactions.Count > 0
                ? existingTransactions.Max(t => t.TransactionId) + 1
                : 1; // Start with 1 if no transactions exist

            // Set the TransactionId dynamically
            newTransaction.TransactionId = nextTransactionId;
            await TransactionService.SaveTransactionAsync(newTransaction);
            newTransaction = new Transaction(); // Reset the form
            
            Nav.NavigateTo("/transactions");
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred while adding the transaction: {ex.Message}";
        }
    }
}