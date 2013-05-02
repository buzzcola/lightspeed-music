using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Describes the different triad inversions that are available.
    /// </summary>
    [Flags]
    public enum TriadInversion
    {
        Root = 0x1,
        First = 0x2,
        Second = 0x4,
        All = 0x7FFFFFFF
    }
}
