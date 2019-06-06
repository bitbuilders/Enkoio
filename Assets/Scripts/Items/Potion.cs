using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    public override void Use(Vector2 force)
    {
        base.Use(force);
    }

    public override void Break()
    {
        // TODO: Send particles to Enko


        base.Break();
    }
}
