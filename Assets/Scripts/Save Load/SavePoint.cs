using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractive
{
    public VoidEventSO saveGameEvent;

    public SpriteRenderer spriteRenderer;
    public GameObject light;
    public Sprite darkSprite;
    public Sprite lightSprite;
    public bool isDone;

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        light.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            light.SetActive(true);

            saveGameEvent.RaiseEvent();
            gameObject.tag = "Untagged";

        }
    }
}