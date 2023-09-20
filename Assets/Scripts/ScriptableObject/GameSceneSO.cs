using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Event/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public AssetReference sceneReference;
    public SceneType sceneType;
}