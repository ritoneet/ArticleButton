using ArticleButton.Controls;
using ArticleButton.Models;

namespace ArticleButton;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        var categoryControl = new ArticleButtonControl();

        var categories = new List<ArticleButtonModel>
        {
            new() { Text = "■  All", Action = "Filter" },
            new() { Text = "■  Bread", Action = "Filter" },
            new() { Text = "■  Sandwich", Action = "Filter" },
            new() { Text = "■  Croissant", Action = "Filter" },
        };

        categoryControl.SetCategories(categories);

        Content = categoryControl;
    }
}

