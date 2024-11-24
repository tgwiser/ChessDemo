namespace ChessCommon.Persistense
{
    public class Game
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Moves { get; set; }

        public int Level { get; set; }

        public DateTime date { get; set; } = DateTime.Now;
    }
}