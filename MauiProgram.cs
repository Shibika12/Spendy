using ShibikaKhatri22068731CW1.Model;
using ShibikaKhatri22068731CW1.Services;
using ShibikaKhatri22068731CW1.Services.Interface;
using MudBlazor.Services;

namespace ShibikaKhatri22068731CW1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddMudServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            return builder.Build();
        }
    }
}