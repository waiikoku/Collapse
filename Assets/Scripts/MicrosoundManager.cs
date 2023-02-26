using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrosoundManager : MonoBehaviour
{
    public enum SoundType
    {
        Music,
        Effect,
        Voice
    }
    public SoundType Type;
    [SerializeField] private AudioSource source;
    [SerializeField] private Sound soundEffect;

    private void Start()
    {
        source.clip = soundEffect.Audio;
        source.Play();
        if (Type == SoundType.Effect)
        {
            SoundManager.Instance.OnSfxVolumeChange += ControlVolume;
            ControlVolume(SoundManager.Instance.SfxVolume);
        }

    }
    private void OnDestroy()
    {
        if (Type == SoundType.Effect)
        {
            SoundManager.Instance.OnSfxVolumeChange -= ControlVolume;
        }
    }

    private void ControlVolume(float volume)
    {
        source.volume = volume * soundEffect.BaseVolume;
    }
}
