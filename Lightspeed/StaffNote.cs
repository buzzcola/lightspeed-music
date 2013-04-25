using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Contains inforamtion about a NoteRepresentation to be displayed on a staff.
    /// </summary>
    public class StaffNote
    {
        /// <summary>
        /// Creates a new StaffNote.
        /// </summary>
        public StaffNote(NoteRepresentation noteRepresentation, Staff staff)
        {
            Staff = staff;
            NoteRepresentation = noteRepresentation;
        }

        /// <summary>
        /// The note representation that will be displayed.
        /// </summary>
        public readonly NoteRepresentation NoteRepresentation;

        /// <summary>
        /// Describes the staff upon which the note will be placed, which determines whether
        /// the note is to be played in the left or right hand.
        /// </summary>
        public readonly Staff Staff;

        /// <summary>
        /// A string that uniquely identifies this StaffNote.
        /// </summary>
        internal string MakeUniqueString()
        {
            return String.Format("{0}-{1}", NoteRepresentation.MakeUniqueString(), (int)Staff);
        }
    }
}
