using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using Lightspeed;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace LightspeedInterface
{
    /// <summary>
    /// Handles the loading and playing of the sound effects for the game.
    /// </summary>
    public class SoundEffects
    {
        Dictionary<Sound, SoundPlayer> _soundPlayers;

        public SoundEffects()
        {
            _soundPlayers = new Dictionary<Sound, SoundPlayer>();
            string filename;
            bool missingFiles = false;
            foreach (Sound s in Enum.GetValues(typeof(Sound)))
            {                
                filename = String.Format(@"Sounds\{0}.wav", s);
                if (File.Exists(filename))
                    _soundPlayers[s] = new SoundPlayer(filename);
                else
                    missingFiles = true;
            }
            if (missingFiles)
                MessageBox.Show("Some sound effects files are missing.  Reinstall Lightspeed to fix this problem.", "Missing Files", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Play(Sound s)
        {
            if(_soundPlayers.ContainsKey(s))
                _soundPlayers[s].Play();
        }
    }
}
