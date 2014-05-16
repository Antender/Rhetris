using System;
using Microsoft.Xna.Framework.Audio;

namespace Rhetris
{
    class Audio
    {
        Cue[] cues = new Cue[11];
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Rhetris _parent;
        public Audio(Rhetris parent)
        {
            _parent = parent;
            audioEngine = new AudioEngine("Content\\Beeps.xgs");
            waveBank = new WaveBank(audioEngine,"Content\\Beeps.xwb");
            soundBank = new SoundBank(audioEngine,"Content\\Beeps.xsb");
            for (var i=0 ; i<=10 ; i++)
            {
                cues[i] = soundBank.GetCue(i.ToString());
            }
        }
        public void playBeep()
        {
            cues[_parent.Rnd(11)].Play();
        }
    }
}
