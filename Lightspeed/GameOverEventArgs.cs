using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    public class GameOverEventArgs:EventArgs
    {
        public readonly GameResult Result;

        public GameOverEventArgs(GameResult result)
        {
            Result = result;
        }
    }
}
