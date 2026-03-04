namespace Quartets.ViewModels
{
    public class CompletedSetPopUpVM
    {
        #region Properties

        public string Title { get; set; }
        public string Message { get; set; }

        #endregion

        #region Constructor

        public CompletedSetPopUpVM(string playerName, bool isCurrentPlayer)
        {
            if (isCurrentPlayer)
            {
                Title = "השלמת רביעייה!";
                Message = $"כל הכבוד {playerName}! השלמת רביעייה!";
            }
            else
            {
                Title = "יריבך השלים רביעייה";
                Message = $"{playerName} השלים רביעייה!";
            }
        }

        #endregion
    }
}
