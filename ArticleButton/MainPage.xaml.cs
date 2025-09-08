using ArticleButton.Models;

namespace ArticleButton
{
    public partial class MainPage : ContentPage
    {
        private readonly List<Button> _buttons = [];
        private Button? _selectedButton;

        public MainPage()
        {
            InitializeComponent();
            var categories = new List<string> { "All", "Bread", "Sandwich", "Croissant" };
            var actions = new List<string> { "Filter", "Filter", "Filter", "Filter" };

            var buttons = new List<ArticleButtonModel>();

            for (int i = 0; i < categories.Count; i++)
            {
                buttons.Add(new ArticleButtonModel
                {
                    Text = $"■  {categories[i]}",
                    Action = actions[i]
                });
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(210) });

                var info = buttons[i];
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

                btn.Clicked += OnClicked;
                btn.Clicked += (s, e) => FilterByCategory(info.Text);
                var pointer = new PointerGestureRecognizer();
                pointer.PointerEntered += OnPointerEntered;
                pointer.PointerExited += OnPointerExited;
                btn.GestureRecognizers.Add(pointer);

                _buttons.Add(btn);

                ButtonGrid.Children.Add(btn);
                Grid.SetColumn(btn, i);
            }

            if (_buttons.Count > 0)
            {
                _selectedButton = _buttons[0];
                SetActiveStyle(_selectedButton);
                _selectedButton.TranslationY = -3;
            }
        }

        private async void OnClicked(object? sender, EventArgs e)
        {
            if (sender is not Button button) return;
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
            {
                ResetButtonStyle(_selectedButton);
            }

            _selectedButton = button;
            SetActiveStyle(_selectedButton);
            await _selectedButton.TranslateTo(0, -3, 100);
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
        private static void FilterByCategory(string category)
        {
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
    }
}

