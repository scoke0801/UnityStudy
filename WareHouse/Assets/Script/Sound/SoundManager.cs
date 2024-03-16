using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    List<SoundSource> sounds = new List<SoundSource>();
        
    public void OnPlayButtonClick()
    {
        foreach(var sound in sounds)
        {
            sound.Play();
        }
    }

    public void OnStopButtonClick()
    {
        foreach(var sound in sounds)
        {
            sound.Stop();
        }
    }

    public void Add(SoundSource source)
    {
        sounds.Add(source);
    }

    public void Remove(SoundSource source)
    {
        sounds.Remove(source);
    }
}
