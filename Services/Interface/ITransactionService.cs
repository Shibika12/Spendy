using ShibikaKhatri22068731CW1.Model;
namespace ShibikaKhatri22068731CW1.Services.Interface;


public interface ITransactionService
{
    Task SaveTransactionAsync(Transaction transaction);
    Task<List<Transaction>> LoadTransactionsAsync();
    Task DeleteTransactionAsync(int transactionId);  // Add this method
}