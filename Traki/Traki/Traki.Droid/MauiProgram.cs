using Core.ViewModels;
using Core.Views;
using Traki.Service;
using TrakiLibrary.Interfaces;

namespace Traki.Droid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Determine the database path based on the platform
            string? dbPath = GetDatabasePath();

            builder.Services.AddSingleton<IAccountService>(provider =>
            {
                return new AccountService(dbPath!);
            });
            builder.UseSharedMauiApp();

            // Register the page and ViewModel with the dbPath
            RegisterPageWithViewModel<SharedHeaderViewModel, SharedHeaderView>(builder, dbPath);

            return builder.Build();
        }

        // Common function to register pages and ViewModels with a factory for ViewModel creation
        private static void RegisterPageWithViewModel<TViewModel, TPage>(MauiAppBuilder builder, string? dbPath = null)
            where TViewModel : class
            where TPage : class
        {
            // If dbPath is provided, use a factory to pass both the dbPath and ITransactionService to the ViewModel constructor
            if (dbPath != null)
            {
                builder.Services.AddSingleton<TViewModel>(provider =>
                {
                    // Get required services
                    var accountService = provider.GetRequiredService<IAccountService>();
                    // Create the ViewModel instance with dbPath and accountService
                    return (TViewModel)Activator.CreateInstance(typeof(TViewModel), dbPath, accountService);
                });
            }
            else
            {
                builder.Services.AddSingleton<TViewModel>();
            }

            builder.Services.AddSingleton<TPage>();
        }

        private static string GetDatabasePath()
        {
            // Use platform-specific code to determine the database path
#if ANDROID
            return Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? FileSystem.AppDataDirectory, "expenses.db");
#elif WINDOWS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses.db");
#else
            return Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
#endif
        }
    }
}
