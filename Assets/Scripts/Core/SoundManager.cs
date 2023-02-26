using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Range(0.0f,1.0f)]
    public float MusicVolume = 0.5f;
    private float musicAddition = 1f;
    [Range(0.0f,1.0f)]
    public float SfxVolume = 0.5f;
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private AudioSource sfxPlayer;

    [SerializeField] private Sound[] musicList;
    [SerializeField] private Sound[] m_sfxList;
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private AudioClip[] sfxList;

    public event Action<float> OnSfxVolumeChange;

    public void ActivateMusic(bool value)
    {
        if(bgmPlayer.clip == null)
        {
            Debug.Log("There is no music to play! (Activate)");
            return;
        }
        if (value)
        {
            bgmPlayer.Play();
            print($"Play BGM({bgmPlayer.clip.name})");
        }
        else
        {
            bgmPlayer.Stop();
            print($"Stop BGM({bgmPlayer.clip.name})");
        }
    }
    public void PauseMusic(bool pause = false)
    {
        if (bgmPlayer.clip == null)
        {
            Debug.Log("There is no music to pause! (Pause)");
            return;
        }
        if (pause)
        {
            if (bgmPlayer.isPlaying == false)
            {
                Debug.Log("Music isn't playing! (Pause)");
                return;
            }
            bgmPlayer.Pause();
            print($"Pause BGM({bgmPlayer.clip.name})");
        }
        else
        {
            bgmPlayer.UnPause();
            print($"UnPause BGM({bgmPlayer.clip.name})");
        }
    }
    public void ChangeMusic(AudioClip music)
    {
        StartCoroutine(TransitionMusic(music, transitionDuration));
    }
    public Sound GetSound(Sound[] array, int id)
    {
        return System.Array.Find(array,audio => audio.Id == id);
    }
    public void ChangeMusic(int id)
    {
        Sound music = GetSound(musicList, id);
        StartCoroutine(TransitionMusic(music.Audio, transitionDuration, music));
        //StartCoroutine(TransitionMusic(playlist[id], transitionDuration));
    }

    private IEnumerator TransitionMusic(AudioClip nextMusic, float duration, Sound sound = null)
    {
        float halfDuration = duration / 2f;
        float timer = 0f;
        float percentage;
        if (bgmPlayer.clip == null)
        {
            bgmPlayer.clip = nextMusic;
        }
        if(bgmPlayer.isPlaying == false)
        {
            bgmPlayer.Play();
            goto FadeIn;
        }
        float lastVolume = bgmPlayer.volume;
        print("Last:" +  lastVolume);

    //FadeOut:
        while (true)
        {
            timer += Time.deltaTime;
            percentage = timer / halfDuration;
            if (percentage >= 1)
            {
                timer = 0;
                percentage = 0;
                bgmPlayer.volume = 0f;
                bgmPlayer.clip = nextMusic;
                bgmPlayer.Play();
                break;
            }
            //print($"{bgmPlayer.volume} = ({MusicVolume} * {musicAddition}) * (1 - {percentage})");
            bgmPlayer.volume = (MusicVolume * musicAddition) * (1 - percentage);
            yield return null;
        }
    FadeIn:
        AdjustMusicBase(sound.BaseVolume);
        while (true)
        {
            timer += Time.deltaTime;
            percentage = timer / halfDuration;
            bgmPlayer.volume = (MusicVolume * musicAddition) * percentage;
            if (percentage >= 1)
            {
                bgmPlayer.volume = MusicVolume * musicAddition;
                break;
            }
            yield return null;
        }
        yield break;
    }
    public void AdjustMusicVolume(float volume)
    {
        MusicVolume = volume;
        bgmPlayer.volume = MusicVolume * musicAddition;
    }
    public void AdjustMusicVolume(float volume, float addition = 1f)
    {
        MusicVolume = volume;
        musicAddition = addition;
        bgmPlayer.volume = MusicVolume * musicAddition;
    }

    public void AdjustMusicBase(float addition)
    {
        musicAddition = addition;
    }

    public void AdjustSfxVolume(float volume)
    {
        SfxVolume = volume;
        sfxPlayer.volume = SfxVolume;
        OnSfxVolumeChange?.Invoke(SfxVolume);
    }
    public void PlaySfx(AudioClip sfx)
    {
        sfxPlayer.PlayOneShot(sfx, SfxVolume);
    }

    public void Play_SFX_AtLocation(Vector3 position,AudioClip sfx,float overrideValue = 1f, float destroyTime = 1f)
    {
        GameObject player = new GameObject();
        player.transform.position = position;
        AudioSource audi = player.AddComponent<AudioSource>();
        audi.volume = sfxPlayer.volume * overrideValue;
        audi.spatialBlend = 1f;
        audi.clip = sfx;
        audi.Play();
        Destroy(player,destroyTime);
    }

    public void PlaySfx(int id)
    {
        sfxPlayer.PlayOneShot(sfxList[id], SfxVolume);
    }

   public void PlaySFXByID(int id)
    {
        PlaySfx(System.Array.Find(m_sfxList, sound => sound.Id == id).Audio);
    }
}
