using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bad : Tile
{
    public override void OnEnter()
    {
        base.OnEnter();
        CombatManager.Instance.NewBattle();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
    }
}
