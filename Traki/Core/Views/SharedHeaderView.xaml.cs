using Core.ViewModels;
using TrakiLibrary.Interfaces;

namespace Core.Views;

public partial class SharedHeaderView : ContentView
{
    private SharedHeaderViewModel? _viewModel;

    public SharedHeaderView()
    {
        InitializeComponent();

        // Delay service resolution until Handler is available
        this.Loaded += SharedHeaderView_Loaded;
    }

    private void SharedHeaderView_Loaded(object? sender, EventArgs e)
    {
        var serviceProvider = this.Handler?.MauiContext?.Services;

        if (serviceProvider != null)
            {
                var accountService = serviceProvider.GetService<IAccountService>();
            var dbPath = Preferences.Get("DbPath", string.Empty);

            _viewModel = new SharedHeaderViewModel(dbPath, accountService!);
            this.BindingContext = _viewModel;
        }
    }

    public SharedHeaderView(SharedHeaderViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}

