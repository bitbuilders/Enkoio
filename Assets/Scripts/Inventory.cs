using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] int m_PotionCount = 10;

    public GameObject Owner { get; private set; }

    public void Init(GameObject owner)
    {
        Owner = owner;

        // TODO: CreateItems
    }

    public void Open()
    {

    }

    public void Close()
    {

    }
}
