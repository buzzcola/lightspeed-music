using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// All the information needed to construct an interval flashcard generator.
    /// </summary>
    public class IntervalFlashcardGeneratorArgs : StaffFlashcardGeneratorArgs
    {
        /// <summary>
        /// Defines which types of intervals will be output by the generator.
        /// </summary>
        public Interval IntervalTypes { get; set; }

        /// <summary>
        /// Make me one with everything!
        /// </summary>
        public new static IntervalFlashcardGeneratorArgs All
        {
            get
            {
                return new IntervalFlashcardGeneratorArgs()
                {
                    Accidentals = AccidentalType.All,
                    Staffs = Staff.All,
                    IntervalTypes = Interval.All
                };
            }
        }
    }
}
