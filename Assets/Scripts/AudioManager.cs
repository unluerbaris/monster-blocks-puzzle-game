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
    [SerializeField] float musicVolume = 0.5f;

    [Range(0, 1)]
    [SerializeField] float sfxVolume = 1.0f;

    [SerializeField] float lowPitch = 0.95f;
    [SerializeField] float highPitch = 1.05f;

    private void Start()
    {
        PlayRandomMusic();
    }

    public AudioSource PlayClipAtPoint(AudioClip clip, Vector2 position, float volume = 1f)
    {
        if (clip != null)
        {
            GameObject gObject = new GameObject("SoundFX" + clip.name);
            gObject.transform.position = position;

            AudioSource source = gObject.AddComponent<AudioSource>();
            source.clip = clip;

            float randomPitch = Random.Range(lowPitch, highPitch);
            source.pitch = randomPitch;

            source.volume = volume;
            Destroy(gObject, clip.length);
            return source;
        }
        return null;
    }

    public AudioSource PlayRandomClip(AudioClip[] clips, Vector2 position, float volume = 1f)
    {
        if (clips != null)
        {
            if (clips.Length != 0)
            {
                int randomIndex = Random.Range(0, clips.Length);

                if (clips[randomIndex] != null)
                {
                    AudioSource source = PlayClipAtPoint(clips[randomIndex], position, volume);
                    return source;
                }
            }
        }
        return null;
    }

    public void PlayRandomMusic()
    {
        PlayRandomClip(musicClips, Vector2.zero, musicVolume);
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
