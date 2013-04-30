using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows;
using Sanford.Multimedia.Midi;

namespace Lightspeed
{
    /// <summary>
    /// Provides access to all user-configurable functions of Lightspeed.
    /// </summary>
    /// <remarks>
    /// This class implements the Singleton pattern and provides a settings object with properties, methods and events that are
    /// static to the running Process.
    /// </remarks>
    public class UserSettings : ApplicationSettingsBase
    {
        #region Singleton

        private UserSettings() { }
        static UserSettings _singleton;
        public static UserSettings Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new UserSettings();
                return _singleton;
            }
        }

        #endregion

        #region Methods

        ~UserSettings()
        {
            DisposeDevices();
        }

        /// <summary>
        /// Release any open MIDI devices.
        /// </summary>
        public void DisposeDevices()
        {
            if (_inputDevice != null)
                _inputDevice.Close();
            if (_outputDevice != null)
                _outputDevice.Close();
        }

        #endregion

        #region Events

        /// <summary>
        /// The user has switched the MIDI output device.
        /// </summary>
        public event EventHandler OutputDeviceChanged;

        /// <summary>
        /// The user switched the MIDI input device.
        /// </summary>
        public event EventHandler InputDeviceChanged;

        #endregion

        #region User Settings

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public int? MIDIInputDeviceNumber
        {
            get { return (int?)this["MIDIInputDeviceNumber"]; }
            set 
            { 
                this["MIDIInputDeviceNumber"] = value;
                if (_inputDevice != null)
                {
                    _inputDevice.Close();
                    _inputDevice = null;
                }
                if (InputDeviceChanged != null)
                    InputDeviceChanged(this, EventArgs.Empty);
            }
        }
                
        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public int? MIDIOutputDeviceNumber
        {
            get { return (int?)this["MIDIOutputDeviceNumber"]; }
            set 
            { 
                this["MIDIOutputDeviceNumber"] = value;
                if (_outputDevice != null)
                {
                    _outputDevice.Close();
                    _outputDevice = null;
                }
                if (OutputDeviceChanged != null)
                    OutputDeviceChanged(this, EventArgs.Empty);
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        public bool ShowVirtualKeyboard
        {
            get { return (bool)this["ShowVirtualKeyboard"]; }
            set { this["ShowVirtualKeyboard"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public Point? WindowLocation_Main
        {
            get { return (Point?)this["WindowLocation_Main"]; }
            set { this["WindowLocation_Main"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public Size? WindowSize_Main
        {
            get { return (Size?)this["WindowSize_Main"]; }
            set { this["WindowSize_Main"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public Point? WindowLocation_VirtualKeyboard
        {
            get { return (Point?)this["WindowLocation_VirtualKeyboard"]; }
            set { this["WindowLocation_VirtualKeyboard"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? FlashcardTypes_Single
        {
            get { return (bool?)this["FlashcardTypes_Single"]; }
            set { this["FlashcardTypes_Single"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? FlashcardTypes_Interval
        {
            get { return (bool?)this["FlashcardTypes_Interval"]; }
            set { this["FlashcardTypes_Interval"] = value; }
        }

        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public bool? FlashcardTypes_Triad
        {
            get { return (bool?)this["FlashcardTypes_Triad"]; }
            set { this["FlashcardTypes_Triad"] = value; }
        }

        [UserScopedSetting]
        [DefaultSettingValue("5")]
        public int MaxFlashcardTime
        {
            get { return (int)this["MaxFlashcardTime"]; }
            set { this["MaxFlashcardTime"] = value; }
        }

        #endregion

        #region MIDI Devices

        public OutputDevice GetOutputDevice()
        {
            if (_outputDevice == null && this.MIDIOutputDeviceNumber.HasValue)
            {                
                try
                {
                    _outputDevice = new OutputDevice(this.MIDIOutputDeviceNumber.Value);
                }
                catch (OutputDeviceException) // the input device is unavailable (in use or not attached.)
                {
                    _outputDevice = null;
                }
            }
            return _outputDevice;
        }
        OutputDevice _outputDevice;

        internal InputDevice GetInputDevice()
        {
            if (_inputDevice == null && this.MIDIInputDeviceNumber.HasValue)
            {
                try
                {
                    _inputDevice = new InputDevice(this.MIDIInputDeviceNumber.Value);
                }
                catch (InputDeviceException) // the input device is unavailable (in use or not attached.)
                {
                    _inputDevice = null;
                }
            }
            return _inputDevice;
        }
        InputDevice _inputDevice;

        #endregion
    }
}
