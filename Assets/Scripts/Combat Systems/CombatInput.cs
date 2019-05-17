using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInput : MonoBehaviour
{
    private Vector3 touchStart;   //First touch position
    private Vector3 touchEnd;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered
    [SerializeField]
    LMckamey_Player m_player = default;
    [SerializeField]
    CombatManager m_manager = default;

    void Start()
    {
        dragDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
    }

    void Update()
    {
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                touchStart = touch.position;
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                touchEnd = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(touchEnd.x - touchStart.x) > dragDistance || Mathf.Abs(touchEnd.y - touchStart.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal

                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        m_player.SwitchElement((touchEnd.x > touchStart.x));    
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (touchEnd.y > touchStart.y)  //If the movement was up
                        {   //Up swipe
                            Debug.Log("Up Swipe");
                        }
                        else
                        {   //Down swipe
                            Debug.Log("Down Swipe");
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    m_manager.CheckIfEnemyTapped(touch.position);

                }
            }
        }
    }
}
