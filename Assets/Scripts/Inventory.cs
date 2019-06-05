using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemInfo
{
    public eItem Type;
    public GameObject ItemTemplate;
    [Range(0, 100)] public int Count;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] RectTransform m_InventoryUI = null;
    [SerializeField] RectTransform m_InventoryContent = null;
    [SerializeField] AnimationCurve m_OpenCurve = null;
    [SerializeField] AnimationCurve m_CloseCurve = null;
    [SerializeField] [Range(0.0f, 5.0f)] float m_InventorySpeed = 2.0f;
    [SerializeField] [Range(0.0f, 2.0f)] float m_InventoryOffset = 0.35f;
    [SerializeField] [Range(10, 200)] int m_InventoryHeight = 100;
    [SerializeField] List<ItemInfo> m_ItemInfo = null;

    public GameObject Owner { get; private set; }

    public void Init(GameObject owner)
    {
        Owner = owner;

        // TODO: CreateItems
        foreach (ItemInfo item in m_ItemInfo)
        {
            GameObject go = Instantiate(item.ItemTemplate, m_InventoryContent);
            InventoryItem i = go.GetComponent<InventoryItem>();
            i.Init(this, item.Count, item.Type);
            TouchManager.Instance.RegisterTouchable(i);
        }
        Close();
    }

    private void LateUpdate()
    {
        m_InventoryUI.transform.position = Enko.Instance.transform.position + Vector3.up * m_InventoryOffset;
    }

    public void Open()
    {
        int cH = (int)m_InventoryUI.sizeDelta.y;
        StopAllCoroutines();
        StartCoroutine(ScaleHeight(cH, m_InventoryHeight, m_InventorySpeed, true));
    }

    public void Close()
    {
        int cH = (int)m_InventoryUI.sizeDelta.y;
        StopAllCoroutines();
        StartCoroutine(ScaleHeight(cH, 0, m_InventorySpeed, false));
    }

    public void RemoveItem(eItem type)
    {
        InventoryItem[] children = m_InventoryContent.GetComponentsInChildren<InventoryItem>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].Type == type)
            {
                // Crude, need animation
                Destroy(children[i].gameObject);
                break;
            }
        }
    }

    void SetInventoryHeight(int height)
    {
        Vector2 size = m_InventoryUI.sizeDelta;
        size.y = height;
        m_InventoryUI.sizeDelta = size;
    }

    IEnumerator ScaleHeight(int start, int end, float speed, bool open)
    {
        if (open) m_InventoryUI.gameObject.SetActive(true);

        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime * speed)
        {
            float v = i;
            AnimationCurve curve = open ? m_OpenCurve : m_CloseCurve;
            float t = curve.Evaluate(v);
            int h = (int)Mathf.LerpUnclamped(start, end, t);
            SetInventoryHeight(h);
            yield return null;
        }

        SetInventoryHeight(end);
        if (!open) m_InventoryUI.gameObject.SetActive(false);
    }
}
