using System;
using System.Linq;
using Domain;
using EntityNetwork;
using UnityEngine;

public static class SnakeAi
{
    public static Tuple<int, int> Think(IClientZone zone, int posX, int posY, int orientX, int orientY)
    {
        var orients = TowardFruit(zone, posX, posY, orientX, orientY);
        var snakes = zone.GetEntities<ClientSnake>().ToList();

        var ox = 0;
        var oy = 0;

        foreach (var orient in orients)
        {
            if (orient == 0)
            {
                ox = orientX;
                oy = orientY;
            }
            else if (orient == 1)
            {
                ox = -orientY;
                oy = orientX;
            }
            else
            {
                ox = orientY;
                oy = -orientX;
            }

            // check if can go there

            var nextX = posX + ox;
            var nextY = posY + oy;

            if (nextX < 0 || nextX >= Rule.BoardWidth)
                continue;
            if (nextY < 0 || nextY >= Rule.BoardHeight)
                continue;

            if (snakes.Any(s => s.IsOnPart(nextX, nextY)))
                continue;

            break;
        }

        return Tuple.Create(ox, oy);
    }

    private static readonly int[][] _forwards = { new[] { 0, 1, 2 }, new[] { 0, 2, 1 } };
    private static readonly int[][] _turnLefts = { new[] { 1, 2, 0 }, new[] { 1, 0, 2 } };
    private static readonly int[][] _turnRights = { new[] { 2, 1, 0 }, new[] { 2, 0, 1 } };
    private static bool _verbose = false;

    private static int[] TowardFruit(IClientZone zone, int posX, int posY, int orientX, int orientY)
    {
        var fruit = zone.GetEntity<ClientFruit>();
        if (fruit != null)
        {
            var dx = fruit.Pos.Item1 - posX;
            var dy = fruit.Pos.Item2 - posY;

            if (_verbose)
            {
                Debug.LogFormat("TowardFruit.Condition (x={0}, y={1}, ox={2}, oy={3}, dx={4}, dy={5})",
                                posX, posY, orientX, orientY, dx, dy);
            }

            // when snake goes in the opposite direction horizontally
            if (dx * orientX < 0)
            {
                return dx * dy > 0 ? _turnRights[Get0or1()] : _turnLefts[Get0or1()];
            }
            // when snake goes in the opposite direction verically
            if (dy * orientY < 0)
            {
                return dx * dy > 0 ? _turnLefts[Get0or1()] : _turnRights[Get0or1()];
            }
            // when snake reaches X but still move in horizontal direction
            if ((dx == 0 && dy != 0) && orientX != 0)
            {
                return dy * orientX > 0 ? _turnLefts[Get0or1()] : _turnRights[Get0or1()];
            }
            // when snake reaches Y but still move in vertical direction
            if ((dx != 0 && dy == 0) && orientY != 0)
            {
                return dx * orientY > 0 ? _turnRights[Get0or1()] : _turnLefts[Get0or1()];
            }
        }

        return _forwards[Get0or1()];
    }

    private static int Get0or1()
    {
        return UnityEngine.Random.Range(0, 2);
    }
}
