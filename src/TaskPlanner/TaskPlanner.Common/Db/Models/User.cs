namespace TaskPlanner.Common.Db.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TelegramUsername { get; set; }
        public int TelegramId { get; set; }
    }
}
