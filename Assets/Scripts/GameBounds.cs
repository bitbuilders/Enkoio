using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameBounds
{
    public static Vector2 OutOfBounds(Vector2 point, float radius, out Vector2 newPosition)
    {
        Vector2 outNorm = Vector2.zero;
        newPosition = Camera.main.WorldToScreenPoint(point);

        Vector2 p = Camera.main.WorldToScreenPoint(point);
        int sW = Screen.width;
        int sH = Screen.height;

        if (p.x - radius < 0)
        {
            outNorm = Vector2.left;
            newPosition.x = 0 + radius;
        }
        else if (p.x + radius > sW)
        {
            outNorm = Vector2.right;
            newPosition.x = sW - radius;
        }
        else if (p.y - radius < 0)
        {
            outNorm = Vector2.down;
            newPosition.y = 0 + radius;
        }
        else if (p.y + radius > sH)
        {
            outNorm = Vector2.up;
            newPosition.y = sH - radius;
        }

        newPosition = Camera.main.ScreenToWorldPoint((Vector3)newPosition + Vector3.forward * 10.0f);

        return outNorm;
    }
}
