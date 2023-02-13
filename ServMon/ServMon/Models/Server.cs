namespace ServMon.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IpAddress { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
        public bool Activity { get; set; }
        public ServerStatus CurrentStatus { get; set; }
        public List<ServEvent> ServEvents { get; set; } = new();
        public List<User> Users { get; set; } = new();
    }
}
