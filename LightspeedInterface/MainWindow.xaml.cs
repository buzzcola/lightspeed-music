using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lightspeed;
using System.Resources;
using System.Media;

namespace LightspeedInterface
{
    /// <summary>
    /// Hosts a game of LightSpeed!
    /// </summary>
    public partial class MainWindow : Window
    {        
        Game Game;
        SoundEffects SoundEffects = new SoundEffects();
        VirtualKeyboard _virtualKeyboard;
        Keyboard _virtualKeyboardWindow;

        int flashcardSeconds { get { return 5; } }
        int gameSeconds { get { return 60; } }
        
        public MainWindow()
        {
            InitializeComponent();

            var us = UserSettings.Instance;            
            if (us.WindowLocation_Main.HasValue)
            {
                Left = us.WindowLocation_Main.Value.X;
                Top = us.WindowLocation_Main.Value.Y;
            }

            // register this window with the main app.
            App.Current.Properties["MainWindow"] = this;
            _virtualKeyboard = new VirtualKeyboard();

            if (us.ShowVirtualKeyboard)
                ShowVirtualKeyboard();
        }

        private void mnuStart_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        /// <summary>
        /// Attempt to create a new game and start it.  Won't do anything if a game is in progress.
        /// </summary>
        private void StartGame()
        {
            if (Game == null || Game.GameMode == GameMode.Finished)
            {
                Game = new Game(flashcardSeconds, gameSeconds, _virtualKeyboard);
                Game.FlashcardResponded += new EventHandler<FlashcardResultEventArgs>(Game_FlashcardResponded);
                Game.NextFlashcard += new EventHandler<FlashcardEventArgs>(Game_NextFlashcard);
                Game.FlashcardTimeExpired += new EventHandler<FlashcardEventArgs>(Game_FlashcardTimeExpired);
                Game.GameTimeExpired += new EventHandler<GameOverEventArgs>(Game_GameTimeExpired);
                DispatchUpdateScoreboard();
                Game.Start();
            }
        }

        /// <summary>
        /// The user responded to a flashcard.
        /// </summary>
        void Game_FlashcardResponded(object sender, FlashcardResultEventArgs e)
        {
            // if the user doesn't have MIDI output configured, play "correct" and "incorrect" sounds
            // to provide some feedback.
            if (!UserSettings.Instance.MIDIOutputDeviceNumber.HasValue)
            {
                if (e.Result.ResultType == FlashcardResultType.Correct)
                    SoundEffects.Play(Sound.ResultCorrect);
                else if (e.Result.ResultType == FlashcardResultType.Incorrect && !UserSettings.Instance.MIDIOutputDeviceNumber.HasValue)
                    SoundEffects.Play(Sound.ResultIncorrect);
            }
            DispatchUpdateScoreboard();
        }

        /// <summary>
        /// Time ran out so the game is over.  Write a helpful summary to the log area.
        /// </summary>
        void Game_GameTimeExpired(object sender, GameOverEventArgs e)
        {
            SoundEffects.Play(Sound.GameOver);
            ClearLog();
            Log("Game Over!");
            var results = e.Result.FlashcardResults;

            if (results.Any(r => r.ResultType != FlashcardResultType.Missed))
            {
                Log("Got {0} Right, {1} Wrong, {2} Missed.",
                    results.Where(r => r.ResultType == FlashcardResultType.Correct).Count(),
                    results.Where(r => r.ResultType == FlashcardResultType.Incorrect).Count(),
                    results.Where(r => r.ResultType == FlashcardResultType.Missed).Count());
                Log("Average response time: {0:0.000}s",
                    results.Where(r => r.ResultType != FlashcardResultType.Missed)
                    .Average(r => r.GetResponseTime().TotalSeconds));
                if (results.Any(r => r.ResultType == FlashcardResultType.Correct))
                {
                    Log("Best correct response time: {0:0.000}s",
                        results.Where(r => r.ResultType == FlashcardResultType.Correct)
                        .Min(r => r.GetResponseTime().TotalSeconds));
                }
            }
        }

        /// <summary>
        /// The game has advanced to a new flashcard.  Get the flashcard's image and display it.
        /// </summary>
        void Game_NextFlashcard(object sender, FlashcardEventArgs e)
        {
            SoundEffects.Play(Sound.NextFlashcard);
            Dispatcher.Invoke((Action)(() =>
            {
                var filename = e.Flashcard.GetFileName();
                imgFlashcard.Source = null;
                var pngStream = GetType().Assembly.GetManifestResourceStream("LightspeedInterface.Images." + filename);
                PngBitmapDecoder decoder = new PngBitmapDecoder(pngStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                imgFlashcard.Source = bitmapSource;
            }));
        }

        /// <summary>
        /// The player ran out of time and missed the flashcard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Game_FlashcardTimeExpired(object sender, FlashcardEventArgs e)
        {
            SoundEffects.Play(Sound.ResultTimeout);
            DispatchUpdateScoreboard();
        }

        void DispatchUpdateScoreboard()
        {
            Dispatcher.Invoke((Action)UpdateScoreboard);
        }

        /// <summary>
        /// Update all of the numbers on the scoreboard.
        /// </summary>
        void UpdateScoreboard()
        {
            lblPoints.Content = Game.Points.ToString();
            lblRight.Content = Game.GetResultCount(FlashcardResultType.Correct);
            lblWrong.Content = Game.GetResultCount(FlashcardResultType.Incorrect);
            lblMissed.Content = Game.GetResultCount(FlashcardResultType.Missed);
        }

        /// <summary>
        /// Empty out the log area of the window.
        /// </summary>
        void ClearLog()
        {
            Dispatcher.Invoke((Action)(() => txtLog.Text = ""));
        }

        /// <summary>
        /// Write a message to the log area of the window.
        /// </summary>
        void Log(string s, params object[] args)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                txtLog.Text = txtLog.Text + String.Format(s, args) + Environment.NewLine;
            }));
        }

        /// <summary>
        /// Menu "cancel" was clicked.
        /// </summary>
        void mnuCancelGame_Click(object sender, RoutedEventArgs e)
        {
            CancelGame();
        }

        /// <summary>
        /// Menu "Exit" was clicked.
        /// </summary>
        void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Call off the game in progress, if there is one.
        /// </summary>
        private void CancelGame()
        {
            if (Game != null)
            {
                Game.Stop();
            }
        }

        /// <summary>
        /// Handle hotkeys here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    StartGame();
                    break;
                case Key.Escape:
                    CancelGame();
                    break;
            }
        }

        /// <summary>
        /// Menu "Change Settings" was clicked.
        /// </summary>
        private void mnuChangeSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        /// <summary>
        /// Menu "show keyboard" was clicked.
        /// </summary>
        private void mnuShowKeyboard_Click(object sender, RoutedEventArgs e)
        {
            ShowVirtualKeyboard();
            var us = UserSettings.Instance;
            us.ShowVirtualKeyboard = true;
            us.Save();
        }

        /// <summary>
        /// Open the virtual keyboard window.
        /// </summary>
        private void ShowVirtualKeyboard()
        {
            if (_virtualKeyboardWindow == null || !_virtualKeyboardWindow.IsLoaded)
                _virtualKeyboardWindow = new Keyboard(_virtualKeyboard);
            _virtualKeyboardWindow.Show();            
        }
    }
}
