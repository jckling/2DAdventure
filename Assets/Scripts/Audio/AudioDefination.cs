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

    public void PlayAudioClip()
    {
        PlayAudioEvent.OnEventRaised(audioClip);
    }
}