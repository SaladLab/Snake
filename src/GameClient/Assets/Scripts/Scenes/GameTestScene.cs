using UnityEngine;

public class GameTestScene : MonoBehaviour
{
    public SnakeControlPad SnakeControlPad;
    public Transform GameEntityRoot;

    protected void Start()
    {
        ClientEntityFactory.Default.RootTransform = GameEntityRoot;

        ApplicationComponent.TryInit();
        UiManager.Initialize();
    }
}
