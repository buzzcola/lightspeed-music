using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// The names of notes in the scale.
    /// </summary>
    public enum NoteName
    {
        A = 0, 
        B = 1, 
        C = 2, 
        D = 3, 
        E = 4, 
        F = 5, 
        G = 6
    }

    public static class NoteNameExtentions
    {
        /// <summary>
        /// Add the requested number of note names to a note.  A plus 2 = C.  G plus 1 = A.
        /// </summary>
        public static NoteName AddNoteNames(this NoteName name, int noteNames)
        {
            return (NoteName)(((int)name + noteNames) % 7);
        }
    }
}
