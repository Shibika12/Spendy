namespace ShibikaKhatri22068731CW1.Model;

public class Debt
{
    public int DebtId { get; set; }
    public string SourceOfDebt { get; set; }
    public float Amount { get; set; }
    public float InitialAmount { get; set; } // Original debt amount
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } // e.g. "Pending", "Cleared"
    public string ClearedFrom { get; set; } // e.g. "Credit Transaction ID
}