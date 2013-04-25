using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// One flashcard that can be displayed to a user playing the game.
    /// </summary>
    public class Flashcard
    {        
        // TODO: these are the lowest and highest note numbers to use for the left and right hands.
        // Currently they constrain notes to a maximum of three leger lines on each clef, but they
		// shoudl be configurable to support different sizes of keyboards.

		/// <summary>
		/// The lowest note supported for the left hand (third leger line below the bass clef.)
		/// </summary>
        public const int LOWER_BOUND_LH = 13;

		/// <summary>
		/// The highest note supported for the left hand (third leger line above the bass clef.)
		/// </summary>
        public const int UPPER_BOUND_LH = 47;        
        
		/// <summary>
		/// The lowest note supported for the right hand (third leger line below the treble clef.)
		/// </summary>
		public const int LOWER_BOUND_RH = 33;

		/// <summary>
		/// The highest note supported for the right hand (third leger line above the trebleclef.)
		/// </summary>
        public const int UPPER_BOUND_RH = 68;

        /// <summary>
        /// The type of the flashcard (see FlashcardType for types.)
        /// </summary>
        public readonly FlashcardType Type;

        /// <summary>
        /// A set of note representations to display on this flashcard.
        /// </summary>
        public readonly StaffNote[] StaffNotes;

        /// <summary>
        /// Makes a new flashcard with a specific clef and set of notes.
        /// </summary>
        internal Flashcard(params StaffNote[] staffNotes)
        {
            if (staffNotes.Any(sn => sn.Staff == Staff.LeftHand && (sn.NoteRepresentation.Note.Number < LOWER_BOUND_LH || sn.NoteRepresentation.Note.Number > UPPER_BOUND_LH)))
                throw new ArgumentException("Unsupported note number.");
            else if (staffNotes.Any(sn => sn.Staff == Staff.RightHand && (sn.NoteRepresentation.Note.Number < LOWER_BOUND_RH || sn.NoteRepresentation.Note.Number > UPPER_BOUND_RH)))
                throw new ArgumentException("Unsupported note number.");

            StaffNotes = staffNotes;
        }

        /// <summary>
        /// Make a string that uniquely identifies this flashcard.
        /// </summary>
        /// <returns></returns>
        public string MakeUniqueString()
        {
            return String.Join("_", StaffNotes.Select(sn => sn.MakeUniqueString()).ToArray());            
        }
                
        /// <summary>
        /// Get the filename for an image of this flashcard.
        /// </summary>
        /// <returns></returns>
        public string GetFileName()
        {
            return MakeUniqueString() + ".png";
        }
    }
}
