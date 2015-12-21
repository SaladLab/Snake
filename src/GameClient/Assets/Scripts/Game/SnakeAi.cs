using System;
using System.Diagnostics;
using System.Linq;
using Domain;
using EntityNetwork;

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

    private static readonly int[] _forward = { 0, 1, 2 };
    private static readonly int[] _turnLeft = { 1, 2, 0 };
    private static readonly int[] _turnRight = { 2, 1, 0 };

    private static int[] TowardFruit(IClientZone zone, int posX, int posY, int orientX, int orientY)
    {
        var fruit = zone.GetEntity<ClientFruit>();
        if (fruit != null)
        {
            var dx = fruit.Pos.Item1 - posX;
            var dy = fruit.Pos.Item2 - posY;

            // when snake goes in the opposite direction horizontally
            if (dx * orientX < 0)
            {
                return dx * dy > 0 ? _turnRight : _turnLeft;
            }
            // when snake goes in the opposite direction verically
            if (dy * orientY < 0)
            {
                return dx * dy > 0 ? _turnLeft : _turnRight;
            }
            // when snake reaches X but still move in horizontal direction
            if ((dx == 0 && dy != 0) && orientX != 0)
            {
                return dy * orientX > 1 ? _turnLeft : _turnRight;
            }
            // when snake reaches Y but still move in vertical direction
            if ((dx != 0 && dy == 0) && orientY != 0)
            {
                return dx * orientY > 1 ? _turnRight : _turnLeft;
            }
        }

        return _forward;
    }
}
