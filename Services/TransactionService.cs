using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services.Interface;
using System.Text;

namespace ShibikaKhatri22068731CW1.Services;

public class TransactionService : ITransactionService
{
    private readonly string _transactionsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "transactions.csv");

    public async Task SaveTransactionAsync(Transaction transaction)

    {
        
        try
        {
            // Load all transactions from the file
            var transactions = await LoadTransactionsAsync();
            // If it's an outflow, verify sufficient balance, considering debt
            if (transaction.Type == "Debit")
            {
                var (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance, _) = CalculateTransactionSums(transactions);
                
                // For updates, exclude the existing transaction amount from the balance calculation
                var existingTrans = transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
                if (existingTrans != null && existingTrans.Type == "Debit")
                {
                    balance += existingTrans.Amount; // Add back the existing outflow amount
                }
                if (transaction.Amount > balance)
                {
                    throw new InvalidOperationException($"Insufficient balance. Current balance: {balance:C}, Attempted outflow: {transaction.Amount:C}");
                }
            }
            // Find and update the transaction in the list
            var existingTransaction = transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
            if (existingTransaction != null)
            {
                existingTransaction.Title = transaction.Title;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.Type = transaction.Type;
                existingTransaction.Date = transaction.Date;
                existingTransaction.Tags = transaction.Tags;
                existingTransaction.Note = transaction.Note;
                existingTransaction.SourceOfDebt = transaction.SourceOfDebt;  // New field
                existingTransaction.DueDate = transaction.DueDate;            // New field
                existingTransaction.IsCleared = transaction.IsCleared;        // New field
            }
            else
            {
                transactions.Add(transaction);
            }
            
            // Rewrite the updated transactions list to the file
            using (var writer = new StreamWriter(_transactionsFilePath, append: false))
            {
               
                // Write the header
                await writer.WriteLineAsync("TransactionId,Title,Amount,Type,Date,Tags,Note");
                foreach (var trans in transactions)
                {
                    string escapedTags = trans.Tags?.Replace(",", "||") ?? "";
                    string escapedNote = trans.Note?.Replace(",", "||") ?? "";
                    string escapedSourceOfDebt = trans.SourceOfDebt?.Replace(",", "||") ?? "";
                    string escapedDueDate = trans.DueDate?.ToString("yyyy-MM-dd") ?? ""; // Format the DueDate for CSV
                    string isCleared = trans.IsCleared.HasValue ? trans.IsCleared.Value.ToString() : ""; // Convert to string
                    string csvRow = $"{trans.TransactionId},{trans.Title},{trans.Amount},{trans.Type},{trans.Date:yyyy-MM-dd}," +
                                    $"{escapedTags},{escapedNote},{escapedSourceOfDebt},{escapedDueDate},{isCleared}";
                    await writer.WriteLineAsync(csvRow);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transaction: {ex.Message}");
            throw;
        }
    }
   
     public async Task<List<Transaction>> LoadTransactionsAsync()
    {
        try
        {
            if (!File.Exists(_transactionsFilePath))
            {
                return new List<Transaction>();
            }

            var transactions = new List<Transaction>();

            using (var reader = new StreamReader(_transactionsFilePath))
            {
                string headerLine = await reader.ReadLineAsync(); // Skip header line

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var fields = line.Split(',');

                    var transaction = new Transaction
                    {
                        TransactionId = int.Parse(fields[0]),
                        Title = fields[1],
                        Amount = decimal.Parse(fields[2]),
                        Type = fields[3],
                        Date = DateTime.Parse(fields[4]),
                        Tags = fields[5]?.Replace("||", ","),
                        Note = fields[6]?.Replace("||", ","),
                        SourceOfDebt = fields.Length > 7 ? fields[7]?.Replace("||", ",") : null, // Handle optional field
                        DueDate = fields.Length > 8 && !string.IsNullOrWhiteSpace(fields[8])
                            ? DateTime.Parse(fields[8])
                            : (DateTime?)null, // Parse or set to null
                        IsCleared = fields.Length > 9 && !string.IsNullOrWhiteSpace(fields[9])
                            ? bool.Parse(fields[9])
                            : (bool?)null // Parse or set to null
                    };

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions: {ex.Message}");
            return new List<Transaction>();
        }
    }
   
    public (decimal totalInflows, decimal totalOutflows, decimal totalDebt, decimal totalClearedDebt, decimal remainingDebt, decimal balance, bool isSufficientBalance) 
        CalculateTransactionSums(List<Transaction> transactions)
    {
        decimal totalInflows = 0;
        decimal totalOutflows = 0;
        decimal totalDebt = 0;
        decimal totalClearedDebt = 0;

        foreach (var transaction in transactions)
        {
            switch (transaction.Type)
            {
                case "Credit":
                    totalInflows += transaction.Amount;
                    break;
                case "Debit":
                    totalOutflows += transaction.Amount;
                    break;
                case "Debt":
                    if (transaction.IsCleared.GetValueOrDefault()) // Handle nullable bool
                    {
                        totalClearedDebt += transaction.Amount; // Only count pending debts
                    }
                    else
                    {
                        totalDebt += transaction.Amount; // Pending debts
                    }
                    break;
            }
        }

       
        decimal remainingDebt = totalDebt;
        decimal balance = totalInflows - totalOutflows + remainingDebt;
        bool isSufficientBalance = balance >= 0;

        return (totalInflows, totalOutflows, totalDebt, totalClearedDebt, remainingDebt, balance, isSufficientBalance);
    }
    public (Transaction highestInflow, Transaction lowestInflow, Transaction highestOutflow, Transaction lowestOutflow, Transaction highestDebt, Transaction lowestDebt) GetTransactionExtremes(List<Transaction> transactions)
    {
        var highestInflow = transactions.Where(t => t.Type == "Credit").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestInflow = transactions.Where(t => t.Type == "Credit").OrderBy(t => t.Amount).FirstOrDefault();
        var highestOutflow = transactions.Where(t => t.Type == "Debit").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestOutflow = transactions.Where(t => t.Type == "Debit").OrderBy(t => t.Amount).FirstOrDefault();
        var highestDebt = transactions.Where(t => t.Type == "Debt").OrderByDescending(t => t.Amount).FirstOrDefault();
        var lowestDebt = transactions.Where(t => t.Type == "Debt").OrderBy(t => t.Amount).FirstOrDefault();

        return (highestInflow, lowestInflow, highestOutflow, lowestOutflow, highestDebt, lowestDebt);
    }
    public async Task DeleteTransactionAsync(int transactionId)
    {
        try
        {
            var transactions = await LoadTransactionsAsync();

            // Find the transaction to delete
            var transactionToDelete = transactions.FirstOrDefault(t => t.TransactionId == transactionId);
            if (transactionToDelete != null)
            {
                transactions.Remove(transactionToDelete);

                // Rewrite the updated transactions list to the file
                using (var writer = new StreamWriter(_transactionsFilePath, append: false))
                {
                    // Write the header
                    await writer.WriteLineAsync("TransactionId,Title,Amount,Type,Date,Tags,Note,SourceOfDebt,DueDate,IsCleared");
                    foreach (var trans in transactions)
                    {
                        
                        string escapedTags = trans.Tags?.Replace(",", "||") ?? "";
                        string escapedNote = trans.Note?.Replace(",", "||") ?? "";
                        string escapedSourceOfDebt = trans.SourceOfDebt?.Replace(",", "||") ?? "";
                        string csvRow = $"{trans.TransactionId},{trans.Title}," +
                                        $"{trans.Amount},{trans.Type},{trans.Date}," +
                                        $"{escapedTags},{escapedNote},{escapedSourceOfDebt}," +
                                        $"{trans.DueDate?.ToString() ?? ""},{trans.IsCleared?.ToString() ?? ""}";
                        await writer.WriteLineAsync(csvRow);
                    }
                }
            }
            else
            {
                throw new Exception("Transaction not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            throw;
        }
    }
    public async Task<List<Transaction>> SearchTransactionsByTitleAsync(string title)
    {
        var transactions = await LoadTransactionsAsync();
        return transactions.Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public async Task<List<Transaction>> FilterTransactionsAsync(DateTime? startDate, DateTime? endDate, string Type, string Tags)
    {
        var transactions = await LoadTransactionsAsync();
        return transactions.Where(t =>
            (!startDate.HasValue || t.Date >= startDate.Value) &&
            (!endDate.HasValue || t.Date <= endDate.Value) &&
            (string.IsNullOrEmpty(Type) || t.Type.Equals(Type, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(Tags) || t.Tags.Equals(Tags, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }

    public async Task<List<Transaction>> SortTransactionsAsync(string sortBy, bool isAscending)
    {
        var transactions = await LoadTransactionsAsync();

        return sortBy.ToLower() switch
        {
            "date" => isAscending ? transactions.OrderBy(t => t.Date).ToList() : transactions.OrderByDescending(t => t.Date).ToList(),
            _ => transactions // Default: no sorting
        };
    }

    public async Task<List<Transaction>> SortTransactionsAscending(string sortBy)
    {
        var transactions = await LoadTransactionsAsync();

        // Sort transactions in ascending order by date
        return transactions.OrderBy(t => t.Date).ToList();
    }
    public async Task<List<Transaction>> SortTransactionsDescending(string sortBy)
    {
        var transactions = await LoadTransactionsAsync();

        // Sort transactions in descending order by date
        return transactions.OrderByDescending(t => t.Date).ToList();
    }
    public (List<Transaction> topInflows, List<Transaction> lowestInflows, 
        List<Transaction> topOutflows, List<Transaction> lowestOutflows) 
        GetTransactionExtremes(List<Transaction> transactions, int count = 5)
    {
        var topInflows = transactions
            .Where(t => t.Type == "Credit")
            .OrderByDescending(t => t.Amount)
            .Take(count)
            .ToList();

        var lowestInflows = transactions
            .Where(t => t.Type == "Credit")
            .OrderBy(t => t.Amount)
            .Take(count)
            .ToList();

        var topOutflows = transactions
            .Where(t => t.Type == "Debit")
            .OrderByDescending(t => t.Amount)
            .Take(count)
            .ToList();

        var lowestOutflows = transactions
            .Where(t => t.Type == "Debit")
            .OrderBy(t => t.Amount)
            .Take(count)
            .ToList();

        return (topInflows, lowestInflows, topOutflows, lowestOutflows);
    }
    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        return await LoadTransactionsAsync();
    }
    public (int TotalTransactionCount, decimal NetTransactions, decimal TotalInflows, decimal TotalOutflows, decimal Balance) 
        GetTransactionSummary(List<Transaction> transactions)
    {
        if (transactions == null || !transactions.Any())
        {
            return (0, 0, 0, 0, 0);
        }
        int totalTransactionCount = transactions.Count;
        decimal totalInflows = transactions.Where(t => t.Type == "Credit").Sum(t => t.Amount);
        decimal totalDebts = transactions.Where(t => t.Type == "Debt").Sum(t => t.Amount);
        decimal totalOutflows = transactions.Where(t => t.Type == "Debit").Sum(t => t.Amount);
        
        totalInflows += totalDebts;
        // Calculate net transactions and balance
        decimal netTransactions = totalInflows + totalDebts - totalOutflows;
        decimal balance = totalInflows + totalDebts - totalOutflows;
        
        return (totalTransactionCount, totalInflows, totalOutflows, netTransactions, balance);
    }
   // debts logic
   public async Task<List<Transaction>> GetPendingDebtsAsync()
   {
       var transactions = await LoadTransactionsAsync();
       return transactions
           .Where(t => t.Type == "Debt" && (!t.IsCleared.HasValue || !t.IsCleared.Value))
           .OrderBy(t => t.DueDate)
           .ToList();
   }
   public async Task<List<Transaction>> GetClearedDebtsAsync()
   {
       var transactions = await LoadTransactionsAsync();
       return transactions
           .Where(t => t.Type == "Debt" && t.IsCleared.HasValue && t.IsCleared.Value)
           .OrderByDescending(t => t.DueDate)
           .ToList();
   }
   public async Task ClearDebtAsync(int transactionId)
   {
       try
       {
           var transactions = await LoadTransactionsAsync();
           var debt = transactions.FirstOrDefault(t => t.TransactionId == transactionId && t.Type == "Debt");
        
           if (debt == null)
               throw new InvalidOperationException("Debt transaction not found.");
           // Calculate current balance excluding the debt to be cleared
           var (totalInflows, totalOutflows, _, _, _, currentBalance, _) = CalculateTransactionSums(
               transactions.Where(t => t.TransactionId != transactionId).ToList()
           );
           // Check if there's enough balance to clear the debt
           if (currentBalance < debt.Amount)
               throw new InvalidOperationException($"Insufficient balance to clear debt. Required: {debt.Amount:C}, Available: {currentBalance:C}");
           // Update the debt transaction to mark it as cleared
           debt.IsCleared = true;
           await SaveTransactionAsync(debt);
           // Create and save a new transaction for debt payment
           var debtPayment = new Transaction
           {
               TransactionId = transactions.Max(t => t.TransactionId) + 1,
               Title = $"Debt Payment - {debt.Title}",
               Amount = debt.Amount,
               Type = "Debit", // This creates an outflow
               Date = DateTime.Now,
               Note = $"Payment for debt: {debt.Title} (ID: {debt.TransactionId})",
               Tags = "Debt Payment",
               SourceOfDebt = debt.SourceOfDebt, // Link to original debt
               DueDate = null,
               IsCleared = true
           };
           await SaveTransactionAsync(debtPayment);
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Error clearing debt: {ex.Message}");
           throw;
       }
   }
   public async Task<decimal> GetTotalPendingDebtAmount()
   {
       var pendingDebts = await GetPendingDebtsAsync();
       return pendingDebts.Sum(d => d.Amount);
   }
    
   public async Task<List<Transaction>> FilterTransactionsByDateAsync(DateTime? startDate, DateTime? endDate)
   {
       var transactions = await LoadTransactionsAsync();
       return transactions.Where(t =>
           (!startDate.HasValue || t.Date >= startDate.Value) &&
           (!endDate.HasValue || t.Date <= endDate.Value) 
       ).ToList();
   }
   
    public async Task<string> ExportToCsvAsync(List<Transaction> transactions)
   {
       try
       {
           if (transactions == null || !transactions.Any())
           {
               throw new InvalidOperationException("No transactions available to export.");
           }
           var csvContent = new StringBuilder();
           // Add header
           csvContent.AppendLine("TransactionId,Title,Amount,Type,Date,Tags,Note,SourceOfDebt,DueDate,IsCleared");
           // Add data rows
           foreach (var transaction in transactions)
           {
               string escapedTags = transaction.Tags?.Replace(",", "||") ?? "";
               string escapedNote = transaction.Note?.Replace(",", "||") ?? "";
               string escapedSourceOfDebt = transaction.SourceOfDebt?.Replace(",", "||") ?? "";
               string dueDate = transaction.DueDate.HasValue ? transaction.DueDate.Value.ToString("yyyy-MM-dd") : "";
               string isCleared = transaction.IsCleared.HasValue ? transaction.IsCleared.Value.ToString() : "";
               csvContent.AppendLine(
                   $"{transaction.TransactionId}," +
                   $"\"{transaction.Title}\"," +
                   $"{transaction.Amount}," +
                   $"{transaction.Type}," +
                   $"{transaction.Date:yyyy-MM-dd}," +
                   $"\"{escapedTags}\"," +
                   $"\"{escapedNote}\"," +
                   $"\"{escapedSourceOfDebt}\"," +
                   $"{dueDate}," +
                   $"{isCleared}"
               );
           }
           // Save to a file (optional)
           var filePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
               $"transactions_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
           );
           await File.WriteAllTextAsync(filePath, csvContent.ToString());
           return filePath; // Return the file path for reference
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Error exporting transactions to CSV: {ex.Message}");
           throw;
       }
   }
}