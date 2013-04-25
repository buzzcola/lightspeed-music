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
            var set = UserSettings.Instance;
            var match = cmbInputDevice.Items.Cast<ComboBoxItem>()
                .Where(cbi => (int?)cbi.Tag == set.MIDIInputDeviceNumber)
                .FirstOrDefault();
            if (match != null)
                match.IsSelected = true;

            match = cmbOutputDevice.Items.Cast<ComboBoxItem>()
                .Where(cbi => (int?)cbi.Tag == set.MIDIOutputDeviceNumber)
                .FirstOrDefault();

            if (match != null)
                match.IsSelected = true;

            chkSingle.IsChecked = set.FlashcardTypes_Single ?? false;
            chkInterval.IsChecked = set.FlashcardTypes_Interval ?? false;
            chkTriad.IsChecked = set.FlashcardTypes_Triad ?? false;
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
            SaveSettings();
            Close();
        }

        /// <summary>
        /// Save all of the settings configured in these controls into the usersettings class.
        /// </summary>
        private void SaveSettings()
        {
            var set = UserSettings.Instance;
            if (cmbInputDevice.SelectedItem != null)
                set.MIDIInputDeviceNumber = (int?)((ComboBoxItem)cmbInputDevice.SelectedItem).Tag;
            if (cmbOutputDevice.SelectedItem != null)
                set.MIDIOutputDeviceNumber = (int?)((ComboBoxItem)cmbOutputDevice.SelectedItem).Tag;

            set.FlashcardTypes_Single = chkSingle.IsChecked;
            set.FlashcardTypes_Interval = chkInterval.IsChecked;
            set.FlashcardTypes_Triad = chkTriad.IsChecked;

            set.Save();
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
