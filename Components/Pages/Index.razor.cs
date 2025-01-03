using Microsoft.AspNetCore.Components;

namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Index : ComponentBase
{
    protected override void OnInitialized()
    {

        Nav.NavigateTo("/login");
    }
}