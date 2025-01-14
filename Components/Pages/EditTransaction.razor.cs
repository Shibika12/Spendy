using Microsoft.AspNetCore.Components;
using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services;
namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class EditTransaction : ComponentBase
{
    [Parameter]
    public int TransactionId { get; set; }
    private List<string> availableTags = new List<string>();
    private List<string> selectedTags = new List<string>();
    private string newTag = "";
    private string errorMessage;
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
            // Load available tags
            availableTags = await TagService.GetAllTagsAsync();
            // Initialize selected tags from transaction
            if (!string.IsNullOrEmpty(transaction.Tags))
            {
                selectedTags = transaction.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transaction: {ex.Message}");
        }
    }
    private void ToggleTag(string tag)
    {
        if (selectedTags.Contains(tag))
            selectedTags.Remove(tag);
        else
            selectedTags.Add(tag);
        
        transaction.Tags = string.Join(",", selectedTags);
    }
    private async Task AddCustomTag()
    {
        if (!string.IsNullOrWhiteSpace(newTag))
        {
            await TagService.AddCustomTagAsync(newTag);
            availableTags = await TagService.GetAllTagsAsync();
            selectedTags.Add(newTag);
            transaction.Tags = string.Join(",", selectedTags);
            newTag = "";
        }
    }
    private async Task SaveTransaction()
    {
        try
        {
                try
                {
                    await TransactionService.SaveTransactionAsync(transaction);
                    Nav.NavigateTo("/transactions");
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
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