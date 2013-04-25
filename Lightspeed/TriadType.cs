using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Describes the different types of supported triads.
    /// </summary>
    [Flags]
    public enum TriadType
    {
        Major = 0x1, 
        Minor = 0x2,
        Augmented = 0x4,
        Diminished = 0x8
    }
}
