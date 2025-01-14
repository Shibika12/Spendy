using ShibikaKhatri22068731CW1.Model;

namespace ShibikaKhatri22068731CW1.Services.Interface
{

    public interface IDebtService
    {


        // Save debts to file
        Task SaveDebtsAsync(List<Debt> debts);

        // Add a new debt
        Task AddDebtAsync(Debt debt);

        // Clear debt using an inflow (credit)
        Task ClearDebtAsync(int debtId, float creditAmount);

        // Highlight pending debts
        Task<List<Debt>> GetPendingDebtsAsync();


        // Get total debt (outstanding amount)
        Task<float> GetTotalDebtAsync();


        // Get the highest debt transaction
        Task<Debt> GetHighestDebtAsync();


        // Get the lowest debt transaction
        Task<Debt> GetLowestDebtAsync();
        Task<List<Debt>> LoadDebtsAsync();
    }
}
