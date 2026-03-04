namespace Quartets.ViewModels
{
    public class LosingPopupVM
    {
        #region Properties

        public string WinnerName { get; set; }

        #endregion

        #region Constructor

        public LosingPopupVM(string name)
        {
            WinnerName = name;
        }

        #endregion
    }
}
