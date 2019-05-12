using System.Collections;
using System.Collections.Generic;
using TMPro;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance = null;

    [Header("Debug")]
    public bool DEBUGGING = false;

    [SerializeField]
    TextMeshProUGUI debugText = null;

    IEnumerator Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            yield break;
        }
        else
        {
            Instance = this;
        }

        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
    }

    LocationInfo locationInfo;
    void Update()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            locationInfo = Input.location.lastData;
            debugText.text =  "Latitude: " + locationInfo.latitude + " Longitude: " + locationInfo.longitude;
        }
    }
}
