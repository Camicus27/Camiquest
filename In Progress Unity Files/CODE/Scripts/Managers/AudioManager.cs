using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private Dictionary<string, Sound> soundsSet;
    private Dictionary<string, Sound> music;
    public Sound currentSong;

    void Awake()
    {
        soundsSet = new Dictionary<string, Sound>();
        music = new Dictionary<string, Sound>();

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            soundsSet.Add(sound.name, sound);
            if (sound.name.Contains("Music"))
                music.Add(sound.name, sound);
        }
    }

    private void Start()
    {
        //Play("OpeningTheme");
    }

    public string GetCurrentlyPlayingSong()
    {
        return currentSong.name;
    }

    public bool IsSoundPlaying(string name)
    {
        return soundsSet[name].source.isPlaying;
    }

    public void Play(string name)
    {
        Sound s;

        if (name.Contains("_"))
        {
            name += Random.Range(0, 3);
            s = soundsSet[name];
            if (!s.source.isPlaying)
                s.source.Play();
            return;
        }

        s = soundsSet[name];

        if (music.ContainsKey(name) && name != "BattleMusic")
            currentSong = s;

        if (s.source.isPlaying)
            s.source.Stop();
        s.source.Play();
    }

    public void PlaySlowly(string name, float fadeInTime)
    {
        Sound sound = soundsSet[name];
        StartCoroutine(Fade(sound, fadeInTime, false, sound.source.volume));
        if (music.ContainsKey(name) && name != "BattleMusic")
            currentSong = sound;
    }

    public void Stop(string name)
    {
        Sound sound = soundsSet[name];
        sound.source.Stop();
    }

    public void StopSlowly(string name, float fadeOutTime)
    {
        Sound sound = soundsSet[name];
        StartCoroutine(Fade(sound, fadeOutTime, true, sound.source.volume));
    }

    private IEnumerator Fade(Sound sound, float fadeTime, bool fadeOut, float ogVolume)
    {
        float alpha;
        if (fadeOut)
        {
            alpha = ogVolume;
            while (alpha > 0)
            {
                alpha -= 0.005f / fadeTime;
                sound.source.volume = alpha;
                yield return null;
            }
            sound.source.Stop();
            sound.source.volume = ogVolume;
        }
        else
        {
            sound.source.volume = 0f;
            sound.source.Play();
            alpha = 0f;
            while (alpha <= ogVolume)
            {
                alpha += 0.005f / fadeTime;
                sound.source.volume = alpha;
                yield return null;
            }
            sound.source.volume = ogVolume;
        }
    }

    public void StopAll()
    {
        foreach (Sound sound in sounds)
            sound.source.Stop();
    }
}
