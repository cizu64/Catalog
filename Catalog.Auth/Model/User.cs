namespace Catalog.Auth.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; } = "USER";
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }

    }
}
