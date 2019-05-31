using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Android;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance = null;

    [Header("Debug")]
    public bool DEBUGGING = false;

    [Space]

    [Header("Settings")]
    [SerializeField]
    TextMeshProUGUI InformationText = null;

    [SerializeField]
    TextMeshProUGUI LatitudeText = null;

    [SerializeField]
    TextMeshProUGUI LongitudeText = null;

    [Range(.1f, 1.0f)]
    [SerializeField]
    private float updateTime = .1f;

    //LocationInfo previousLocationInfo;
    LocationInfo currentLocationInfo;
    LocationInfo originalLocationInfo;

    Vector2 distance = Vector2.zero;
    float totalDistanceTraveled = 0.0f;
    [HideInInspector]
    public Vector2 position = Vector2.zero;

    const float EarthRadius = 6371.137f;

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

        Input.location.Start(2.0f, 1.0f);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait == 0)
        {
            yield break;
        }

        UpdateInformation();

        StartCoroutine(UpdateLocation());
    }

    bool initialLocationSet = false;
    int updates = 0;

    IEnumerator UpdateLocation()
    {
        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                if (!initialLocationSet && updates == 10)
                {
                    originalLocationInfo = Input.location.lastData;
                    initialLocationSet = true;
                }
                else
                {
                    updates++;
                }

                if (initialLocationSet)
                {
                    //previousLocationInfo = currentLocationInfo;
                    currentLocationInfo = Input.location.lastData;

                    float distanceTraveled = CalculateDistance(
                        originalLocationInfo.latitude,
                        currentLocationInfo.latitude,
                        originalLocationInfo.longitude,
                        currentLocationInfo.longitude);

                    if (distanceTraveled - totalDistanceTraveled >= 1.0f)
                    {
                        Vector2 direction = CalculateDirection(
                            originalLocationInfo.latitude,
                            currentLocationInfo.latitude,
                            originalLocationInfo.longitude,
                            currentLocationInfo.longitude);

                        totalDistanceTraveled = distanceTraveled;
                        position = direction * totalDistanceTraveled;
                    }

                    UpdateInformation();
                }
            }

            yield return new WaitForSeconds(updateTime);
        }
    }

    private float CalculateDistance(float lat1, float lat2, float lon1, float lon2)
    {
        float rLat1 = lat1 * Mathf.Deg2Rad;
        float rLat2 = lat2 * Mathf.Deg2Rad;
        float rLon1 = lon1 * Mathf.Deg2Rad;
        float rLon2 = lon2 * Mathf.Deg2Rad;

        float dLat = (rLat2 - rLat1);
        float dLon = (rLon2 - rLon1);

        float a = Mathf.Pow(Mathf.Sin(dLat / 2), 2) + (Mathf.Pow(Mathf.Sin(dLon / 2), 2) * Mathf.Cos(rLat1) * Mathf.Cos(rLat2));
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float total_dist = EarthRadius * c * 1000.0f;

        return total_dist;
    }

    private Vector2 CalculateDirection(float lat1, float lat2, float lon1, float lon2)
    {
        Vector2 direction = Vector2.zero;

        float rLat1 = lat1 * Mathf.Deg2Rad;
        float rLat2 = lat2 * Mathf.Deg2Rad;
        float rLon1 = lon1 * Mathf.Deg2Rad;
        float rLon2 = lon2 * Mathf.Deg2Rad;

        Vector2 oldPosition = new Vector2((EarthRadius * Mathf.Cos(rLat1) * Mathf.Cos(rLon1)), EarthRadius * Mathf.Cos(rLat1) * Mathf.Sin(rLon1));
        Vector2 newPosition = new Vector2((EarthRadius * Mathf.Cos(rLat2) * Mathf.Cos(rLon2)), EarthRadius * Mathf.Cos(rLat2) * Mathf.Sin(rLon2));

        direction = (newPosition - oldPosition).normalized;

        return direction;
    }

    private void UpdateInformation()
    {
        LatitudeText.text = $"Latitude: {currentLocationInfo.latitude}";
        LongitudeText.text = $"Longitude: {currentLocationInfo.longitude}";
        InformationText.text = $"Distance from start: {totalDistanceTraveled} \n" +
            $"Position: {position.ToString("F5")}";
    }
}