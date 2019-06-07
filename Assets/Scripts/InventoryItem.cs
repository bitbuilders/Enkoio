using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : Touchable
{
    [SerializeField] GameObject m_ItemTemplate = null;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MaxDistance = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MinDistance = 0.2f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_ItemSize = 0.2f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_ForceAmplifier = 500.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_MaxForce = 250.0f;

    public Inventory Inventory { get; set; }
    public Vector2 Force { get; private set; }
    public eItem Type { get; set; }
    public int Count { get; set; }

    GameObject m_Item;
    Vector2 m_LastPosition;
    bool m_Mouse;

    public void Init(Inventory inventory, int count, eItem type)
    {
        Inventory = inventory;
        Count = count;
        Type = type;
    }

    public void Click()
    {
        CreateItem();
        m_Mouse = true;
    }

    public override void Touch()
    {
        CreateItem();
        m_Mouse = false;
    }

    public override void Drop()
    {
        if (GreaterThanMin()) DropItem();
        else if (m_Item) Destroy(m_Item.gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mouseWorld - (Vector2)transform.position;
            if (dir.sqrMagnitude < m_ItemSize * m_ItemSize)
            {
                Click();
            }
        }

        if (m_Item)
        {
            Vector3 pos = m_Mouse ? Input.mousePosition : (Vector3)TouchManager.Instance.TouchPosition;
            Vector3 wPos = Camera.main.ScreenToWorldPoint(pos);
            wPos.z = 0.0f;
            m_Item.transform.position = wPos;
            Force = ((Vector2)m_Item.transform.position - m_LastPosition) * m_ForceAmplifier;
            m_LastPosition = m_Item.transform.position;
            
            Vector2 dir = transform.position - m_Item.transform.position;
            if (dir.sqrMagnitude > m_MaxDistance * m_MaxDistance || (m_Mouse && Input.GetMouseButtonUp(0)))
            {
                if (GreaterThanMin()) DropItem();
                else if (m_Item) Destroy(m_Item.gameObject);
            }
        }
    }

    public void Use()
    {
        // TODO: Use inventory's owner to get position for animation

        Vector2 force = Force.magnitude > m_MaxForce ? Force.normalized * m_MaxForce : Force;
        m_Item.GetComponent<Item>().Use(force);
    }

    void CreateItem()
    {
        m_Item = Instantiate(m_ItemTemplate);
    }

    void DropItem()
    {
        Use();
        m_Item = null;
    }

    bool GreaterThanMin()
    {
        if (!m_Item) return false;

        Vector2 dir = transform.position - m_Item.transform.position;
        return dir.sqrMagnitude > m_MinDistance * m_MinDistance;
    }
}
