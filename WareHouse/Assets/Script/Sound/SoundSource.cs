using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;

    private float stopTime;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
    }

    private void OnEnable()
    {
        SoundManager.Instance.Add(this);
        AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
    }

    private void OnDisable()
    {
        SoundManager.Instance.Remove(this);
        AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
    }

    public void Play()
    {
        source.time = stopTime;
        source.Play();

        Debug.Log($"Source.Time: {source.time}, stopTime: {stopTime}");
    }

    public void Stop()
    {
        Debug.Log($"Time: {source.time}");
        stopTime = source.time;
        source.Stop();
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
