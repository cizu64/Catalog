namespace Catalog.API.Model
{
    public class Shop
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string StoreName { get; set; } = "MyShop";
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
