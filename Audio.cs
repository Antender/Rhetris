using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;

namespace Rhetris
{
    class Audio
    {
        Rhetris _parent;
        private SoundPlayer soundPlayer;
        public Audio(Rhetris parent)
        {
            _parent = parent;
            soundPlayer = new SoundPlayer(new FileStream("Content\\Audio\\Beep.wav", FileMode.Open));
        }

        public void playBeat()
        {
            soundPlayer.Play();
        }
    }
}
