using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Generates major and minor triads in all inversions.
    /// </summary> 
    public class TriadFlashcardGenerator:StaffFlashcardGenerator
    {
        /// <summary>
        /// This flags enum determines the types of triads that will be generated.
        /// </summary>
        public readonly TriadType? TriadTypes;

        /// <summary>
        /// This flags enum determines the inversions that will be generated.
        /// </summary>
        public readonly TriadInversion? TriadInversions;
                
        /// <summary>
        /// Creates a new TriadFlashcardGenerator.
        /// </summary>
        /// <param name="triadTypes">This flags value indicates which types of triads to include.</param>
        /// <param name="triadInversions">This flags value indicates which triad inversions to include.</param>
        /// <param name="staffs">This flags value indicates which staffs to include.</param>
        public TriadFlashcardGenerator(TriadType? triadTypes = null, TriadInversion? triadInversions = null, Staff? staffs = null)
            :base(staffs)
        {
            TriadTypes = triadTypes;
            TriadInversions = triadInversions;
        }

        protected override IEnumerable<Flashcard> GenerateFlashcards(Staff stave, int lower, int upper)
        {
            foreach(var noteNumber in Enumerable.Range(lower, upper - lower + 1))
            {
                foreach (var rep in new Note(noteNumber).GetRepresentations())
                {
                    foreach (TriadType type in Enum.GetValues(typeof(TriadType)))
                    {
                        if (TriadTypes.HasValue && !TriadTypes.Value.HasFlag(type)) continue;

                        if (type == TriadType.Minor || type == TriadType.Major) // invertible types
                        {
                            foreach (TriadInversion inversion in Enum.GetValues(typeof(TriadInversion)))
                            {
                                if (TriadInversions.HasValue && !TriadInversions.Value.HasFlag(inversion)) continue;

                                var triad = MakeTriad(rep, type, inversion);
                                if (triad != null)
                                {
                                    var staffNotes = triad.Select(r => new StaffNote(r, stave)).ToArray();
                                    if (staffNotes.Last().NoteRepresentation.Note.Number <= upper) // only return chords that fit within the upper bound.
                                        yield return new Flashcard(staffNotes);
                                }
                            }

                        }
                        /* non-invertible types - when inverted, diminished and augmented triads just 
                         * make diminished and augmented triads starting from other notes, causing duplicates in the set. */
                        else 
                        {
                            var triad = MakeTriad(rep, type);
                            if (triad != null)
                            {
                                var staffNotes = triad.Select(r => new StaffNote(r, stave)).ToArray();
                                if (staffNotes.Last().NoteRepresentation.Note.Number <= upper) // only return chords that fit within the upper bound.
                                    yield return new Flashcard(staffNotes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes an array of NoteRepresentations to form a triad.
        /// </summary>
        private NoteRepresentation[] MakeTriad(NoteRepresentation rep, TriadType type, TriadInversion inv = TriadInversion.Root)
        {
            switch (type)
            {
                case TriadType.Major:
                    switch (inv)
                    {
                        case TriadInversion.Root:
                            return rep.MakeGroup(Interval.MajorThird, Interval.PerfectFifth);
                        case TriadInversion.First:
                            return rep.MakeGroup(Interval.MinorThird, Interval.MinorSixth);
                        case TriadInversion.Second:
                            return rep.MakeGroup(Interval.PerfectFourth, Interval.MajorSixth);                          
                    }
                    break;
                case TriadType.Minor:
                    switch (inv)
                    {
                        case TriadInversion.Root:
                            return rep.MakeGroup(Interval.MinorThird, Interval.PerfectFifth);
                        case TriadInversion.First:
                            return rep.MakeGroup(Interval.MajorThird, Interval.MajorSixth);
                        case TriadInversion.Second:
                            return rep.MakeGroup(Interval.PerfectFourth, Interval.MinorSixth);
                    }
                    break;
                case TriadType.Diminished: // since diminished and augmented triads are symmetrical, they don't invert.
                    return rep.MakeGroup(Interval.MinorThird, Interval.DiminishedFifth);
                case TriadType.Augmented:
                    return rep.MakeGroup(Interval.MajorThird, Interval.AugmentedFifth);                
            }
            return null;
        }
                
        /// <summary>
        /// Randomly select a triad type, then a card to keep the distribution even between types.
        /// </summary>        
        public override Flashcard Next()
        {
            var typeArray = Enum.GetValues(typeof(TriadType)) as TriadType[];
            var selectedTypes = typeArray.Where(t => !TriadTypes.HasValue || TriadTypes.Value.HasFlag(t)).ToArray();

            if (selectedTypes.Count() > 1)
            {
                // if there's more than one selected type, we want an even distribution of triad types, not
                // weighted toward the types that have more cards.  create an array of single-type generators
                // and randomly select one each time Next is called.
                if (_generators == null)
                {
                    _generators = new TriadFlashcardGenerator[selectedTypes.Count()];
                    for (int i = 0; i < _generators.Length; i++)
                        _generators[i] = new TriadFlashcardGenerator(selectedTypes[i], TriadInversions);
                }
                var randomType = r.Next(typeArray.Length);
                return _generators[randomType].Next();
            }
            
            // only one selected type - call base which will randomly select a flashcard.
            return base.Next();
        }
        private TriadFlashcardGenerator[] _generators;
    }
}
