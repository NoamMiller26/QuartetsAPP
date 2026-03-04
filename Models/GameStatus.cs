using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartets.Models
{
    public class GameStatus
    {
        #region Fields

        private readonly string[] msgs = [Strings.WaitMessage, Strings.PlayMessage];

        #endregion

        #region Types

        public enum Status { Wait, Play }

        #endregion

        #region Properties

        public Status CurrentStatus { get; set; } = Status.Wait;
        public string StatusMessage => msgs[(int)CurrentStatus];

        #endregion
    }
}
