using System;
using Microsoft.Xna.Framework.Audio;

namespace Rhetris
{
    class Audio
    {
        Cue beat;
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Rhetris _parent;
        public Audio(Rhetris parent)
        {
            _parent = parent;
            audioEngine = new AudioEngine("Content\\Sound.xgs");
            waveBank = new WaveBank(audioEngine,"Content\\Sound.xwb");
            soundBank = new SoundBank(audioEngine,"Content\\Sound.xsb");
            beat = soundBank.GetCue("beep");
        }
        public void playBeat()
        {
            beat.Play();
        }
    }
}
