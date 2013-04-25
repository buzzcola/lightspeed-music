using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Describes how everything went for the player regarding one flashcard.
    /// </summary>
    public class FlashcardResult
    {
        #region Properties

        /// <summary>
        /// The flashcard that was used in play.
        /// </summary>
        public readonly Flashcard Flashcard;

        /// <summary>
        /// The outcome.
        /// </summary>
        public readonly FlashcardResultType ResultType;

        /// <summary>
        /// The time, in ticks, it took the player to respond.  If the flashcard was missed, this will be zero.
        /// </summary>
        public readonly long ResponseTime;

        #endregion
      
        #region Constructors

        private FlashcardResult(Flashcard card, FlashcardResultType resultType, DateTime? flashcardStartTime = null)
        {
            ResultType = resultType;
            if(flashcardStartTime.HasValue)
                ResponseTime = (DateTime.Now - flashcardStartTime.Value).Ticks;
        }

        public static FlashcardResult Missed(Flashcard card)
        {
            return new FlashcardResult(card, FlashcardResultType.Missed);
        }

        public static FlashcardResult Correct(Flashcard card, DateTime flashcardStartTime)
        {
            return new FlashcardResult(card, FlashcardResultType.Correct, flashcardStartTime);
        }

        public static FlashcardResult Incorrect(Flashcard card, DateTime flashcardStartTime)
        {
            return new FlashcardResult(card, FlashcardResultType.Incorrect, flashcardStartTime);
        }

        #endregion

        #region Methods

        public TimeSpan GetResponseTime()
        {
            return TimeSpan.FromTicks(ResponseTime);
        }

        #endregion
    }
}
