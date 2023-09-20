using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour, ISaveable
{
    public SceneLoadEventSO loadEventSo;
    public SceneLoadEventSO unloadEventSo;

    public GameSceneSO firstLoadScene;
    private GameSceneSO currentLoadScene;
    private GameSceneSO sceneToGo;
    private Vector3 posToGo;
    private bool fade;
    public float fadeDuration;

    public Transform playerTrans;
    public Vector3 initialPosition;
    private bool isLoading;

    public GameSceneSO menuScene;
    public Vector3 menuPosition;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    public VoidEventSO afterSceneLoadedEvent;
    [Header("Broadcast")] public FadeEventSO fadeEventSo;

    private void Start()
    {
        loadEventSo.RaiseLoadRequestEvent(menuScene, menuPosition, true);
    }

    private void OnEnable()
    {
        loadEventSo.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        loadEventSo.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void NewGame()
    {
        sceneToGo = firstLoadScene;
        loadEventSo.RaiseLoadRequestEvent(sceneToGo, initialPosition, true);
    }

    private void OnBackToMenuEvent()
    {
        sceneToGo = menuScene;
        loadEventSo.RaiseLoadRequestEvent(sceneToGo, menuPosition, true);
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fade)
    {
        if (isLoading) return;
        isLoading = true;

        this.sceneToGo = sceneToGo;
        this.posToGo = posToGo;
        this.fade = fade;

        if (currentLoadScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    IEnumerator UnloadPreviousScene()
    {
        if (fade)
        {
            fadeEventSo.FadeIn(fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);
        unloadEventSo.RaiseLoadRequestEvent(sceneToGo, posToGo, true);
        yield return currentLoadScene.sceneReference.UnLoadScene();

        playerTrans.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToGo.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadScene = sceneToGo;
        playerTrans.position = posToGo;
        playerTrans.gameObject.SetActive(true);
        if (fade)
        {
            fadeEventSo.FadeOut(fadeDuration);
        }

        isLoading = false;
        if (currentLoadScene.sceneType == SceneType.Location)
        {
            afterSceneLoadedEvent.RaiseEvent();
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            posToGo = data.characterPosDict[playerID].ToVector3();
            sceneToGo = data.GetSavedScene();
            OnLoadRequestEvent(sceneToGo, posToGo, true);
        }
    }
}