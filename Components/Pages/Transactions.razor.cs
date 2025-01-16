using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShibikaKhatri22068731CW1.Services.Interface;
using ShibikaKhatri22068731CW1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShibikaKhatri22068731CW1.Components.Pages
{
    public partial class Transactions : ComponentBase
    {
        private List<Transaction> transactions = new List<Transaction>();
        private List<Transaction> FilteredTransactions = new List<Transaction>();

        private string PreferredCurrency => UserService.PreferredCurrency;

        // Updated transaction summary fields
        private (int TotalTransactionCount, decimal TotalInflows, decimal TotalOutflows, decimal NetTransactions,
            decimal Balance) _transactionSummary;

        private readonly List<string> _defaultTags = new List<string>
        {
            "Yearly", "Monthly", "Food", "Drinks", "Clothes", "Gadgets",
            "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
        };

        private string SearchTitle;
        private string FilterType;
        private string FilterTags;
        private DateTime? StartDate;
        private DateTime? EndDate;

        // Sorting variables
        private string SortBy { get; set; } = "Date";
        private bool IsAscending { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            // Fetch all transactions
            await LoadTransactionsAsync();
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                transactions = await TransactionService.LoadTransactionsAsync();
                FilteredTransactions = transactions;
                // Recalculate the transaction summary
                _transactionSummary = TransactionService.GetTransactionSummary(transactions);
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

        private void NavigateToAddPage()
        {
            Nav.NavigateTo("/addtransaction");
        }

        private async Task DeleteTransaction(int transactionId)
        {
            await TransactionService.DeleteTransactionAsync(transactionId);
            // Clear all filters
            await ClearFilters();

            // Fetch all transactions
            await LoadTransactionsAsync();
        }

        private async Task ApplyFiltersAndSearch()
        {
            var filtered = await TransactionService.FilterTransactionsAsync(StartDate, EndDate, FilterType, FilterTags);
            if (!string.IsNullOrEmpty(SearchTitle))
            {
                filtered = filtered.Where(t => t.Title.Contains(SearchTitle, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            FilteredTransactions = filtered;
            // Recalculate the transaction summary with filtered transactions
            _transactionSummary = TransactionService.GetTransactionSummary(FilteredTransactions);
        }

        private async Task ApplySorting()
        {
            try
            {
                if (IsAscending)
                {
                    FilteredTransactions = await TransactionService.SortTransactionsAscending(SortBy);
                }
                else
                {
                    FilteredTransactions = await TransactionService.SortTransactionsDescending(SortBy);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sorting transactions: {ex.Message}");
            }
        }

        private async Task ClearFilters()
        {
            SearchTitle = "";
            FilterTags = "";
            FilterType = "";
            StartDate = null;
            EndDate = null;
            SortBy = "Date";
            IsAscending = true;
            await LoadTransactionsAsync();
        }

        private async Task DeleteTag(string tag)
        {
            var confirmed = await JS.InvokeAsync<bool>("confirm", $"Are you sure you want to delete the tag '{tag}'?");
            if (confirmed)
            {
                try
                {
                    await TagService.DeleteCustomTagAsync(tag);
                    // Refresh transactions to update tags
                    await LoadTransactionsAsync();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("alert", $"Error deleting tag: {ex.Message}");
                }
            }
        }

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
                    return "$"; // Default to USD if no currency is selected
            }
        }

        // Helper to format amounts with the currency symbol
        private string FormatAmount(decimal amount)
        {
            return $"{GetCurrencySymbol()} {amount:N2}";
        }
        private async Task ExportToCsvAsync()
        {
            try
            {
                var transactions = await TransactionService.GetTransactionsAsync();
                if (transactions.Any())
                {
                    var filePath = await TransactionService.ExportToCsvAsync(transactions);
                    await JS.InvokeVoidAsync("alert", $"Transactions exported to: {filePath}");
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "No transactions to export.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"Error exporting transactions: {ex.Message}");
            }
        }
    }
    
}
