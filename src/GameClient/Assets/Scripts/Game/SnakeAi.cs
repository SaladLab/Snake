using System;
using Domain;

public static class SnakeAi
{
    public static Tuple<int, int> Think(int posX, int posY, int orientX, int orientY)
    {
        var ox = 0;
        var oy = 0;

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                ox = orientX;
                oy = orientY;
            }
            else if (i == 1)
            {
                ox = -orientY;
                oy = orientX;
            }
            else
            {
                ox = orientY;
                oy = -orientX;
            }

            var nextX = posX + ox;
            var nextY = posY + oy;

            if (nextX < 0 || nextX >= Rule.BoardWidth)
                continue;
            if (nextY < 0 || nextY >= Rule.BoardHeight)
                continue;

            break;
        }

        return Tuple.Create(ox, oy);
    }
}
