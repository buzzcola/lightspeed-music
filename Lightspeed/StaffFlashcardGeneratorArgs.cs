using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// All the information required to construct a FlashcardGenerator.
    /// </summary>
    public class StaffFlashcardGeneratorArgs
    {
        /// <summary>
        /// Determines which staffs will be included on the generated flashcards.
        /// </summary>
        public Staff Staffs { get; set; }

        /// <summary>
        /// Determines which accidentals will be included on the generated flashcards.
        /// </summary>
        public AccidentalType Accidentals { get; set; }

        /// <summary>
        /// Make me one with everything!
        /// </summary>
        public static StaffFlashcardGeneratorArgs All
        {
            get
            {
                return new StaffFlashcardGeneratorArgs()
                    {
                        Accidentals = AccidentalType.All,
                        Staffs = Staff.All
                    };
            }
        }
    }
}
