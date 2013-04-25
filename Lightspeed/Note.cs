using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// One keyboard note.  A note can have multiple enharmonic representations and might be drawn on different clefs.   
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Make a note for the specified number.
        /// </summary>
        public Note(int number)
        {
            Number = number;
        }

        #region Properties

        /// <summary>
        /// The number of the note, where the bottom note of an 88-key keyboard is 1, and
        /// the top note is 88.
        /// </summary>
        public readonly int Number;

        /// <summary>
        /// The note's number in MIDI - sending a note on/off message for this number will play the note on a midi device.
        /// </summary>
        public int MIDINumber { get { return MIDIUtils.ConvertNoteNumberToMIDINoteNumber(Number); } }

        #endregion

        #region Methods
        
        /// <summary>
        /// Determines whether this note corresponds to an accidental, a.k.a. a black key on the piano.
        /// </summary>
        public bool IsAccidental()
        {
            return _accidentalSteps.Contains((Number - 1) % 12);
        }
        // zero-based index of accidental steps in the first 12 notes of the keyboard.
        static readonly int[] _accidentalSteps = new int[] { 1, 4, 6, 9, 11 };

        /// <summary>
        /// Get all supported enharmonic represntations for this note.
        /// </summary>
        /// <returns></returns>
        public NoteRepresentation[] GetRepresentations()
        {
            if (!IsAccidental()) // natural note - easy.
                return new NoteRepresentation[] { new NoteRepresentation(this) };
            else
            {
                // get the note names of upper and lower neighbours, and return the sharp and flat of those respectively.
                return new NoteRepresentation[]{
                    new NoteRepresentation(this, AccidentalType.Sharp, NoteRepresentation.GetNaturalNoteName(new Note(this.Number - 1))),
                    new NoteRepresentation(this, AccidentalType.Flat, NoteRepresentation.GetNaturalNoteName(new Note(this.Number + 1)))
                };
            }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create a note from a midi number.
        /// </summary>
        public static Note FromMIDI(int midiNoteNumber)
        {
            return new Note(MIDIUtils.ConvertMIDINoteNumberToNoteNumber(midiNoteNumber));
        }

        /// <summary>
        /// Get all the notes on the keyboard that are naturals (white keys.)
        /// </summary>
        public static Note[] GetNaturals()
        {
            if(_naturals == null)
            {
                _naturals = Enumerable.Range(1, 88)
                    .Select(n => new Note(n))
                    .Where(n => !n.IsAccidental())
                    .ToArray();
            }
            return _naturals;            
        }
        static Note[] _naturals;

        /// <summary>
        /// Get all the notes on the keyboard that are accidentals (black keys.)
        /// </summary>
        public static Note[] GetAccidentals()
        {
            if (_accidentals == null)
            {
                _accidentals = Enumerable.Range(1, 88)
                    .Select(n => new Note(n))
                    .Where(n => !n.IsAccidental())
                    .ToArray();
            }
            return _accidentals;            
        }
        static Note[] _accidentals;

        #endregion

        #region Equality
        
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var n = obj as Note;
            if (n == null)
                return false;
            return Number == n.Number;
        }

        public override int GetHashCode()
        {
            return Number;
        }

        #endregion
    }
}
