using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInput : MonoBehaviour
{
    private Vector3 touchStart;  
    private Vector3 touchEnd;    
    private float dragDistance;  
    [SerializeField]
    LMckamey_Player m_player = default;
    [SerializeField]
    CombatManager m_manager = default;

    void Awake()
    {
        dragDistance = Screen.height * 15 / 100; 
    }

    void Update()
    {
        if (Input.touchCount == 1) 
        {
            Touch touch = Input.GetTouch(0); 
            if (touch.phase == TouchPhase.Began) 
            {
                touchStart = touch.position;
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) 
            {
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) 
            {
                touchEnd = touch.position; 

                
                if (Mathf.Abs(touchEnd.x - touchStart.x) > dragDistance || Mathf.Abs(touchEnd.y - touchStart.y) > dragDistance)
                {
                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {
                        m_player.SwitchElement((touchEnd.x > touchStart.x));    
                    }
                    else
                    {
                        if (touchEnd.y > touchStart.y)  
                        {   
                            Debug.Log("Up Swipe");
                        }
                        else
                        {   
                            Debug.Log("Down Swipe");
                        }
                    }
                }
                else
                {   
                    m_manager.CheckIfEnemyTapped(touch.position);

                }
            }
        }
    }
}
