namespace ArticleButton.Models
{
    class ArticleButtonModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
