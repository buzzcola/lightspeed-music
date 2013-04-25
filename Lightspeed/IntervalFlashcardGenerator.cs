using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Makes interval flashcards (two notes.)
    /// </summary>
    public class IntervalFlashcardGenerator:StaffFlashcardGenerator
    {
        /// <summary>
        /// This flags value indicates which types of intervals will be generated.  Null is treated as All.
        /// </summary>
        public readonly Interval? IntervalTypes;

        /// <summary>
        /// Creates a new Interval Flashcard Generator.
        /// </summary>
        /// <param name="intervalTypes">This flags value idnicates which types of intervals will be generated.</param>
        /// <param name="staffs">This flags value indicates which staffs to include.</param>
        public IntervalFlashcardGenerator(Interval? intervalTypes = null, Staff? staffs = null):base(staffs)
        {
            IntervalTypes = intervalTypes;
        }

        protected override IEnumerable<Flashcard> GenerateFlashcards(Staff staff, int lower, int upper)
        {
            foreach (Interval interval in Enum.GetValues(typeof(Interval)))
            {
                if (!IntervalTypes.HasValue || IntervalTypes.Value.HasFlag(interval))
                {
                    foreach (var bottomNote in Enumerable.Range(lower, upper - lower + 1 - interval.NumberOfSemitones()))
                    {
                        foreach (var bottomRep in new Note(bottomNote).GetRepresentations())
                        {
                            var reps = bottomRep.MakeGroup(interval);
                            if (reps != null)
                            {
                                var staffNotes = reps.Select(r => new StaffNote(r, staff)).ToArray();
                                yield return new Flashcard(staffNotes);
                            }
                        }
                    }
                }
            }
        }
    }
}
