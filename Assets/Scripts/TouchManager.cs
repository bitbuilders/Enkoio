using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Touchable : MonoBehaviour
{
    public RectTransform RectTransform { get; set; }

    abstract public void Touch();
    abstract public void Drop();
}

public class TouchManager : Singleton<TouchManager>
{
    [SerializeField] List<Touchable> m_Touchables = null;

    public Vector2 TouchPosition { get; private set; }

    private void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            TouchPosition = touch.position;
            foreach (Touchable touchable in m_Touchables)
            {
                Vector2 size = touchable.RectTransform.sizeDelta;
                Vector2 touchablePos = touchable.transform.position;
                Vector2 dir = touchablePos - TouchPosition;
                bool touched = (dir.x < size.x / 2 && dir.x > -size.x / 2) && 
                                (dir.y < size.y / 2 && dir.y > -size.y / 2);

                if (touched) touchable.Touch();
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            foreach (Touchable touchable in m_Touchables)
            {
                touchable.Drop();
            }
        }
    }

    public void RegisterTouchable(Touchable touchable)
    {
        touchable.RectTransform = touchable.GetComponent<RectTransform>();
        m_Touchables.Add(touchable);
    }
}
