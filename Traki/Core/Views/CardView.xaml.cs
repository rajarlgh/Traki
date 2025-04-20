namespace Core.Views;

public partial class CardView : ContentView
{
	public CardView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
       nameof(Title), typeof(string), typeof(CardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}