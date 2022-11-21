namespace Barwy.Data.Data.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public double Cost { get; set; }
        public string Image { get; set; }
        public virtual IEnumerable<Category> Categories { get; set; }
    }
}
