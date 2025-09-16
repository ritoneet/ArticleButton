using ArticleButton.Models;
using Microsoft.Maui.Controls.Shapes;
namespace ArticleButton.Controls;

public partial class ArticleButtonControl : ContentView
{
    private readonly Grid _buttonGrid;
    private readonly ScrollView? _scroller;
    private readonly List<Button> _buttons = [];
    private Button? _selectedButton;
    private const double ScrollStep = 20;
    static double scale = 0.2;
    double w = 75 * scale;
    double h = 116 * scale;
#if WINDOWS
    bool leftArrowEnabled = true;
    bool rightArrowEnabled = true;
#endif
    public static Application App => Application.Current;
    public event Action<string>? CategorySelected;

    public ArticleButtonControl()
    {
        var pointer = new PointerGestureRecognizer();
        _buttonGrid = new Grid { HeightRequest = 54 };
        var leftArrowContainer = CreateArrow(isLeft: true);
        var rightArrowContainer = CreateArrow(isLeft: false);

        _scroller = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = _buttonGrid,
        };

        var layout = new Grid
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            RowDefinitions = { new RowDefinition { Height = 54 } },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        layout.Children.Add(leftArrowContainer);
        Grid.SetRow(leftArrowContainer, 0);
        Grid.SetColumn(leftArrowContainer, 0);

        layout.Children.Add(_scroller);
        Grid.SetRow(_scroller, 0);
        Grid.SetColumn(_scroller, 1);

        layout.Children.Add(rightArrowContainer);
        Grid.SetRow(rightArrowContainer, 0);
        Grid.SetColumn(rightArrowContainer, 2);

        Content = layout;
    }


    private Grid CreateArrow(bool isLeft)
    {
        var container = new Grid
        {
            WidthRequest = 54,
            HeightRequest = 54,
            BackgroundColor = Colors.Transparent,
        };

        var polygon = new Polygon
        {
            Points = isLeft
                ? [new Point(w, 0), new Point(w, h), new Point(0, h / 2)]
                : [new Point(0, 0), new Point(0, h), new Point(w, h / 2)],
            Fill = new SolidColorBrush((Color)App.Resources["BlueArrow"])
        };

        container.Children.Add(polygon);
        
        polygon.TranslationY = isLeft ? 15 : 15;
        polygon.TranslationX = isLeft ? 20 : 20;

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) =>
        {
            if (isLeft)
                OnLeftArrowClicked();
            else
                OnRightArrowClicked();
        };
        container.GestureRecognizers.Add(tap);

#if WINDOWS
        // Hover только на Windows
        var pointer = new PointerGestureRecognizer();
        pointer.PointerEntered += (s, e) => polygon.Fill = new SolidColorBrush((Color)App.Resources["HowerBlueArrow"]);
        pointer.PointerExited += (s, e) => polygon.Fill = new SolidColorBrush((Color)App.Resources["BlueArrow"]);
        container.GestureRecognizers.Add(pointer);
#endif
        return container;
    }
    private async void OnLeftArrowClicked()
    {
        if (_scroller == null) return;
        double x = Math.Max(0, _scroller.ScrollX - ScrollStep);
        await _scroller.ScrollToAsync(x, 0, true);
    }

    private async void OnRightArrowClicked()
    {
        if (_scroller == null) return;
        double contentWidth = _buttonGrid.Width;
        double viewportWidth = _scroller.Width;
        double maxX = Math.Max(0, contentWidth - viewportWidth);
        double x = Math.Min(maxX, _scroller.ScrollX + ScrollStep);
        await _scroller.ScrollToAsync(x, 0, true);
    }

    public void SetCategories(List<ArticleButtonModel> categories)
    {
        _buttons.Clear();
        _buttonGrid.Children.Clear();
        _buttonGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < categories.Count; i++)
        {
            _buttonGrid.ColumnDefinitions.Add(new ColumnDefinition(210));

            var info = categories[i];
            var btn = new Button
            {
                Text = info.Text,
                WidthRequest = 200,
                HeightRequest = 40,
                ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Left, 10),
                CornerRadius = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "OpenSansRegular",
                TextColor = Colors.Black,
                Shadow = new Shadow
                {
                    Offset = new Point(2, 3),
                    Radius = 2,
                    Opacity = 1,
                    Brush = new SolidColorBrush(Colors.Gray)
                },
                BorderWidth = 2,
                BorderColor = (Color)ArticleButtonControl.App.Resources["BorderColor"],
                BackgroundColor = (Color)ArticleButtonControl.App.Resources["White"],
                ImageSource = info.Image
            };

            btn.Clicked += (s, e) => OnButtonClicked(btn, info.Text);

            var pointer = new PointerGestureRecognizer();
#if WINDOWS
            pointer.PointerEntered += OnPointerEntered;
            pointer.PointerExited += OnPointerExited;
#endif
            btn.GestureRecognizers.Add(pointer);

            _buttons.Add(btn);
            _buttonGrid.Add(btn, i, 0);
        }

        if (_buttons.Count > 0)
        {
            _selectedButton = _buttons[0];
            SetActiveStyle(_selectedButton);
            _selectedButton.TranslationY = -3;
        }
    }

    private async void OnButtonClicked(Button button, string category)
    {
        if (_selectedButton == button)
        {
            if (_selectedButton == _buttons[0])
                return;

            ResetButtonStyle(_selectedButton);

            _selectedButton = _buttons[0];
            SetActiveStyle(_selectedButton);
            await _selectedButton.TranslateTo(0, -3, 100);
            return;
        }

        if (_selectedButton != null)
            ResetButtonStyle(_selectedButton);

        _selectedButton = button;
        SetActiveStyle(_selectedButton);
        await _selectedButton.TranslateTo(0, -3, 100);
        CategorySelected?.Invoke(category);
    }
#if WINDOWS
    private void OnArrowEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            btn.BackgroundColor = (Color)App.Resources["HowerBlueArrow"];
        }
    }
    private void OnArrowExited(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            btn.BackgroundColor = (Color)App.Resources["BlueArrow"];
        }
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn == _selectedButton)
                btn.BackgroundColor = (Color)App.Resources["HowerActive"];
            else
                btn.BackgroundColor = (Color)App.Resources["Default"];
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn == _selectedButton)
                btn.BackgroundColor = (Color)App.Resources["ActiveButton"];
            else
                btn.BackgroundColor = (Color)App.Resources["White"];
        }
    }
#endif
    private static void SetActiveStyle(Button button)
    {
        button.AbortAnimation("TranslationY");
        button.BackgroundColor = (Color)App.Resources["ActiveButton"];
        button.TextColor = Colors.White;
        button.BorderWidth = 0;
    }

    private static async void ResetButtonStyle(Button button)
    {
        button.AbortAnimation("TranslationY");
        await button.TranslateTo(0, 0, 100);
        button.BackgroundColor = (Color)App.Resources["White"];
        button.TextColor = Colors.Black;
        button.BorderWidth = 2;
    }
}