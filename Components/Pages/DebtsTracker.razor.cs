
using Microsoft.AspNetCore.Components;

using ShibikaKhatri22068731CW1.Model;

namespace ShibikaKhatri22068731CW1.Components.Pages
{
     public partial class DebtsTracker : ComponentBase
    {
       
       private List<Transaction> pendingDebts = new();
       private string PreferredCurrency => UserService.PreferredCurrency;
    private List<Transaction> clearedDebts = new();
    private string activeTab = "pending";
    private decimal totalPendingDebt;
    private decimal totalClearedDebt;
    private decimal remainingDebt;
    private int pendingDebtsCount;
    private int clearedDebtsCount;
    private DateTime? nearestDueDate;
    private int? daysUntilNextDue;
    private decimal debtResolutionRate;
       
    protected override async Task OnInitializedAsync()
    {
        await LoadDebts();
        CalculateMetrics();
    }
    private async Task LoadDebts()
    {
        pendingDebts = await TransactionService.GetPendingDebtsAsync();
        clearedDebts = await TransactionService.GetClearedDebtsAsync();
    }
    private void CalculateMetrics()
    {
        totalPendingDebt = pendingDebts.Sum(d => d.Amount);
        totalClearedDebt = clearedDebts.Sum(d => d.Amount);
        pendingDebtsCount = pendingDebts.Count;
        clearedDebtsCount = clearedDebts.Count;
        
        // Calculate remaining debt as total pending debt minus cleared debt
        remainingDebt =  totalPendingDebt - totalClearedDebt;
        if (remainingDebt <= 0)
        {
            remainingDebt = 0;
        }
        var totalDebts = pendingDebtsCount + clearedDebtsCount;
        debtResolutionRate = totalDebts == 0 ? 0 : (decimal)clearedDebtsCount / totalDebts;
        
        nearestDueDate = pendingDebts
            .Where(d => d.DueDate.HasValue)
            .OrderBy(d => d.DueDate)
            .FirstOrDefault()?.DueDate;

        
        if (nearestDueDate.HasValue)
        {
            daysUntilNextDue = (nearestDueDate.Value - DateTime.Today).Days;
        }
    }
    
    private void SwitchTab(string tab)
    {
        activeTab = tab;
    }
    
    private string GetDueDateClass(DateTime? dueDate)
    {
        if (!dueDate.HasValue) return "text-muted";
        
        var daysUntilDue = (dueDate.Value - DateTime.Today).Days;
        return daysUntilDue switch
        {
            < 0 => "text-danger",
            < 7 => "text-warning",
            _ => "text-success"
        };
    }
    private async Task ClearDebt(int transactionId)
    {
        try
        {
            await TransactionService.ClearDebtAsync(transactionId); // Update debt status
            await LoadDebts(); // Refresh both pending and cleared debts lists
            CalculateMetrics(); // Recalculate metrics
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing debt: {ex.Message}");
            // Optionally, add user-facing notifications here
        }
    }

    
    private async Task DeleteDebt(int transactionId)
    {
        try
        {
            await TransactionService.DeleteTransactionAsync(transactionId);
            await LoadDebts();
            CalculateMetrics();
        }
        catch (Exception ex)
        {
            // Add error handling/notification here
            Console.WriteLine($"Error deleting debt: {ex.Message}");
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
                return "$";  // Default to USD if no currency is selected
        }
    }
    // Helper to format amounts with the currency symbol
    private string FormatAmount(decimal amount)
    {
        return $"{GetCurrencySymbol()} {amount:N2}";
    } 
    }
}