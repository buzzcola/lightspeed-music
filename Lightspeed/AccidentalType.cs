using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// The kinds of accidental available on a note.  Currently not supported - double flats, double sharps, and anything crazier than that.
    /// </summary>
    public enum AccidentalType
    {
        Flat = -1,
        Natural = 0,
        Sharp = 1        
    }
}
