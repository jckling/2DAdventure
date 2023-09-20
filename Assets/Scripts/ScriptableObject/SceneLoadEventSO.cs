using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    public void RaiseLoadRequestEvent(GameSceneSO sceneToGo, Vector3 positionToGo, bool fade)
    {
        LoadRequestEvent?.Invoke(sceneToGo, positionToGo, fade);
    }
}