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
using System.Windows.Shapes;
using Lightspeed;

namespace LightspeedInterface
{
    /// <summary>
    /// Interaction logic for KeyboardWindow.xaml
    /// </summary>
    public partial class Keyboard : Window
    {
        const double BLACK_KEY_HEIGHT = 3.75;
        const double BLACK_KEY_WIDTH = 0.55;
        const double WHITE_KEY_HEIGHT = 5.875;
        const double WHITE_KEY_WIDTH = 0.875;

        readonly VirtualKeyboard _keyboard;

        public Keyboard(VirtualKeyboard keyboard)
        {
            InitializeComponent();
            var us = UserSettings.Instance;
            if (us.WindowLocation_VirtualKeyboard.HasValue)
            {
                Left = us.WindowLocation_VirtualKeyboard.Value.X;
                Top = us.WindowLocation_VirtualKeyboard.Value.Y;
            }

            this.Closed += new EventHandler(Keyboard_Closed);
            App.Current.Properties["KeyboardWindow"] = this;
            _keyboard = keyboard;
            BuildKeyboard();
        }

        /// <summary>
        /// Build a beautiful piano keyboard for the user to play!
        /// </summary>
        private void BuildKeyboard()
        {
            // two rows in the grid (top row will hold black keys, white will span both.)
            grdKeyboard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(BLACK_KEY_HEIGHT, GridUnitType.Star) });
            grdKeyboard.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(WHITE_KEY_HEIGHT - BLACK_KEY_HEIGHT, GridUnitType.Star) });

            // add 52 columns to the grid (one for each white key.)
            foreach (var i in Enumerable.Range(0, 52))
            {
                grdKeyboard.ColumnDefinitions.Add(new ColumnDefinition() { });
            }

            var currentColumn = -1;
            var octavePosition = 9; //0-11: position of first note within octave (i.e. 9 is A.)
            var blackKeyPositions = new int[] { 1, 3, 6, 8, 10 };

            // must assume first note is white key - first key black not supported.
            foreach (var note in Enumerable.Range(0, 88))
            {
                var b = new Button() { Background = Brushes.White };
                b.Tag = note + 1;
                b.PreviewMouseDown += new MouseButtonEventHandler(b_MouseDown);
                b.PreviewMouseUp += new MouseButtonEventHandler(b_MouseUp);
                //b.Click += new RoutedEventHandler(b_Click);

                var isWhiteKey = !blackKeyPositions.Contains(octavePosition);
                if (isWhiteKey)
                    currentColumn++; // white key, move to next column.                                

                if (isWhiteKey)
                {
                    Grid.SetRowSpan(b, 2); // full height.
                    b.Background = Brushes.White;
                    grdKeyboard.Children.Add(b);
                    Grid.SetColumn(b, currentColumn);
                }
                else
                {
                    b.Background = Brushes.Black;

                    // place the black key in a new three-column grid that straddles two white key columns.
                    Grid grdBlackHolder = new Grid();
                    grdBlackHolder.RowDefinitions.Add(new RowDefinition());
                    var emptyColumnWidth = WHITE_KEY_WIDTH - (BLACK_KEY_WIDTH / 2);
                    grdBlackHolder.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(emptyColumnWidth, GridUnitType.Star) });
                    grdBlackHolder.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(BLACK_KEY_WIDTH, GridUnitType.Star) });
                    grdBlackHolder.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(emptyColumnWidth, GridUnitType.Star) });

                    Grid.SetColumnSpan(grdBlackHolder, 2); // straddle two columns.
                    Grid.SetColumn(grdBlackHolder, currentColumn);
                    Grid.SetZIndex(grdBlackHolder, Grid.GetZIndex(b) + 2);
                    Grid.SetColumn(b, 1);
                    grdBlackHolder.Children.Add(b);
                    grdKeyboard.Children.Add(grdBlackHolder);
                }

                octavePosition = (octavePosition + 1) % 12;
            }
        }

        void Keyboard_Closed(object sender, EventArgs e)
        {
            // discard saved size information if they closed the keyboard, so next time it
            // opens it has that nice piano aspect ratio again.
            var us = UserSettings.Instance;
            us.WindowLocation_VirtualKeyboard = null;
            us.Save();
        }

        void b_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var noteNumber = (int)(sender as Button).Tag;
            _keyboard.ReleaseNote(noteNumber);
        }

        void b_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var noteNumber = (int)(sender as Button).Tag;
            _keyboard.PlayNote(noteNumber);
        }
    }
}
