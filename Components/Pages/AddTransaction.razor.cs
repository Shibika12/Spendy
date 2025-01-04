using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class AddTransaction : ComponentBase
{
    private Transaction newTransaction = new Transaction();


    private string errorMessage;

    private async Task AddNewTransaction()
    {

        try
        {
            var existingTransactions = await TransactionService.LoadTransactionsAsync();

            // If it's an outflow, check the balance first
            if (newTransaction.Type == "Debit")
            {
                var (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance, isSufficientBalance) = 
                    TransactionService.CalculateTransactionSums(existingTransactions);

                // Check if there's enough balance for this outflow
                if (newTransaction.Amount > balance)
                {
                    errorMessage = $"Insufficient balance. Current balance: {balance:C}, Attempted outflow: {newTransaction.Amount:C}";
                    return; // Don't proceed with adding the transaction
                }
            }

            int nextTransactionId = existingTransactions.Count > 0
                ? existingTransactions.Max(t => t.TransactionId) + 1
                : 1;

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