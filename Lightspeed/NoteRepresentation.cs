using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// One enharmonic representation of a keyboard note including a note name and accidental.
    /// </summary>
    public class NoteRepresentation
    {
        /// <summary>
        /// Creates a new note representation.
        /// </summary>
        internal NoteRepresentation(Note note, AccidentalType accidental, NoteName name)
        {
            Note = note;
            Accidental = accidental;
            Name = name;
        }

        /// <summary>
        /// Creates a representation for a natural.
        /// </summary>
        /// <param name="naturalNote">Can't be an accidental.</param>
        internal NoteRepresentation(Note naturalNote)
        {
            if (naturalNote.IsAccidental())
                throw new Exception("Only a natural can be passed to this constructor.");
            
            Note = naturalNote;
            Name = GetNaturalNoteName(naturalNote);
            Accidental = AccidentalType.Natural;
        }

        /// <summary>
        /// The keyboard note being represented.
        /// </summary>
        public readonly Note Note;

        /// <summary>
        /// The accidental that describes the sharpness, flatness, or natural-ness of the reprsentation.
        /// </summary>
        public readonly AccidentalType Accidental;
        
        /// <summary>
        /// The note's name in this representation.
        /// </summary>
        public readonly NoteName Name;

        /// <summary>
        /// Add an interval to this representation.
        /// </summary>
        /// <param name="interval">The interval to add.</param>
        /// <param name="direction">Which direction to go - up or down.</param>
        /// <returns>Null if there is no supported enharmonically correct representation of the requested interval.  For example,
        /// adding a minor second to a Bb should return a Cb, but this is not supported so the result will be null.  If there is
        /// a valid representation (for example, A# plus a minor second correctly yields B) it will be returned.</returns>
        public NoteRepresentation AddInterval(Interval interval, IntervalDirection direction = IntervalDirection.Up)
        {
            var coefficient = direction == IntervalDirection.Up ? 1 : -1;
            var newNoteNumber = this.Note.Number + (interval.NumberOfSemitones() * coefficient);
            var newNoteName = this.Name.AddNoteNames(interval.NumberOfNoteNames() * coefficient);
            var result = new Note(newNoteNumber).GetRepresentations().SingleOrDefault(r => r.Name == newNoteName);
            return result;
        }

        /// <summary>
        /// Make a string that is unique to this representation.
        /// </summary>
        /// <returns></returns>
        public string MakeUniqueString()
        {
            return String.Format("{0}{1}", Note.Number, Name);
        }

        public static NoteName GetNaturalNoteName(Note naturalNote)
        {
            if (naturalNote.IsAccidental())
                throw new Exception("Argument must be a natural.");
            return (NoteName)(Array.IndexOf(Note.GetNaturals(), naturalNote) % 7);
        }

        /// <summary>
        /// Make a group of note representations using this one as the bottom and applying
        /// a list of intervals.  For example, passing in a major third and a perfect fifth
        /// will return the representations for a major chord.
        /// </summary>
        /// <returns>The requested representations, or Null, if the requested intervals point to notes that can't be represented
        /// using supported enharmonic representations (for example, an A# major chord
        /// would return null, since the system doesn't support C## for the third.)</returns>
        public NoteRepresentation[] MakeGroup(params Interval[] intervals)
        {
            var result = new NoteRepresentation[intervals.Length + 1];
            result[0] = this;
            for (int i = 0; i < intervals.Length; i++)
            {
                var rep = this.AddInterval(intervals[i]);
                if (rep == null)
                    return null; // there's at least one non-representable note in this group, so don't do it.
                result[i + 1] = rep;
            }
            return result;
        }

        /// <summary>
        /// A human-readable name for the representation.  Note is identified using "tuner's" notation (notes
        /// numbered from 1-88.)
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}{1} ({2})",
                Name,
                Accidental == AccidentalType.Natural ? ""
                  : Accidental == AccidentalType.Sharp ? "#"
                  : "b",
                Note.Number);
        }
    }
}
