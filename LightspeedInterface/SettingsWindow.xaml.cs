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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {        
        public SettingsWindow()
        {
            InitializeComponent();
            LoadCombos();
            LoadSettings();
        }

        /// <summary>
        /// Get all the settings from the usersettings class and populate the controls.
        /// </summary>
        private void LoadSettings()
        {
            var settings = UserSettings.Instance;
            var match = cmbInputDevice.Items.Cast<ComboBoxItem>()
                .Where(cbi => (int?)cbi.Tag == settings.MIDIInputDeviceNumber)
                .FirstOrDefault();
            if (match != null)
                match.IsSelected = true;

            match = cmbOutputDevice.Items.Cast<ComboBoxItem>()
                .Where(cbi => (int?)cbi.Tag == settings.MIDIOutputDeviceNumber)
                .FirstOrDefault();

            if (match != null)
                match.IsSelected = true;

            chkSingle.IsChecked = settings.FlashcardTypes_Single ?? false;
            chkInterval.IsChecked = settings.FlashcardTypes_Interval ?? false;
            chkTriad.IsChecked = settings.FlashcardTypes_Triad ?? false;

            txtMaxFlashcardTime.Text = settings.MaxFlashcardTime.ToString();
        }

        /// <summary>
        /// Populate drop-down lists and such with data.
        /// </summary>
        private void LoadCombos()
        {
            cmbInputDevice.Items.Clear();
            cmbInputDevice.Items.Add(new ComboBoxItem() { Tag = null, Content = "None - Virtual Keyboard Only" });
            foreach (var item in MIDIUtils.GetMIDIInputDevices())
                cmbInputDevice.Items.Add(new ComboBoxItem() { Tag = item.Item1, Content = item.Item2 });

            cmbOutputDevice.Items.Clear();
            cmbOutputDevice.Items.Add(new ComboBoxItem() { Tag = null, Content = "No MIDI Output" });
            foreach (var item in MIDIUtils.GetMIDIOutputDevices())
                cmbOutputDevice.Items.Add(new ComboBoxItem() { Tag = item.Item1, Content = item.Item2 });
        }

        /// <summary>
        /// OK button was clicked.
        /// </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(SaveSettings())
                Close();
        }

        /// <summary>
        /// Save all of the settings configured in these controls into the usersettings class.
        /// </summary>
        private bool SaveSettings()
        {
            int maxFlashcardTime;
            if (!int.TryParse(txtMaxFlashcardTime.Text, out maxFlashcardTime) || maxFlashcardTime < 1)
            {
                MessageBox.Show("You must enter a whole number greater than zero for the Maximum Flashcard Time.");
                return false;
            }

            var settings = UserSettings.Instance;
            settings.MaxFlashcardTime = maxFlashcardTime;

            if (cmbInputDevice.SelectedItem != null)
                settings.MIDIInputDeviceNumber = (int?)((ComboBoxItem)cmbInputDevice.SelectedItem).Tag;
            if (cmbOutputDevice.SelectedItem != null)
                settings.MIDIOutputDeviceNumber = (int?)((ComboBoxItem)cmbOutputDevice.SelectedItem).Tag;

            settings.FlashcardTypes_Single = chkSingle.IsChecked;
            settings.FlashcardTypes_Interval = chkInterval.IsChecked;
            settings.FlashcardTypes_Triad = chkTriad.IsChecked;

            settings.Save();
            return true;
        }

        /// <summary>
        /// Cancel button was clicked.
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
