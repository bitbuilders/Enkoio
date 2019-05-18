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
    [Space]
    [Header("Settings")]
    [SerializeField]
    TextMeshProUGUI currentLocationText = null;

    [Range(.1f, 1.0f)]
    [SerializeField]
    private float updateTime = .1f;

    LocationInfo previousLocationInfo;
    LocationInfo currentLocationInfo;
    Vector2 distance = Vector2.zero;

    IEnumerator Start()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            yield break;
        }
        else
        {
            Instance = this;
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        StartCoroutine(UpdateLocation());
    }

    void Update()
    {
    }

    IEnumerator UpdateLocation()
    {
        while (true)
        {
            if(Input.location.status == LocationServiceStatus.Running)
            {
                previousLocationInfo = currentLocationInfo;
                currentLocationInfo = Input.location.lastData;
                currentLocationText.text = "Latitude: " + currentLocationInfo.latitude + " Longitude: " + currentLocationInfo.longitude;

                Vector2 direction = new Vector2(
                    currentLocationInfo.latitude - previousLocationInfo.latitude,
                    currentLocationInfo.longitude - previousLocationInfo.longitude).normalized;

                float distanceTraveled = CalculateDistance(
                    previousLocationInfo.latitude,
                    currentLocationInfo.latitude,
                    previousLocationInfo.longitude,
                    currentLocationInfo.longitude);

                distance += direction * distanceTraveled;
                if(DEBUGGING)
                {
                    Debug.Log(distance);
                }
            }
            if (!DEBUGGING)
            {
                yield return new WaitForSecondsRealtime(updateTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    private float CalculateDistance(float lat_1, float lat_2, float long_1, float long_2)
    {
        int R = 6371;
        var lat_rad_1 = Mathf.Deg2Rad * lat_1;
        var lat_rad_2 = Mathf.Deg2Rad * lat_2;
        var d_lat_rad = Mathf.Deg2Rad * (lat_2 - lat_1);
        var d_long_rad = Mathf.Deg2Rad * (long_2 - long_1);
        var a = Mathf.Pow(Mathf.Sin(d_lat_rad / 2), 2) + (Mathf.Pow(Mathf.Sin(d_long_rad / 2), 2) * Mathf.Cos(lat_rad_1) * Mathf.Cos(lat_rad_2));
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var total_dist = R * c * 1000;
        //feet 3280.84
        return total_dist;
    }
}
