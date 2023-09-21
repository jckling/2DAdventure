using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;
    public AudioSource FXSource;
    public AudioSource BGMSource;

    public AudioMixer audioMixer;
    public FloatEventSO volumeEvent;
    public VoidEventSO pauseEvent;
    public FloatEventSO syncVolumeEvent;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        volumeEvent.OnEventRaised += OnVolumeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeEvent.OnEventRaised -= OnVolumeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnVolumeEvent(float value)
    {
        audioMixer.SetFloat("MasterVolume", value * 100 - 80);
    }

    private void OnPauseEvent()
    {
        audioMixer.GetFloat("MasterVolume", out var volume);
        syncVolumeEvent.RaiseEvent(volume);
    }
}