using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanford.Multimedia.Midi;

namespace Lightspeed
{
    /// <summary>
    /// Handy methods for MIDI.
    /// </summary>
    public static class MIDIUtils
    {
        /// <summary>
        /// Get the MIDI input devices in the system by number and name.
        /// </summary>
        public static Tuple<int, string>[] GetMIDIInputDevices()
        {
            var devices = new Tuple<int, string>[InputDevice.DeviceCount];
            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                var device = InputDevice.GetDeviceCapabilities(i);
                devices[i] = Tuple.Create<int, string>(i, device.name);
            }
            return devices;
        }

        /// <summary>
        /// Get the MIDI output devices in the system by number and name.
        /// </summary>
        public static Tuple<int, string>[] GetMIDIOutputDevices()
        {
            var devices = new Tuple<int, string>[OutputDevice.DeviceCount];
            for (int i = 0; i < OutputDevice.DeviceCount; i++)
            {
                var device = OutputDevice.GetDeviceCapabilities(i);
                devices[i] = Tuple.Create<int, string>(i, device.name);
            }
            return devices;
        }

        /// <summary>
        /// Convert a MIDI note number into a Lightspeed (tuner's notation) note number.
        /// </summary>
        public static int ConvertMIDINoteNumberToNoteNumber(int midiNoteNumber)
        {
            return midiNoteNumber - 20;
        }

        /// <summary>
        /// Turn a Lightspeed note's number (tuner's notation) into a MIDI note number.
        /// </summary>
        public static int ConvertNoteNumberToMIDINoteNumber(int noteNumber)
        {
            return noteNumber + 20;
        }
    }
}
