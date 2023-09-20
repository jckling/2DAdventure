using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public SceneLoadEventSO unloadSceneEventSo;
    public PlayerStarBar playerStarBar;
    public CharacterEventSO healthEvent;
    public VoidEventSO loadGameEventSo;
    public VoidEventSO gameOverEventSo;
    public VoidEventSO backToMenuEventSo;

    public GameObject gameOverPanel;
    public GameObject restartBtn;

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadSceneEventSo.LoadRequestEvent += OnUnLoadRequestEvent;
        loadGameEventSo.OnEventRaised += OnLoadGameEvent;
        gameOverEventSo.OnEventRaised += OnGameOverEvent;
        backToMenuEventSo.OnEventRaised += OnLoadGameEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadSceneEventSo.LoadRequestEvent -= OnUnLoadRequestEvent;
        loadGameEventSo.OnEventRaised -= OnLoadGameEvent;
        gameOverEventSo.OnEventRaised -= OnGameOverEvent;
        backToMenuEventSo.OnEventRaised -= OnLoadGameEvent;
    }

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStarBar.OnHealthChange(percentage);
        playerStarBar.OnPowerChange(character);
    }

    private void OnUnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fade)
    {
        playerStarBar.gameObject.SetActive(sceneToGo.sceneType == SceneType.Location);
    }

    private void OnLoadGameEvent()
    {
        gameOverPanel.SetActive(false);
    }
}