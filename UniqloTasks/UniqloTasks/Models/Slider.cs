namespace UniqloTasks.Models
{
    public class Slider :BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Subtitle { get; set; } = null!;
        public string? Link { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
