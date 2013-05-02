using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// The kinds of accidental available on a note.  Currently not supported - double flats, double sharps, and anything crazier than that.
    /// </summary>
    [Flags]
    public enum AccidentalType
    {
        Natural = 0x1,
        Sharp = 0x2,
        Flat = 0x4,
        All = 0x7FFFFFFF
    }
}
