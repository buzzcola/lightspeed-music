using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// A list of supported intervals.
    /// </summary>
    [Flags]
    public enum Interval
    {
        MinorSecond = 0x1,
        MajorSecond = 0x2,
        MinorThird = 0x4,
        MajorThird = 0x8,
        PerfectFourth = 0x10,
        AugmentedFourth = 0x20,
        DiminishedFifth = 0x40,
        PerfectFifth = 0x80,
        AugmentedFifth = 0x100,
        MinorSixth = 0x200,
        MajorSixth = 0x400,
        MinorSeventh = 0x800,
        MajorSeventh = 0x1000,
        Octave = 0x2000,
    }

    public static class IntervalExtensions
    {        
        /// <summary>
        /// Gets the number of note names that ought to be between two properly
        /// rendered notes representing this interval.  For exaple, a minor second
        /// should be rendered as a C and a Db (one note name difference), never
        /// C and C# (zero name difference.)
        /// </summary>
        public static int NumberOfNoteNames(this Interval i)
        {
            switch (i)
            {
                case Interval.MinorSecond:
                case Interval.MajorSecond:
                    return 1;
                case Interval.MinorThird:
                case Interval.MajorThird:
                    return 2;
                case Interval.PerfectFourth:
                case Interval.AugmentedFourth:
                    return 3;
                case Interval.PerfectFifth:
                case Interval.DiminishedFifth:
                case Interval.AugmentedFifth:
                    return 4;
                case Interval.MinorSixth:
                case Interval.MajorSixth:
                    return 5;
                case Interval.MinorSeventh:
                case Interval.MajorSeventh:
                    return 6;
                case Interval.Octave:
                    return 7;
                default:
                    throw new Exception("Unknown Interval type.");
            }
        }

        /// <summary>
        /// Gets the number of semitones represented by this interval.
        /// </summary>
        public static int NumberOfSemitones(this Interval i)
        {
            switch (i)
            {
                case Interval.MinorSecond:
                    return 1;
                case Interval.MajorSecond:
                    return 2;
                case Interval.MinorThird:
                    return 3;
                case Interval.MajorThird:
                    return 4;
                case Interval.PerfectFourth:
                    return 5;
                case Interval.AugmentedFourth:
                case Interval.DiminishedFifth:
                    return 6;
                case Interval.PerfectFifth:
                    return 7;
                case Interval.MinorSixth:
                case Interval.AugmentedFifth:
                    return 8;
                case Interval.MajorSixth:
                    return 9;
                case Interval.MinorSeventh:
                    return 10;
                case Interval.MajorSeventh:
                    return 11;
                case Interval.Octave:
                    return 12;
                default:
                    throw new Exception("Unknown Interval type.");
            }
        }
    }
}
