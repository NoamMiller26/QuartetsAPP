namespace Quartets.ViewModels
{
    public class CompletedSetPopUpVM
    {
        public string Title { get; set; }
        public string Message { get; set; }

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
    }
}
