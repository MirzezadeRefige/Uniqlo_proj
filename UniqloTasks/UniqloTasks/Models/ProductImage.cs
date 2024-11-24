namespace UniqloTasks.Models
{
    public class ProductImage
    {
        public  int ProductId { get; set; }
        public Product Product { get; set; }
        public string ImageUrl { get; set; }
    }
}
