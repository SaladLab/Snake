using System;
using Domain;
using UnityEngine;

public class ClientFruit : FruitClientBase, IFruitClientHandler
{
    public Tuple<int, int> Pos { get; private set; }

    public override void OnSnapshot(FruitSnapshot snapshot)
    {
        Pos = snapshot.Pos;
        BoardPlacement.SetPosition(GetComponent<Transform>(), snapshot.Pos.Item1, snapshot.Pos.Item2);
    }
}
