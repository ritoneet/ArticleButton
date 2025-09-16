using ArticleButton.Controls;
using ArticleButton.Models;

namespace ArticleButton;

public partial class MainPage : ContentPage
{
    private List<string> allData;

    public MainPage()
    {
        InitializeComponent();

        var categoryControl = new ArticleButtonControl();

        var categories = new List<ArticleButtonModel>
        {
            new() { Text = "■  All", Action = "All" },
            new() { Text = "■  Bread", Action = "Bread" },
            new() { Text = "■  Sandwich", Action = "Sandwich" },
            new() { Text = "■  Croissant", Action = "Croissant" },
        };

        categoryControl.SetCategories(categories);

        // подписываем фильтр напрямую
        categoryControl.CategorySelected += FilterData;

        Content = categoryControl;

        // все данные — заранее
        allData =
        [
            "White Bread",
            "Dark Bread",
            "Cheese Sandwich",
            "Ham Sandwich",
            "Butter Croissant"
        ];
    }

    private void FilterData(string category)
    {
        Console.WriteLine($"Filter: {category}");

        IEnumerable<string> filtered;

        if (category == "All" || category == "■  All")
            filtered = allData;
        else
            filtered = allData.Where(x => x.Contains(category, StringComparison.OrdinalIgnoreCase));

        foreach (var item in filtered)
            Console.WriteLine(item);
    }
}

