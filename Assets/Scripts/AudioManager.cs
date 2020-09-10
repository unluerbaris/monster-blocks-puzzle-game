using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip[] musicClips;
    public AudioClip[] winSFXs;
    public AudioClip[] loseSFXs;
    public AudioClip[] bonusSFXs;

    [Range(0, 1)]
    public float musicVolume = 0.5f;
    [SerializeField] bool musicLoop = true;

    [Range(0, 1)]
    public float sfxVolume = 1.0f;

    [SerializeField] float lowPitch = 0.95f;
    [SerializeField] float highPitch = 1.05f;

    private void Start()
    {
        PlayRandomMusic();
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector2 position, float volume = 1f, bool isLooping = false)
    {
        if (clip != null)
        {
            GameObject gObject = new GameObject("SoundFX" + clip.name);
            gObject.transform.position = position;

            AudioSource source = gObject.AddComponent<AudioSource>();
            source.clip = clip;

            if (isLooping)
            {
                source.loop = musicLoop;
            }

            float randomPitch = Random.Range(lowPitch, highPitch);
            source.pitch = randomPitch;
            source.volume = volume;
            source.Play();

            if (!isLooping)
            {
                Destroy(gObject, clip.length);
            }
            return source;
        }
        return null;
    }

    public AudioSource PlayRandomClip(AudioClip[] clips, Vector2 position, float volume = 1f, bool isLooping = false)
    {
        if (clips != null)
        {
            if (clips.Length != 0)
            {
                int randomIndex = Random.Range(0, clips.Length);

                if (clips[randomIndex] != null)
                {
                    AudioSource source = PlayClipAtPoint(clips[randomIndex], position, volume, isLooping);
                    return source;
                }
            }
        }
        return null;
    }

    public void PlayRandomMusic()
    {
        PlayRandomClip(musicClips, Vector2.zero, musicVolume, true);
    }

    public void PlayRandomWinSFX()
    {
        PlayRandomClip(winSFXs, Vector2.zero, sfxVolume);
    }

    public void PlayRandomLoseSFX()
    {
        PlayRandomClip(loseSFXs, Vector2.zero, sfxVolume);
    }

    public void PlayRandomBonusSFX()
    {
        PlayRandomClip(bonusSFXs, Vector2.zero, sfxVolume);
    }
}
