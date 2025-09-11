using ArticleButton.Models;
using Microsoft.Maui.Controls.Shapes;

namespace ArticleButton.Controls;

public partial class ArticleButtonControl : ContentView
{
    private readonly Grid _buttonGrid;
    private readonly ScrollView? _scroller;
    private readonly List<Button> _buttons = [];
    private Button? _selectedButton;
    private const double ScrollStep = 420;

    public ArticleButtonControl()
    {
        _buttonGrid = new Grid { Padding = new Thickness(0) };

        _scroller = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            Content = _buttonGrid
        };

        var leftArrow = new ContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 40,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            Content = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(25, 10),
                    new Point(25, 30),
                    new Point(10, 20)
                },
                BackgroundColor = Application.Current?.Resources["BlueArrow"] as Color ?? Colors.Blue,
            }
        };

        leftArrow.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                OnLeftArrowClicked(leftArrow, EventArgs.Empty);
            })
        });

        var rightArrow = new ContentView
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            WidthRequest = 40,
            HeightRequest = 40,
            BackgroundColor = Colors.Transparent,
            Content = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(15, 10),
                    new Point(15, 30),
                    new Point(30, 20)
                },
                BackgroundColor = Application.Current?.Resources["BlueArrow"] as Color ?? Colors.Blue,
            }
        };
        rightArrow.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() =>
            {
                OnRightArrowClicked(rightArrow, EventArgs.Empty);
            })
        });

        var layout = new Grid
        {   
            HorizontalOptions = LayoutOptions.Start,
            Padding = new Thickness(20),
            RowDefinitions = { new RowDefinition { Height = 54 } },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        layout.Children.Add(leftArrow);
        Grid.SetRow(leftArrow, 0);
        Grid.SetColumn(leftArrow, 0);

        layout.Children.Add(_scroller);
        Grid.SetRow(_scroller, 0);
        Grid.SetColumn(_scroller, 1);

        layout.Children.Add(rightArrow);
        Grid.SetRow(rightArrow, 0);
        Grid.SetColumn(rightArrow, 2);

        Content = layout;
    }
    private async void OnLeftArrowClicked(object? sender, EventArgs e)
    {
        if (_scroller == null) return;
        double x = Math.Max(0, _scroller.ScrollX - ScrollStep);
        await _scroller.ScrollToAsync(x, 0, true);
    }

    private async void OnRightArrowClicked(object? sender, EventArgs e)
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
                BorderColor = (Color)Application.Current.Resources["BorderColor"],
                BackgroundColor = (Color)Application.Current.Resources["White"],
                ImageSource = info.Image
            };

            btn.Clicked += (s, e) => OnButtonClicked(btn, info.Text);

            var pointer = new PointerGestureRecognizer();
            pointer.PointerEntered += OnPointerEntered;
            pointer.PointerExited += OnPointerExited;
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
        Console.WriteLine($"Filter: {category}");
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn == _selectedButton)
                btn.BackgroundColor = (Color)Application.Current.Resources["HowerActive"];
            else
                btn.BackgroundColor = (Color)Application.Current.Resources["Default"];
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            if (btn == _selectedButton)
                btn.BackgroundColor = (Color)Application.Current.Resources["ActiveButton"];
            else
                btn.BackgroundColor = (Color)Application.Current.Resources["White"];
        }
    }

    private static void SetActiveStyle(Button button)
    {
        button.AbortAnimation("TranslationY");
        button.BackgroundColor = (Color)Application.Current.Resources["ActiveButton"];
        button.TextColor = Colors.White;
        button.BorderWidth = 0;
    }

    private static async void ResetButtonStyle(Button button)
    {
        button.AbortAnimation("TranslationY");
        await button.TranslateTo(0, 0, 100);
        button.BackgroundColor = (Color)Application.Current.Resources["White"];
        button.TextColor = Colors.Black;
        button.BorderWidth = 2;
    }
}