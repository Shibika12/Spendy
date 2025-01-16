using Microsoft.AspNetCore.Components;
using ShibikaKhatri22068731CW1.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShibikaKhatri22068731CW1.Components.Pages
{
    public partial class EditTransaction
    {
        [Parameter]
        public int Id { get; set; }

        private Transaction transaction;
        private string errorMessage;
        private string newTag;
        private List<string> selectedTags = new List<string>();
        private List<string> availableTags = new List<string>();

        private readonly List<string> _defaultTags = new List<string>
        {
            "Yearly", "Monthly", "Food", "Drinks", "Clothes", "Gadgets",
            "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
        };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Load the transaction
                var transactions = await TransactionService.LoadTransactionsAsync();
                transaction = transactions.Find(t => t.TransactionId == Id);

                if (transaction != null && !string.IsNullOrEmpty(transaction.Tags))
                {
                    selectedTags = new List<string>(transaction.Tags.Split(','));
                }

                // Load available tags
                availableTags = await TagService.GetAllTagsAsync();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading transaction: {ex.Message}";
            }
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                transaction.Tags = string.Join(",", selectedTags);
                await TransactionService.SaveTransactionAsync(transaction);
                Nav.NavigateTo("/transactions", false);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error saving transaction: {ex.Message}";
            }
        }

        private void ToggleTag(string tag)
        {
            if (selectedTags.Contains(tag))
            {
                selectedTags.Remove(tag);
            }
            else
            {
                selectedTags.Add(tag);
            }
        }

        private async Task AddCustomTag()
        {
            if (string.IsNullOrWhiteSpace(newTag)) return;

            try
            {
                await TagService.AddCustomTagAsync(newTag);
                if (!availableTags.Contains(newTag))
                {
                    availableTags.Add(newTag);
                }
                newTag = string.Empty;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error adding tag: {ex.Message}";
            }
        }

        private async Task DeleteCustomTag(string tag)
        {
            try
            {
                await TagService.DeleteCustomTagAsync(tag);
                availableTags.Remove(tag);
                selectedTags.Remove(tag);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting tag: {ex.Message}";
            }
        }
    }
}