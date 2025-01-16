using ShibikaKhatri22068731CW1.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
namespace ShibikaKhatri22068731CW1.Services.Interface;

public interface ITransactionService
{
    Task SaveTransactionAsync(Transaction transaction);
    Task<List<Transaction>> LoadTransactionsAsync();
    Task DeleteTransactionAsync(int transactionId);  // Add this method
   
    (decimal totalInflows, decimal totalOutflows, decimal totalDebt, decimal totalClearedDebt, decimal remainingDebt,
        decimal balance, bool isSufficientBalance)
        CalculateTransactionSums(List<Transaction> transactions);

    (Transaction highestInflow, Transaction lowestInflow, 
        Transaction highestOutflow, Transaction lowestOutflow, 
        Transaction highestDebt, Transaction lowestDebt) 
        GetTransactionExtremes(List<Transaction> transactions);

    // New methods
    Task<List<Transaction>> SearchTransactionsByTitleAsync(string title);
    Task<List<Transaction>> FilterTransactionsAsync(DateTime? startDate, DateTime? endDate, string type, string tags);
    Task<List<Transaction>> SortTransactionsAsync(string sortBy, bool isAscending);
    Task<List<Transaction>> SortTransactionsAscending(string sortBy);
    Task<List<Transaction>> SortTransactionsDescending(string sortBy);
    (List<Transaction> topInflows, List<Transaction> lowestInflows, 
        List<Transaction> topOutflows, List<Transaction> lowestOutflows) 
        GetTransactionExtremes(List<Transaction> transactions, int count = 5);

    Task<List<Transaction>> GetTransactionsAsync();
    
    (int TotalTransactionCount, decimal NetTransactions, decimal TotalInflows, decimal TotalOutflows, decimal Balance)
        GetTransactionSummary(List<Transaction> transactions);
    
    Task<List<Transaction>> GetPendingDebtsAsync();
    Task<List<Transaction>> GetClearedDebtsAsync();
    Task ClearDebtAsync(int transactionId);
    Task<decimal> GetTotalPendingDebtAmount();
    Task<List<Transaction>> FilterTransactionsByDateAsync(DateTime? startDate, DateTime? endDate);
    
    Task<string> ExportToCsvAsync(List<Transaction> transactions);
}