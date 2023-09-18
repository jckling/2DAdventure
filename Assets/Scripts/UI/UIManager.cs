using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStarBar playerStarBar;
    public CharacterEventSO healthEvent;

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStarBar.OnHealthChange(percentage);
        playerStarBar.OnPowerChange(character);
    }
}