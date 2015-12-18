using Domain;
using UnityEngine;

public class ClientFruit : FruitClientBase, IFruitClientHandler
{
    public override void OnSnapshot(FruitSnapshot snapshot)
    {
        BoardPlacement.SetPosition(GetComponent<Transform>(), snapshot.Pos.Item1, snapshot.Pos.Item2);
    }
}
