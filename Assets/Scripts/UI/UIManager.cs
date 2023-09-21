using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public GameObject mobileTouch;
    public Button settingsBtn;
    public GameObject pausePanel;

    public VoidEventSO pauseEvent;
    public FloatEventSO syncVolumeEvent;
    public Slider volumeSlider;

    private void Awake()
    {
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif

        settingsBtn.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadSceneEventSo.LoadRequestEvent += OnUnLoadRequestEvent;
        loadGameEventSo.OnEventRaised += OnLoadGameEvent;
        gameOverEventSo.OnEventRaised += OnGameOverEvent;
        backToMenuEventSo.OnEventRaised += OnLoadGameEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadSceneEventSo.LoadRequestEvent -= OnUnLoadRequestEvent;
        loadGameEventSo.OnEventRaised -= OnLoadGameEvent;
        gameOverEventSo.OnEventRaised -= OnGameOverEvent;
        backToMenuEventSo.OnEventRaised -= OnLoadGameEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void TogglePausePanel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnSyncVolumeEvent(float volume)
    {
        volumeSlider.value = (volume + 80) / 100;
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