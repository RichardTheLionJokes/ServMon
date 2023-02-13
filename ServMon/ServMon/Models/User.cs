namespace ServMon.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PrintName { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<Server> Servers { get; set; } = new();
    }
}
