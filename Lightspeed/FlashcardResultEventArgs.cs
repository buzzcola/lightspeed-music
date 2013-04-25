using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    public class FlashcardResultEventArgs : EventArgs
    {
        public readonly FlashcardResult Result;

        public FlashcardResultEventArgs(FlashcardResult result)
        {
            Result = result;
        }
    }
}
