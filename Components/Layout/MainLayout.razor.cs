using Microsoft.AspNetCore.Components;
namespace ShibikaKhatri22068731CW1.Components.Layout;

public partial class MainLayout
{
    private bool IsLogout { get; set; } = false;
    private void Logout()
    {
        // Clear login state if necessary (not required in this simple setup)
        IsLogout = false; // Ensure the modal does not display after navigating.
        Nav.NavigateTo("/login");
    }
    
    private void ShowLogoutConfirmation()
    {
        IsLogout = true;
    }

    private void HideLogoutConfirmation()
    {
        IsLogout = false;
    }
    
}