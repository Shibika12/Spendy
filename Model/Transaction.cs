namespace ShibikaKhatri22068731CW1.Model;

public class Transaction
{
    public int TransactionId { get; set; }  // Unique identifier for the transaction
    public string Title { get; set; }  // Title or description of the transaction
    public decimal Amount { get; set; }  // Amount of the transaction
    public string Type { get; set; }  // Type of transaction: Credit, Debit, Debt
    
    public DateTime? Date { get; set; }  // Date of the transaction
    
    public string SourceOfDebt { get; set; }
    
    public DateTime? DueDate { get; set; }
    public string Tags { get; set; } 
    public string Note { get; set; }  // Additional information or description about the transaction
    
    public bool? IsCleared { get; set; }
}