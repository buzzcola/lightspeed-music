using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lightspeed
{
    /// <summary>
    /// Describes what happened in a player's completed game.
    /// </summary>
    public class GameResult
    {
        public readonly DateTime GameDate;
        public readonly int Points;
        public readonly FlashcardResult[] FlashcardResults;

        public GameResult(IEnumerable<FlashcardResult> results, DateTime gameDate, int points)
        {
            FlashcardResults = results.ToArray();
            GameDate = gameDate;
            Points = points;
        }
    }
}
