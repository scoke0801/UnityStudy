using UnityEngine;

/// <summary>
/// 루프, 페이드 관련 속성, 오디오 클립 속성들.
/// </summary>
public class SoundClip
{
    public SoundPlayType playType = SoundPlayType.None;
    public string clipName = string.Empty;
    public string clipPath = string.Empty;
    public float maxVolume = 1.0f;
    public bool hasLoop = false;
    public float[] checkTime = new float[0];
    public float[] setTime = new float[0];
    public int readId = 0;

    private AudioClip clip = null;
    public int currentLoop = 0;
    public float pitch = 1.0f;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    public float minDistance = 10000.0f;
    public float maxDistance = 50000.0f;
    public float spatialblend = 1.0f;

    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;
    public Interpolate.Function interpolateFunc;
    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public SoundClip() { }
    public SoundClip( string clipPath, string clipName)
    {
        this.clipPath = clipPath;
        this.clipName = clipName;
    }

    public void PreLaod()
    {
        if(clip == null)
        {
            string fullPath = clipPath + clipName;
            clip = ResourceManager.Load(fullPath) as AudioClip;
        }
    }

    public void AddLoop()
    {
        checkTime = ArrayHelper.Add(0.0f, checkTime);
        setTime = ArrayHelper.Add(0.0f, setTime);
    }

    public void RemoveLoop(int index)
    {
        checkTime = ArrayHelper.Remove(index, checkTime);
        setTime = ArrayHelper.Remove(index, setTime);
    }

    public AudioClip GetClip()
    {
        if (clip == null)
        {
            PreLaod();
        }

        if( clip == null && clipName != string.Empty)
        {
            Debug.LogError($"Can not load audio clip resource {clipName}");
            return null;
        }

        return clip;
    }

    public void ReleaseClip()
    {
        if (clip != null)
        {
            clip = null;
        }
    }

    public bool GetHasLoop()
    {
        return checkTime.Length > 0;
    }

    public void NextLoop()
    {
        this.currentLoop++;
        if(this.currentLoop >= this.checkTime.Length)
        {
            currentLoop = 0;
        }
    }

    public void CheckLoop(AudioSource source)
    {
        if(checkTime.Length > 0 && source.time >= checkTime[currentLoop])
        {
            source.time = setTime[currentLoop];
            NextLoop();
        }
    }

    public void FadeIn(float time, Interpolate.EaseType easeType)
    {
        isFadeOut = false;
        fadeTime1 = 0.0f;
        fadeTime2 = time;
        interpolateFunc = Interpolate.Ease(easeType);
        isFadeIn = true;
    }

    public void FadeOut(float time, Interpolate.EaseType easeType)
    {
        isFadeIn = false;
        fadeTime1 = 0.0f;
        fadeTime2 = time;
        interpolateFunc = Interpolate.Ease(easeType);
        isFadeOut = true;
    }


    public void DoFade(float time, AudioSource audio)
    {
        if (isFadeIn)
        {
            fadeTime1 += time;
            audio.volume = Interpolate.Ease(interpolateFunc, 0, maxVolume, fadeTime1, fadeTime2);

            if(fadeTime1 >= fadeTime2)
            {
                isFadeIn = false;
            }
        }
        else if (isFadeOut)
        {
            fadeTime1 += time;
            audio.volume = Interpolate.Ease(interpolateFunc, maxVolume, -maxVolume, fadeTime1, fadeTime2);

            if (fadeTime1 >= fadeTime2)
            {
                isFadeOut = false;
                audio.Stop();
            }
        }
    }
}
