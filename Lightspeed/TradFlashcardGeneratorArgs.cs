using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// All the information to construct a triad flashcard generator.
    /// </summary>
    public class TradFlashcardGeneratorArgs : StaffFlashcardGeneratorArgs
    {
        /// <summary>
        /// Determines the types of triads that will be output by the generator.
        /// </summary>
        public TriadType TriadTypes { get; set; }

        /// <summary>
        /// Determines the triad inversions that will be output by the generator.
        /// </summary>
        public TriadInversion TriadInversions { get; set; }

        /// <summary>
        /// Make me one with everything!
        /// </summary>
        public new static TradFlashcardGeneratorArgs All
        {
            get
            {
                return new TradFlashcardGeneratorArgs()
                {
                    Accidentals = AccidentalType.All,
                    Staffs = Staff.All,
                    TriadTypes = TriadType.All,
                    TriadInversions = TriadInversion.All
                };
            }
        }
    }
}
