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

            string? dbPath = GetDatabasePath();

            // 🛠️ Register SKCanvasView Handler
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SkiaSharp.Views.Maui.Controls.SKCanvasView, SkiaSharp.Views.Maui.Handlers.SKCanvasViewHandler>();
            });

            RegisterService<IAccountService, AccountService>(builder, dbPath);
            RegisterService<ITransactionService, TransactionService>(builder, dbPath);
            RegisterService<ICategoryService, CategoryService>(builder, dbPath);
            RegisterService<ITransactionByCategoryService, TransactionByCategoryService>(builder, dbPath);
            RegisterService<ITransactionByAccountService, TransactionByAccountService>(builder, dbPath);


            builder.UseSharedMauiApp();

            RegisterPageWithViewModel<SharedHeaderViewModel, SharedHeaderView>(builder);
            RegisterPageWithViewModel<ManageAccountsViewModel, ManageAccountsPage>(builder);
            RegisterPageWithViewModel<ExcelUploaderViewModel, ExcelUploaderPage>(builder);
            RegisterPageWithViewModel<TransactionViewModel, TransactionPage>(builder);
            RegisterPageWithViewModel<DetailedTransactionsViewModel, DetailedTransactionsView>(builder);
            RegisterPageWithViewModel<IncomeViewModel, IncomeView>(builder);
            RegisterPageWithViewModel<ExpenseViewModel, ExpenseView>(builder);

            builder.Services.AddSingleton<DashboardViewModel>(provider =>
            {
                var service = provider.GetRequiredService<IAccountService>();
                return new DashboardViewModel(service, provider);
            });

            //builder.Services.AddSingleton<IncomeViewModel>(provider =>
            //{
            //    var service = provider.GetRequiredService<ITransactionService>();
            //    return new IncomeViewModel(dbPath, service);
            //});

            //builder.Services.AddSingleton<ExcelUploaderViewModel>(provider =>
            //{
            //    var transactionService = provider.GetRequiredService<ITransactionService>();
            //    var accountService = provider.GetRequiredService<IAccountService>();
            //    var categoryService = provider.GetRequiredService<ICategoryService>();
            //    return new ExcelUploaderViewModel(transactionService, accountService, categoryService);
            //});

            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<ExcelUploaderPage>();

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
