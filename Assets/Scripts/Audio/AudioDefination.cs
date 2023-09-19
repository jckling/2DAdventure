using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayAudioEventSO PlayAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable) PlayAudioClip();
    }

    private void PlayAudioClip()
    {
        PlayAudioEvent.OnEventRaised(audioClip);
    }
}