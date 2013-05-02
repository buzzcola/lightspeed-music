using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Sanford.Multimedia.Midi;

namespace Lightspeed
{
    /// <summary>
    /// A Game of Lightspeed.  Create one, start it, play it until time runs out, then get rid of it!
    /// </summary>
    public class Game
    {
        #region Events

        /// <summary>
        /// Raised three times on a one-second interval before the game starts.
        /// </summary>
        public event EventHandler<NewGameCountdownEventArgs> NewGameCountdown;

        /// <summary>
        /// Raised when the game starts at the end of the countdown.
        /// </summary>
        public event EventHandler GameStarted;

        /// <summary>
        /// Raised when the game has moved to a new flashcard.
        /// </summary>
        public event EventHandler<FlashcardEventArgs> NextFlashcard;

        /// <summary>
        /// Raised when the player has run out of time on the current flashcard.
        /// </summary>
        public event EventHandler<FlashcardEventArgs> FlashcardTimeExpired;

        /// <summary>
        /// Raised when the user hits a key in response to a flashcard.
        /// </summary>
        public event EventHandler<FlashcardResultEventArgs> FlashcardResponded;

        /// <summary>
        /// Raised when the game time has run out.
        /// </summary>
        public event EventHandler<GameOverEventArgs> GameTimeExpired;


        #endregion

        #region Constants

        /// <summary>
        /// The number of points the player gets for a correct flashcard response.
        /// </summary>
        const int POINTS_CORRECT = 5;

        /// <summary>
        /// The number of points the player gets for an incorrect flashcard response.  We can't just
        /// have them wildly mashing the keys!
        /// </summary>
        const int POINTS_INCORRECT = -5;

        /// <summary>
        /// The length of one game, in seconds.
        /// </summary>
        const int GAME_DURATION = 60;
        
        #endregion

        #region Private Properties

        /// <summary>
        /// Times the game from start to finish.
        /// </summary>
        Timer _gameTimer;
        
        /// <summary>
        /// Times one flashcard until the player responds or time runs out.
        /// </summary>
        Timer _flashcardTimer;
        
        /// <summary>
        /// Times the little pause in between flashcards.
        /// </summary>
        Timer _nextFlashcardTimer;

        /// <summary>
        /// Used for the countdown before a new game.
        /// </summary>
        Timer _newGameCountdownTimer;

        /// <summary>
        /// A MIDI input device to control the game.
        /// </summary>
        InputDevice _inputDevice;

        /// <summary>
        /// An optional MIDI output device to provide musical feedback.
        /// </summary>
        OutputDevice _outputDevice;

        /// <summary>
        /// A neverending provider of flashcards to play with.
        /// </summary>
        FlashcardGenerator _flashcardSource;

        /// <summary>
        /// The flashcard currently in play.
        /// </summary>
        Flashcard _currentFlashcard;

        /// <summary>
        /// The time when the current flashcard was displayed.
        /// </summary>
        DateTime _currentFlashcardStartTime;
        
        /// <summary>
        /// Tracks the user's current response.  For multi-note flashcards like intervals and triads, the MIDI messages 
        /// don't come in at exactly the same time so they have to be tracked to see if we've received all of the correct
        /// notes yet.
        /// </summary>
        Dictionary<Note, bool> _currentResponse;

        /// <summary>
        /// Holds all of the flashcard results from the game.
        /// </summary>
        List<FlashcardResult> _flashcardResults;

        /// <summary>
        /// The time when the game started.
        /// </summary>
        DateTime _gameStartTime;

        #endregion

        #region Public Properties

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameMode GameMode { get; private set; }
        
        /// <summary>
        /// The total points the player has earned.
        /// </summary>
        public int Points { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Game.
        /// </summary>
        /// <param name="flashcardSeconds">The maximum number of seconds that the player has to respond to a flashcard.</param>
        /// <param name="gameSeconds">The length of the game.  At the end of this time, it's game over.</param>
        /// <param name="virtualKeyboards">If supplied, the game will take input from these as well as the configured MIDI input device (if any.)</param>
        public Game(params VirtualKeyboard[] virtualKeyboards)
        {
            var settings = UserSettings.Instance;

            var types = new List<FlashcardGenerator>();
            if (settings.FlashcardTypes_Triad == true)
                types.Add(new TriadFlashcardGenerator(TriadType.All, TriadInversion.All, settings.Accidentals, settings.Staffs));
            if (settings.FlashcardTypes_Interval == true)
                types.Add(new IntervalFlashcardGenerator(Interval.All, settings.Staffs, settings.Accidentals));
            if (settings.FlashcardTypes_Single == true || !types.Any()) // if they set no flashcard types, give them single.
                types.Add(new SingleNoteFlashcardGenerator(settings.Staffs, settings.Accidentals));

            _flashcardSource = new MultipleFlashcardGenerator(types.ToArray());
            _currentResponse = new Dictionary<Note, bool>();
            _flashcardResults = new List<FlashcardResult>();

            // if the user changes MIDI devices mid-game, handle that.
            UserSettings.Instance.OutputDeviceChanged += new EventHandler(UserSettings_OutputDeviceChanged);
            UserSettings.Instance.InputDeviceChanged += new EventHandler(UserSettings_InputDeviceChanged);
            
            _outputDevice = UserSettings.Instance.GetOutputDevice();
            _inputDevice = UserSettings.Instance.GetInputDevice();

            if (_inputDevice != null)
                _inputDevice.ChannelMessageReceived += new EventHandler<ChannelMessageEventArgs>(ChannelMessageReceived);

            foreach(var vk in virtualKeyboards)            
                vk.ChannelMessageReceived += new EventHandler<ChannelMessageEventArgs>(ChannelMessageReceived);

            _gameTimer = new Timer(GAME_DURATION * 1000);
            _flashcardTimer = new Timer(settings.MaxFlashcardTime * 1000);
            _nextFlashcardTimer = new Timer(500);
            _newGameCountdownTimer = new Timer(1000);
            _nextFlashcardTimer.AutoReset = false;
            _gameTimer.Elapsed += new ElapsedEventHandler(GameTimer_Elapsed);
            _flashcardTimer.Elapsed += new ElapsedEventHandler(FlashcardTimer_Elapsed);
            _nextFlashcardTimer.Elapsed += new ElapsedEventHandler(NextFlashcardTimer_Elapsed);

            GameMode = GameMode.NotStarted;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// The MIDI input device has changed.  Grab the new one.
        /// </summary>
        void UserSettings_InputDeviceChanged(object sender, EventArgs e)
        {
            _inputDevice = UserSettings.Instance.GetInputDevice();
            if (_inputDevice != null)
                _inputDevice.ChannelMessageReceived += new EventHandler<ChannelMessageEventArgs>(ChannelMessageReceived);
        }

        /// <summary>
        /// The MIDI output device has changed.  Grab the new one.
        /// </summary>
        void UserSettings_OutputDeviceChanged(object sender, EventArgs e)
        {
            _outputDevice = UserSettings.Instance.GetOutputDevice();
        }

        /// <summary>
        /// The time limit for the game has run out.
        /// </summary>
        void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GameOver();
        }

        /// <summary>
        /// The time limit for the current Flashcard has run out before the user could respond.
        /// </summary>
        void FlashcardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FlashcardTimeUp();
        }

        /// <summary>
        /// We've received a MIDI message - handle it appropriately for the game state and message type.
        /// </summary>
        void ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {            
            CheckReceivedMessage(e.Message);
        }

        /// <summary>
        /// The pause between flashcards is complete.
        /// </summary>
        void NextFlashcardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {            
            GotoNextFlashcard();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the game by firing up the new game countdown timer.
        /// </summary>
        public void Start()
        {
            if (GameMode != Lightspeed.GameMode.NotStarted)
                return;

            GameMode = Lightspeed.GameMode.CountingDownToStart;
            int secondsToGo = 3;
            _newGameCountdownTimer.Elapsed += (s, e) =>
            {
                if (secondsToGo == 0)
                {
                    _newGameCountdownTimer.Stop();
                    BeginGame();
                }
                else
                {
                    if (NewGameCountdown != null)
                        NewGameCountdown(this, new NewGameCountdownEventArgs(secondsToGo));
                    secondsToGo--;
                }
            };
            _newGameCountdownTimer.Start();
        }

        /// <summary>
        /// The countdown is complete, so start the game.
        /// </summary>
        private void BeginGame()
        {
            if (GameMode != Lightspeed.GameMode.CountingDownToStart)
                return;

            _gameStartTime = DateTime.Now;
            GameMode = Lightspeed.GameMode.BetweenFlashcards;
            if (_inputDevice != null)
                _inputDevice.StartRecording();
            if (GameStarted != null)
                GameStarted(this, EventArgs.Empty);
            _gameTimer.Start();
            GotoNextFlashcard();
        }

        /// <summary>
        /// Stop the game.  Once stopped, it can't be restarted.
        /// </summary>
        public void Stop()
        {
            if (GameMode != Lightspeed.GameMode.Finished)
                GameOver();
        }

        /// <summary>
        /// Time ran out before the user hit a note.
        /// </summary>
        private void FlashcardTimeUp()
        {
            if (GameMode == Lightspeed.GameMode.InFlashcard)
            {
                _flashcardResults.Add(FlashcardResult.Missed(_currentFlashcard));
                GameMode = GameMode.BetweenFlashcards;
                if (FlashcardTimeExpired != null)
                    FlashcardTimeExpired(this, new FlashcardEventArgs(_currentFlashcard));
                _nextFlashcardTimer.Start();
            }
        }

        /// <summary>
        /// Advance to the next flashcard.
        /// </summary>
        private void GotoNextFlashcard()
        {
            var f = _flashcardSource.Next();
            _currentFlashcard = f;
            
            // populate the response dictionary with the notes in the flashcard
            // and a False value so they can be checked off as they are received.
            _currentResponse.Clear();
            foreach (var sn in f.StaffNotes)
                _currentResponse.Add(sn.NoteRepresentation.Note, false);
            
            _currentFlashcardStartTime = DateTime.Now;
            GameMode = Lightspeed.GameMode.InFlashcard;
            _flashcardTimer.Start();

            if (NextFlashcard != null)
                NextFlashcard(this, new FlashcardEventArgs(_currentFlashcard));            
        }

        /// <summary>
        /// It's all over!
        /// </summary>
        private void GameOver()
        {
            GameMode = GameMode.Finished;
            _gameTimer.Stop();
            _flashcardTimer.Stop();
            _nextFlashcardTimer.Stop();
            if (_inputDevice != null)
            {
                _inputDevice.StopRecording();
            }

            var result = new GameResult(_flashcardResults, _gameStartTime, Points);

            if (GameTimeExpired != null)
                GameTimeExpired(this, new GameOverEventArgs(result));
        }

        /// <summary>
        /// Evaluate a received midi message and take the appropriate action.
        /// </summary>
        private void CheckReceivedMessage(ChannelMessage channelMessage)
        {
            if (GameMode != GameMode.InFlashcard)
                return;
            if (channelMessage.Command != ChannelCommand.NoteOn)
                return;

            var receivedNote = Note.FromMIDI(channelMessage.Data1);
            bool correct = false;
            bool flashcardFinished = false;

            if (!_currentResponse.ContainsKey(receivedNote)) // incorrect note.
            {
                correct = false;
                flashcardFinished = true;

                // play the flashcard out on the MIDI device so the user hears the correct answer.
                if (_outputDevice != null)
                    PlayFlashcard(_currentFlashcard);
            }
            else
            {
                correct = true;
                _currentResponse[receivedNote] = true; // mark the note as complete.
                flashcardFinished = _currentResponse.Values.All(r => r); // if we got them all, the flashcard is correct.                
            }

            if (flashcardFinished)
            {
                _flashcardTimer.Stop();
                GameMode = GameMode.BetweenFlashcards;
                FlashcardResult result;
                int points;
                if (correct)
                {
                    result = FlashcardResult.Correct(_currentFlashcard, _currentFlashcardStartTime);
                    points = POINTS_CORRECT;
                }
                else
                {
                    result = FlashcardResult.Incorrect(_currentFlashcard, _currentFlashcardStartTime);
                    points = POINTS_INCORRECT;
                }

                Points += points;
                _flashcardResults.Add(result);

                if (FlashcardResponded != null)
                    FlashcardResponded(this, new FlashcardResultEventArgs(result));
                _nextFlashcardTimer.Start();
            }
        }

        /// <summary>
        /// Play the notes from a flashcard through the output device.
        /// </summary>
        /// <param name="f"></param>
        private void PlayFlashcard(Flashcard f)
        {
            ChannelMessageBuilder cbm = new ChannelMessageBuilder();
            cbm.Command = ChannelCommand.NoteOn;            
            cbm.Data2 = 75;
            cbm.MidiChannel = 0;
            cbm.Build();
            cbm.Command = ChannelCommand.NoteOff;
            var onMessages = new ChannelMessage[f.StaffNotes.Length];
            var offMessages = new ChannelMessage[f.StaffNotes.Length];

            for (var i = 0; i < f.StaffNotes.Length; i++)
            {
                cbm.Data1 = f.StaffNotes[i].NoteRepresentation.Note.MIDINumber;
                cbm.Command = ChannelCommand.NoteOn;
                cbm.Build();
                onMessages[i] = cbm.Result;
                cbm.Command = ChannelCommand.NoteOff;
                cbm.Build();
                offMessages[i] = cbm.Result;
            }

            var t = new Timer(500) { AutoReset = false };
            t.Elapsed += (s, a) => Array.ForEach(offMessages, m => _outputDevice.Send(m));
            Array.ForEach(onMessages, m => _outputDevice.Send(m));
            t.Start();
        }

        /// <summary>
        /// Get the count of Responses for the provided type.
        /// </summary>
        public int GetResultCount(FlashcardResultType type)
        {
            return _flashcardResults.Where(r => r.ResultType == type).Count();
        }

        #endregion

    }
}
