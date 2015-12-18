using System;

namespace Domain
{
    public class ServerFruit : FruitServerBase, IFruitServerHandler
    {
        public Tuple<int, int> Pos { get; private set; }

        public override void OnSpawn(object param)
        {
            Pos = (Tuple<int, int>)param;
        }

        public override void OnDespawn()
        {
            Zone.GetEntity<ServerZoneController>().OnFruitDespawn(this);
        }

        public override FruitSnapshot OnSnapshot()
        {
            return new FruitSnapshot { Pos = Pos };
        }
    }
}
