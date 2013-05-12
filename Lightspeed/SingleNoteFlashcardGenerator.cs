using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// The source for the simplest flashcards, those that only contain a single note.
    /// </summary>
    public class SingleNoteFlashcardGenerator:StaffFlashcardGenerator
    {
        /// <summary>
        /// Creates a new SingleNoteFlashcardGenerator.
        /// </summary>
        /// <param name="staffs">This flags value indicates which staffs to include.</param>
        public SingleNoteFlashcardGenerator(StaffFlashcardGeneratorArgs args) : base(args) { }

        /// <summary>
        /// Make flashcards for all supported single notes.
        /// </summary>
        protected override IEnumerable<Flashcard> GenerateFlashcards(Staff staff, int lowerNoteNumber, int upperNoteNumber)
        {
            for (int i = lowerNoteNumber; i <= upperNoteNumber; i++)
                foreach (var rep in new Note(i).GetRepresentations().Where(r => Accidentals.HasFlag(r.Accidental)))
                    yield return new Flashcard(new StaffNote(rep, staff));
        }
    }
}
