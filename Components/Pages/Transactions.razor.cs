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

        private decimal _totalInflows;
        private decimal _totalOutflows;

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
            try
            {
                // Call the delete service
                await TransactionService.DeleteTransactionAsync(transactionId);

                // Reload transactions
                await LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting transaction: {ex.Message}");
            }
        }

        private async Task ApplyFiltersAndSearch()
        {
            try
            {
                var filtered = await TransactionService.FilterTransactionsAsync(StartDate, EndDate, FilterType, FilterTags);

                if (!string.IsNullOrEmpty(SearchTitle))
                {
                    filtered = filtered.Where(t => t.Title.Contains(SearchTitle, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                FilteredTransactions = filtered;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filtering transactions: {ex.Message}");
            }
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
            SearchTitle = string.Empty;
            FilterTags = string.Empty;
            FilterType = string.Empty;
            StartDate = null;
            EndDate = null;
            SortBy = "Date";
            IsAscending = true;

            await LoadTransactionsAsync();
        }
    }
}
