using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{

    new RectTransform transform;

    [SerializeField]
    GameObject player;

    [SerializeField]
    TextMeshProUGUI positionText;

    public GameObject[] tiles;

    int xPosOnGrid = 0;
    int yPosOnGrid = 0;

    void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        LocationManager locationManager = LocationManager.Instance;
        xPosOnGrid = Mathf.RoundToInt((locationManager.position.x / 10.0f) * (transform.rect.width / 7.0f));
        yPosOnGrid = Mathf.RoundToInt((locationManager.position.y / 10.0f) * (transform.rect.height / 7.0f));
        if (player != null)
        {
            player.transform.position = new Vector3(xPosOnGrid, yPosOnGrid);
            positionText.text = $"({xPosOnGrid}, {yPosOnGrid})";
        }
        else
        {
            positionText.text = "I am here";
        }
       
    }
}
