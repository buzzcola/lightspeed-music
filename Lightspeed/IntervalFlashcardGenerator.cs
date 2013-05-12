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
        /// This flags value indicates which types of intervals will be generated.
        /// </summary>
        public readonly Interval IntervalTypes;

        /// <summary>
        /// Creates a new Interval Flashcard Generator.
        /// </summary>
        /// <param name="intervalTypes">This flags value idnicates which types of intervals will be generated.</param>
        /// <param name="staffs">This flags value indicates which staffs to include.</param>
        public IntervalFlashcardGenerator(IntervalFlashcardGeneratorArgs args):base(args)
        {
            if (args.IntervalTypes == 0)
                throw new ArgumentException("Need at least one interval type to construct an interval flashcard generator.");

            IntervalTypes = args.IntervalTypes;
        }

        protected override IEnumerable<Flashcard> GenerateFlashcards(Staff staff, int lower, int upper)
        {
            foreach (Interval interval in Enum.GetValues(typeof(Interval)))
            {
                if (interval == Interval.All)
                    continue;

                if (IntervalTypes.HasFlag(interval))
                {
                    foreach (var bottomNote in Enumerable.Range(lower, upper - lower + 1 - interval.NumberOfSemitones()))
                    {
                        foreach (var bottomRep in new Note(bottomNote).GetRepresentations())
                        {
                            var reps = bottomRep.MakeGroup(interval);
                            if (reps != null && reps.All(r => Accidentals.HasFlag(r.Accidental)))
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
