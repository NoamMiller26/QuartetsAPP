namespace Quartets.Models
{
    public class NumberOfPlayers
    {
        public int NumPlayers { get; set; }
        public string DisplayName => $"{NumPlayers}";
        public NumberOfPlayers(int numPlayers)
        {
            NumPlayers = numPlayers;
        }
        public NumberOfPlayers()//Change
        {
            NumPlayers = 5;
        }
    }
}
