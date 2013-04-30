using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    public class NewGameCountdownEventArgs : EventArgs
    {
        public readonly int SecondsToGo;

        public NewGameCountdownEventArgs(int secondsToGo)
        {
            SecondsToGo = secondsToGo;
        }
    }
}
