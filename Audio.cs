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
		private SoundPlayer beatPlayer;
		private SoundPlayer sub_beatPlayer;
		private bool even;

		public Audio(Rhetris parent)
        {
            _parent = parent;
            beatPlayer = new SoundPlayer(new FileStream("Content\\Audio\\beat.wav", FileMode.Open));
			sub_beatPlayer = new SoundPlayer(new FileStream("Content\\Audio\\sub_beat.wav", FileMode.Open));

        }

        public void playBeat()
        {
			if(even == false)
			{
				beatPlayer.Play();
				even = true;
			}
			else
			{
				sub_beatPlayer.Play();
				even = false;
			}

        }
    }
}
