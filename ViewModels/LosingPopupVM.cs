namespace Quartets.ViewModels
{
    public class LosingPopupVM
    {
        public string WinnerName { get; set; }
        public LosingPopupVM(string name)
        {
            WinnerName = name;
        }
    }
}
