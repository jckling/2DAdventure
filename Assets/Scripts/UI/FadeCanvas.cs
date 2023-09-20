using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    [Header("Listen")] public FadeEventSO fadeEventSo;
    public Image fadeImage;

    private void OnEnable()
    {
        fadeEventSo.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEventSo.OnEventRaised -= OnFadeEvent;
    }

    private void OnFadeEvent(Color target, float duration, bool fadeIn)
    {
        fadeImage.DOBlendableColor(target, duration);
    }
}