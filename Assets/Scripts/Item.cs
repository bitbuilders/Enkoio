using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Inventory Inventory { get; set; }
    public eItem Type { get; set; }
    public int Count { get; set; }

    public void Init(Inventory inventory, int count, eItem type)
    {
        Inventory = inventory;
        Count = count;
        Type = type;
    }

    public void Use()
    {
        // TODO: Use inventory's owner to get position for animation


    }
}
