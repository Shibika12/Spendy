using ShibikaKhatri22068731CW1.Model;
namespace ShibikaKhatri22068731CW1.Services.Interface;

public interface IUserService
{
    bool Login(User user);
    string PreferredCurrency { get; }
    bool IsLoggedIn { get; }
    string LoggedInUser { get; }
}