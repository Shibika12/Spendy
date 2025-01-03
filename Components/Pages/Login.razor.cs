using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShibikaKhatri22068731CW1.Model;

namespace ShibikaKhatri22068731CW1.Components.Pages;

public partial class Login : ComponentBase
{
    private string ? ErrorMessage;

    [Required(ErrorMessage = "Username is required.")]
    private MudForm? form;
    public User Users { get; set; } = new();

    private async Task HandleLogin()
    {
        await form.Validate();
        if (form.IsValid)
        {
            if (UserService.Login(Users))
            {
                Nav.NavigateTo("/home");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
            }
        }
    }
}