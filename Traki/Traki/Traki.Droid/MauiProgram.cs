using AndroidX.ViewPager.Widget;
using Core.Pages;
using Core.ViewModels;
using Core.Views;
using Microsoft.Extensions.DependencyInjection;
using Traki.Pages;
using Traki.Service;
using Traki.ViewModels;
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

            RegisterService<IAccountService, AccountService>(builder, dbPath);
            RegisterService<ITransactionService, TransactionService>(builder, dbPath);

            builder.UseSharedMauiApp();

            // Register the page and ViewModel with the dbPath
            RegisterPageWithViewModel<SharedHeaderViewModel, SharedHeaderView>(builder, dbPath);
            RegisterPageWithViewModel<ManageAccountsViewModel, ManageAccountsPage>(builder);

            //RegisterPageWithViewModel<Traki.ViewModels.DashboardViewModel, DashboardPage>(builder, dbPath);
            builder.Services.AddSingleton<DashboardViewModel>(provider =>
            {
                var service = provider.GetRequiredService<IAccountService>();
                return new DashboardViewModel(dbPath, service, provider);
            });
            builder.Services.AddSingleton<IncomeViewModel>(provider =>
            {
                var service = provider.GetRequiredService<ITransactionService>();
                return new IncomeViewModel(dbPath, service);
            });

            builder.Services.AddSingleton<DashboardPage>();

            return builder.Build();
        }

        private static void RegisterService<TInterface, TImplementation>(MauiAppBuilder builder, string? dbPath = null)
    where TInterface : class
    where TImplementation : class, TInterface
        {
            builder.Services.AddSingleton<TInterface>(provider =>
            {
                return (TInterface)(Activator.CreateInstance(typeof(TImplementation), dbPath!)
                    ?? throw new InvalidOperationException($"Could not create instance of {typeof(TImplementation)}"));
            });
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
                    var transactionService = provider.GetRequiredService<ITransactionService>();
                    // Create the ViewModel instance with dbPath and accountService
                    //return (TViewModel)Activator.CreateInstance(typeof(TViewModel), dbPath, accountService);
                    var instance = Activator.CreateInstance(typeof(TViewModel), dbPath, accountService, transactionService)
    ?? throw new InvalidOperationException($"Could not create an instance of {typeof(TViewModel)}.");
                    return (TViewModel)instance;

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
