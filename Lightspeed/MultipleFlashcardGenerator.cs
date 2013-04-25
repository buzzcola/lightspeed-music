using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// This generator contains other generators and can randomly select flashcards from each of them.
    /// </summary>
    public class MultipleFlashcardGenerator:FlashcardGenerator
    {
        readonly FlashcardGenerator[] _generators;

        /// <summary>
        /// Creates a generator that can create multiple types of flashcards.
        /// </summary>
        /// <param name="generators">One or more generators that will be used.</param>
        public MultipleFlashcardGenerator(params FlashcardGenerator[] generators)
        {
            _generators = generators;
        }

        public override IEnumerable<Flashcard> GenerateFlashcards()
        {
            return _generators.SelectMany(g => g.GenerateFlashcards());            
        }

        /// <summary>
        /// The parent class will choose one from all of the flashcards, which will weight the results toward
        /// the generators that make the largest number of cards.  Instead choose a random generator first, then
        /// ask it for a random flashcard, creating an even distribution of flashcard types.
        /// </summary>
        public override Flashcard Next()
        {
            var index = r.Next(_generators.Length);
            return _generators[index].Next();
        }
    }
}
