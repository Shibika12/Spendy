using Microsoft.AspNetCore.Components;
using ShibikaKhatri22068731CW1.Services.Interface;
using ShibikaKhatri22068731CW1.Model;
namespace ShibikaKhatri22068731CW1.Components.Pages;
public partial class DebtEntry : ComponentBase
{
    private Debt debt = new Debt();
    private async Task HandleValidSubmit()
    {
        // Call the DebtService to add the debt to the database
        debt.Status = "Pending";
        debt.ClearedFrom = "";
        await DebtService.AddDebtAsync(debt);
        // Reset the form after adding the debt
        debt = new Debt();
        // Optionally show a success message or redirect
        Nav.NavigateTo("/debts-tracker");
    }
}