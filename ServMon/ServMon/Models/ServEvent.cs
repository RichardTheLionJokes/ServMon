namespace ServMon.Models
{
    public class ServEvent
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public Server? Server { get; set; }
        public DateTime DateTime { get; set; }
        public ServEventType Type { get; set; }
        public ServerStatus ServerStatus { get; set; }
    }
}
