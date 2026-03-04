namespace Quartets.ViewModels
{
    public class WinningPopupVM
    {
        #region Properties

        public string WinnerName { get; set; }

        #endregion

        #region Constructor

        public WinningPopupVM(string name)
        {
            WinnerName = name;
        }

        #endregion
    }
}
