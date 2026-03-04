namespace Quartets.Models
{
    public class NumberOfPlayers
    {
        #region Properties

        public int NumPlayers { get; set; }
        public string DisplayName => $"{NumPlayers}";

        #endregion

        #region Constructors

        public NumberOfPlayers(int numPlayers)
        {
            NumPlayers = numPlayers;
        }
        public NumberOfPlayers()
        {
            NumPlayers = 2;
        }

        #endregion
    }
}
