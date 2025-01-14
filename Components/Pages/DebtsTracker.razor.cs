using Microsoft.AspNetCore.Components;

using ShibikaKhatri22068731CW1.Model;

namespace ShibikaKhatri22068731CW1.Components.Pages
{
    public partial class DebtsTracker : ComponentBase
    {
        private List<Debt> debts = new List<Debt>();
        private int totalDebtsCount;
        private int clearedDebtsCount;
        private int remainingDebtsCount;
        private float totalDebtsAmount;
        private float clearedDebtsAmount;
        private float remainingDebtsAmount;
        
        // Method to load debts
        protected override async Task OnInitializedAsync()
        {
            debts = await DebtService.LoadDebtsAsync();
            UpdateDebtSummary();
        }
        // Update summary information
        private void UpdateDebtSummary()
        {
            totalDebtsCount = debts.Count;
            totalDebtsAmount = debts.Sum(d => d.InitialAmount);
            
            clearedDebtsCount = debts.Count(d => d.Status == "Cleared");
            clearedDebtsAmount = debts
                .Where(d => d.Status == "Cleared")
                .Sum(d => d.InitialAmount); 
            remainingDebtsCount = debts.Count(d => d.Status == "Pending");
            remainingDebtsAmount = debts.Where(d => d.Status == "Pending").Sum(d => d.Amount);
        }
        // Method to clear a debt
        private async Task ClearDebt(int debtId, float creditAmount)
        {
            try
            {
                // Clear debt using the service
                await DebtService.ClearDebtAsync(debtId, creditAmount);
                // Update only the cleared debt in the local list
                var debt = debts.FirstOrDefault(d => d.DebtId == debtId);
                if (debt != null)
                {
                    if (creditAmount >= debt.Amount)
                    {
                        debt.Status = "Cleared";
                        debt.Amount = 0;
                        debt.ClearedFrom = $"Credit {creditAmount}";
                    }
                    else
                    {
                        debt.Amount -= creditAmount;
                        debt.ClearedFrom = $"Partial Credit {creditAmount}";
                    }
                }
                // Refresh summary
                UpdateDebtSummary();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing debt: {ex.Message}");
            }
        }
        private void NavigateToAddDebtEntry()
        {
            NavigationManager.NavigateTo("/debt-entry");
        }
    }
}

