using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    public class FlashcardEventArgs:EventArgs
    {
        public readonly Flashcard Flashcard;

        public FlashcardEventArgs(Flashcard flashcard)
        {
            Flashcard = flashcard;
        }
    }
}
