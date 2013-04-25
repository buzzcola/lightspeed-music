using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// The staffs supported in Lightspeed.
    /// </summary>
    [Flags]
    public enum Staff
    {
        RightHand = 0x1,
        LeftHand = 0x2
    }
}
