using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanford.Multimedia.Midi;
using Lightspeed;
using System.Data.Entity;

namespace TestConsole
{    
    /// <summary>
    /// Run this EXE to generate the flashcard PNG files.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            RenderNotes();
        }

        private static void RenderNotes()
        {
            LilypondUtils.RenderAllNotes(@"C:\Program Files (x86)\LilyPond\usr\bin\lilypond.exe", @"C:\users\james\outputtest");
        }
    }
}
