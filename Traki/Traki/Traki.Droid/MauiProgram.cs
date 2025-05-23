using Core.Pages;
using Core.ViewModels;
using Core.Views;
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

            string dbPath = GetDatabasePath();

            builder.Services.AddSingleton<IBudgetContextService>(provider =>
            {
                var service = new BudgetContextService();
                service.SetBudget(dbPath);
                return service;
            });

            // Register SKCanvasView Handler
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SkiaSharp.Views.Maui.Controls.SKCanvasView, SkiaSharp.Views.Maui.Handlers.SKCanvasViewHandler>();
            });

            // Register services that depend on BudgetContextService
            RegisterService<IAccountService, AccountService>(builder);
            RegisterService<ICategoryService, CategoryService>(builder);
            RegisterService<ITransactionByCategoryService, TransactionByCategoryService>(builder);
            RegisterService<ITransactionByAccountService, TransactionByAccountService>(builder);

            builder.UseSharedMauiApp();

            // Register pages with viewmodels
            RegisterPageWithViewModel<SharedHeaderViewModel, SharedHeaderView>(builder);
            RegisterPageWithViewModel<ManageAccountsViewModel, ManageAccountsPage>(builder);
            RegisterPageWithViewModel<ExcelUploaderViewModel, ExcelUploaderPage>(builder);
            RegisterPageWithViewModel<TransactionViewModel, TransactionPage>(builder);
            RegisterPageWithViewModel<DetailedTransactionsViewModel, DetailedTransactionsView>(builder);
            RegisterPageWithViewModel<IncomeViewModel, IncomeView>(builder);
            RegisterPageWithViewModel<ExpenseViewModel, ExpenseView>(builder);
            RegisterPageWithViewModel<DashboardViewModel, DashboardPage>(builder);

            builder.Services.AddSingleton<DashboardPage>();
            builder.Services.AddSingleton<ExcelUploaderPage>();

            return builder.Build();
        }

        private static void RegisterService<TInterface, TImplementation>(MauiAppBuilder builder)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            builder.Services.AddSingleton<TInterface>(provider =>
            {
                var budgetContext = provider.GetRequiredService<IBudgetContextService>();
                return Activator.CreateInstance(typeof(TImplementation), budgetContext) as TInterface
                    ?? throw new InvalidOperationException($"Could not create instance of {typeof(TImplementation)}");
            });
        }

        private static void RegisterPageWithViewModel<TViewModel, TPage>(MauiAppBuilder builder)
            where TViewModel : class
            where TPage : class
        {
            builder.Services.AddSingleton<TViewModel>();
            builder.Services.AddSingleton<TPage>();
        }

        private static string GetDatabasePath()
        {
#if ANDROID
            //return Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? FileSystem.AppDataDirectory, "expenses.db");
            return Path.Combine(FileSystem.AppDataDirectory, "expenses_initial" + ".db");
#elif WINDOWS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "expenses_initial" + ".db");          
#else
            return Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
#endif
        }
    }
}
