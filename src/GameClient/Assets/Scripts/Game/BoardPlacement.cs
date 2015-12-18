using UnityEngine;

public static class BoardPlacement
{
    public static int BlockSize = 30;

    public static void SetPosition(Transform transform, int x, int y)
    {
        transform.localPosition = new Vector3(x * BlockSize, y * BlockSize, 0);
    }
}
