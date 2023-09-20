using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractive
{
    public SceneLoadEventSO loadEventSo;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    public void TriggerAction()
    {
        loadEventSo.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}