namespace BoardgamesApi.Models
{
    public class Boardgame
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Players { get; set; }
    }

    public class NewBoardgame
    {
        public string? Name { get; set; }
        public int? Players { get; set; }
    }
}