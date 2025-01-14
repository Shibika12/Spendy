using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services.Interface;
using System.Linq;

namespace ShibikaKhatri22068731CW1.Services
{
   public class DebtService : IDebtService
    {
        private readonly string _debtsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "debts.csv");
        private readonly string _transactionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "transactions.csv");

        // Load all debts from file
        public async Task<List<Debt>> LoadDebtsAsync()
        {
            var debts = new List<Debt>();
            try
            {
                if (File.Exists(_debtsFilePath))
                {
                    var lines = await File.ReadAllLinesAsync(_debtsFilePath);
                    foreach (var line in lines.Skip(1)) // Skipping header
                    {
                        var columns = line.Split(',');
                        debts.Add(new Debt
                        {
                            DebtId = int.Parse(columns[0]),
                            SourceOfDebt = columns[1],
                            Amount = float.Parse(columns[2]),
                            DueDate = DateTime.Parse(columns[3]),
                            Status = columns[4],
                            ClearedFrom = columns[5]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading debts: {ex.Message}");
            }
            return debts;
        }

        // Save debts to file
        public async Task SaveDebtsAsync(List<Debt> debts)
        {
            try
            {
                using (var writer = new StreamWriter(_debtsFilePath, false))
                {
                    // Write header
                    await writer.WriteLineAsync("DebtId,SourceOfDebt,Amount,DueDate,Status,ClearedFrom");
                    foreach (var debt in debts)
                    {
                        string csvRow = $"{debt.DebtId},{debt.SourceOfDebt},{debt.Amount},{debt.DueDate},{debt.Status},{debt.ClearedFrom}";
                        await writer.WriteLineAsync(csvRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving debts: {ex.Message}");
            }
        }

        // Add a new debt
        public async Task AddDebtAsync(Debt debt)
        {
            var debts = await LoadDebtsAsync();
            debts.Add(debt);
            await SaveDebtsAsync(debts);
        }

        // Clear debt using an inflow (credit)
        public async Task ClearDebtAsync(int debtId, float creditAmount)
        {
            var debts = await LoadDebtsAsync();
            var debt = debts.FirstOrDefault(d => d.DebtId == debtId);
            
            if (debt != null && creditAmount > 0)
            {
                // Clear the debt from the credit
                if (creditAmount >= debt.Amount)
                {
                    debt.Status = "Cleared";
                    debt.ClearedFrom = $"Credit Transaction ID: {debtId}"; // This could be updated with actual credit transaction reference
                    debt.Amount = 0; // Mark the debt as fully cleared
                }
                else
                {
                    // Partially clear the debt
                    debt.Amount -= creditAmount;
                    debt.Status = "Pending";
                    debt.ClearedFrom = $"Partial Credit {creditAmount}";
                }

                await SaveDebtsAsync(debts);
            }
            else
            {
                throw new InvalidOperationException("Debt not found or invalid credit amount.");
            }
        }

        // Highlight pending debts (could be used to show in UI)
        public async Task<List<Debt>> GetPendingDebtsAsync()
        {
            var debts = await LoadDebtsAsync();
            return debts.Where(d => d.Status == "Pending").ToList();
        }

        // Calculate total remaining debt
        public async Task<float> GetTotalDebtAsync()
        {
            var debts = await LoadDebtsAsync();
            return debts.Where(d => d.Status == "Pending").Sum(d => d.Amount);
        }

        // Get the highest debt transaction (still pending)
        public async Task<Debt> GetHighestDebtAsync()
        {
            var debts = await LoadDebtsAsync();
            return debts.Where(d => d.Status == "Pending").OrderByDescending(d => d.Amount).FirstOrDefault();
        }

        // Get the lowest debt transaction (still pending)
        public async Task<Debt> GetLowestDebtAsync()
        {
            var debts = await LoadDebtsAsync();
            return debts.Where(d => d.Status == "Pending").OrderBy(d => d.Amount).FirstOrDefault();
        }
    }
}