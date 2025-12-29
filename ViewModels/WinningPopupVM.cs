namespace Quartets.ViewModels
{
    public class WinningPopupVM
    {
        public string WinnerName { get; set; }
        public WinningPopupVM(string name)
        {
            WinnerName = name;
        }
    }
}
