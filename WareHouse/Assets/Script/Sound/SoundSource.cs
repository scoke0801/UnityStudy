using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        Play();
        AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
    }

    private void OnDisable()
    {
        AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
    }

    public void Play()
    {
        source.Play();
    }

    private void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        if(deviceWasChanged)
        {
            source.Stop();
            var cur = clip.length;
            var total = source.time;

            Debug.Log($"cur: {cur}, total: {total}");
        }
    }
}
