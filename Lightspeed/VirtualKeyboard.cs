using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sanford.Multimedia.Midi;
using System.Timers;

namespace Lightspeed
{
    /// <summary>
    /// This input device is an alternative to a windows MIDI device which can work with the sightreading game.
    /// Pass it in to the game constructor and control it from any keyboard-like interface - on-screen, touchscreen, 
    /// audio-in with real time pitch detection, whatever man!
    /// 
    /// Also plays notes through the output device when they're hit, so the user can actually play a VirtualKeyboard.
    /// </summary>
    public class VirtualKeyboard
    {        
        public event EventHandler<ChannelMessageEventArgs> ChannelMessageReceived;
        
        OutputDevice _device;

        public VirtualKeyboard()
        {            
            _device = UserSettings.Instance.GetOutputDevice();
            UserSettings.Instance.OutputDeviceChanged += new EventHandler(UserSettings_OutputDeviceChanged);
            this.ChannelMessageReceived += new EventHandler<ChannelMessageEventArgs>(VirtualKeyboard_ChannelMessageReceived);
        }

        /// <summary>
        /// The MIDI output device has changed.  Start using that instead.
        /// </summary>
        void UserSettings_OutputDeviceChanged(object sender, EventArgs e)
        {
            _device = UserSettings.Instance.GetOutputDevice();
        }

        /// <summary>
        /// We got a ChannelMessage.  If there's an OutputDevice, play the note!
        /// </summary>
        void VirtualKeyboard_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            if(_device != null)
                _device.Send(e.Message);
        }

        public void PlayNote(int noteNumber)
        {
            var b = new ChannelMessageBuilder();
            b.Command = ChannelCommand.NoteOn;
            b.Data1 = MIDIUtils.ConvertNoteNumberToMIDINoteNumber(noteNumber);
            b.Data2 = 105;
            b.MidiChannel = 0;
            b.Build();
            if (ChannelMessageReceived != null)
                ChannelMessageReceived(this, new ChannelMessageEventArgs(b.Result));
        }

        public void ReleaseNote(int noteNumber)
        {
            var b = new ChannelMessageBuilder();
            b.Command = ChannelCommand.NoteOff;
            b.Data1 = MIDIUtils.ConvertNoteNumberToMIDINoteNumber(noteNumber);            
            b.MidiChannel = 0;
            b.Build();
            if (ChannelMessageReceived != null)
                ChannelMessageReceived(this, new ChannelMessageEventArgs(b.Result));
        }
    }
}
