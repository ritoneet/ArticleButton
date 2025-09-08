using ArticleButton.Models;
namespace ArticleButton.Services
{
    class ArticleButtonService
    {
        public static async Task<List<ArticleButtonModel>> GetButtonsAsync()
        {
            await Task.Delay(100);

            return
            [
                new ArticleButtonModel { Text = "All", Image = "logo.png", Action = "Filter" },
            ];
        }
    }
}
