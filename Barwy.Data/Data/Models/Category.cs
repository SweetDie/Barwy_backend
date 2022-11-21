namespace Barwy.Data.Data.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual IEnumerable<Product> Products { get; set; }
    }
}
