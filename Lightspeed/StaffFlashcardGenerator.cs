using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// A class that can generate flashcards particular to one staff.
    /// </summary>
    public abstract class StaffFlashcardGenerator:FlashcardGenerator
    {        
        /// <summary>
        /// This flags variable determines the staffs that will be generated.
        /// </summary>
        public readonly Staff Staffs;

        /// <summary>
        /// This flags variable determines the accidentals that can be included in generated flashcards.
        /// </summary>
        public readonly AccidentalType Accidentals;

        /// <summary>
        /// Creates a new StaffFlashcardGenerator.
        /// </summary>
        /// <param name="staffs">This flags value indicates which staffs to include.</param>
        public StaffFlashcardGenerator(Staff staffs, AccidentalType accidentals)
        {
            Staffs = staffs;
            Accidentals = accidentals;
        }

        /// <summary>
        /// Creates all of the flashcards that this generator can come up with.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Flashcard> GenerateFlashcards()
        {
            if (Staffs.HasFlag(Staff.LeftHand))
                foreach (var f in GenerateFlashcards(Staff.LeftHand, Flashcard.LOWER_BOUND_LH, Flashcard.UPPER_BOUND_LH))
                    yield return f;
            if (Staffs.HasFlag(Staff.RightHand))
                foreach (var f in GenerateFlashcards(Staff.RightHand, Flashcard.LOWER_BOUND_RH, Flashcard.UPPER_BOUND_RH))
                    yield return f;
        }

        /// <summary>
        /// Generates flashcards in one staff.
        /// </summary>
        /// <param name="staff">The staff in which to generate flashcards.</param>
        /// <param name="lowerNoteNumber">No flashcards should have notes with a number lower than this.</param>
        /// <param name="upperNoteNumber">No flashcards should have notes with a number higher than this.</param>
        protected abstract IEnumerable<Flashcard> GenerateFlashcards(Staff staff, int lowerNoteNumber, int upperNoteNumber);
    }
}
