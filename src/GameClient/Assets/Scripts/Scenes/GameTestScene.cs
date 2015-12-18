using TypeAlias;
using UnityEngine;

public class GameTestScene : MonoBehaviour
{
    public SnakeControlPad SnakeControlPad;
    public Transform GameEntityRoot;

    protected void Start()
    {
        ClientEntityFactory.Default.RootTransform = GameEntityRoot;

        var typeTable = new TypeAliasTable();
        EntityNetworkManager.TypeTable = typeTable;
        EntityNetworkManager.ProtobufTypeModel = new DomainProtobufSerializer();

        ApplicationComponent.TryInit();
        UiManager.Initialize();
    }
}
