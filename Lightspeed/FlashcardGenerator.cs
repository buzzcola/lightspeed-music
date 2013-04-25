using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// A class that can generate flashcards.
    /// </summary>
    public abstract class FlashcardGenerator
    {
        /// <summary>
        /// Make some flashcards!
        /// </summary>
        public abstract IEnumerable<Flashcard> GenerateFlashcards();
                
        /// <summary>
        /// A pre-instantiated Random.
        /// </summary>
        protected readonly Random r = new Random();

        /// <summary>
        /// Holds all the flashcards.
        /// </summary>
        private readonly List<Flashcard> _all = new List<Flashcard>();

        /// <summary>
        /// Produces one random flashcard.
        /// </summary>
        public virtual Flashcard Next()
        {
            if (_all.Count == 0)
                _all.AddRange(GenerateFlashcards());

            int index = r.Next(_all.Count);
            var result = _all[index];
            _all.RemoveAt(index);
            return result;
        }
    }
}
