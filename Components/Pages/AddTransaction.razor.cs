using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ShibikaKhatri22068731CW1.Components.Pages
{
    public partial class AddTransaction : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; } // Inject IJSRuntime here

        private Transaction newTransaction = new Transaction();
        private List<string> availableTags = new List<string>();
        private List<string> selectedTags = new List<string>();
        private string newTag = "";

        private string errorMessage;
        private readonly List<string> _defaultTags = new List<string>
        {
            "Yearly", "Monthly", "Food", "Drinks", "Clothes", "Gadgets",
            "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
        };

        protected override async Task OnInitializedAsync()
        {
            availableTags = await TagService.GetAllTagsAsync();
            newTransaction.Date = DateTime.Today; // Set default date
            newTransaction.IsCleared = false; // Default IsCleared for new transactions
        }

        private void ToggleTag(string tag)
        {
            if (selectedTags.Contains(tag))
                selectedTags.Remove(tag);
            else
                selectedTags.Add(tag);

            newTransaction.Tags = string.Join(",", selectedTags);
        }

        private async Task AddCustomTag()
        {
            if (!string.IsNullOrWhiteSpace(newTag))
            {
                await TagService.AddCustomTagAsync(newTag);
                availableTags = await TagService.GetAllTagsAsync();
                selectedTags.Add(newTag);
                newTransaction.Tags = string.Join(",", selectedTags);
                newTag = "";
            }
        }

        private async Task AddNewTransaction()
        {
            try
            {
                var existingTransactions = await TransactionService.LoadTransactionsAsync();

                // If it's an outflow, check the balance first
                if (newTransaction.Type == "Debit")
                {
                    var (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance,
                            isSufficientBalance) = 
                        TransactionService.CalculateTransactionSums(existingTransactions);

                   
                    if (newTransaction.Amount > balance)
                    {
                        errorMessage =
                            $"Insufficient balance. Current balance: {balance}, Attempted outflow: {newTransaction.Amount}";
                        return; // Don't proceed with adding the transaction
                    }
                }

                int nextTransactionId = existingTransactions.Count > 0
                    ? existingTransactions.Max(t => t.TransactionId) + 1
                    : 1;

                newTransaction.TransactionId = nextTransactionId;
                // If it's a debt transaction, set IsCleared to false explicitly
                if (newTransaction.Type == "Debt")
                {
                    newTransaction.IsCleared = false;
                }
                await TransactionService.SaveTransactionAsync(newTransaction);
                newTransaction = new Transaction(); // Reset the form

                Nav.NavigateTo("/transactions");
            }
            catch (Exception ex)
            {
                errorMessage = $"An error occurred while adding the transaction: {ex.Message}";
            }
        }

        private async Task DeleteCustomTag(string tag)
        {
            try
            {
                if (!_defaultTags.Contains(tag))
                {
                    var confirmed = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to delete the tag '{tag}'?");
                    if (confirmed)
                    {
                        await TagService.DeleteCustomTagAsync(tag);
                        // Remove from selected tags if it was selected
                        if (selectedTags.Contains(tag))
                        {
                            selectedTags.Remove(tag);
                            newTransaction.Tags = string.Join(",", selectedTags);
                        }
                        // Refresh available tags
                        availableTags = await TagService.GetAllTagsAsync();
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting tag: {ex.Message}";
            }
        }
    }
}
