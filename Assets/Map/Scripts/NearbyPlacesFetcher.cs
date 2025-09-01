using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NearbyPlacesFetcher : MonoBehaviour
{
    // Google Maps Places API URL (Make sure to secure your API key)
    public string apiUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?&location=28.5822777%2C77.3245374&radius=900&key=AIzaSyDApxEjE4ftEYvREz_Q-HwNADkrT6dUj4g";
    public NearbyPlacesMapData nearbyPlacesMapData;

    void Start()
    {
        StartCoroutine(FetchNearbyPlaces());
    }

    IEnumerator FetchNearbyPlaces()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Request Failed: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            nearbyPlacesMapData = JsonConvert.DeserializeObject<NearbyPlacesMapData>(json);
            Debug.Log("API Response: " + json);

            // You can parse the JSON here using a class or JSONUtility/Json.NET
            // Example: var result = JsonUtility.FromJson<PlaceResponse>(json);
        }
    }
}



// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[System.Serializable]
public class Geometry
{
    public Location location ;
    public Viewport viewport ;
}

[System.Serializable]
public class Location
{
    public double lat ;
    public double lng ;
}

[System.Serializable]
public class Northeast
{
    public double lat ;
    public double lng ;
}

[System.Serializable]
public class OpeningHours
{
    public bool open_now ;
}

[System.Serializable]
public class Photo
{
    public int height ;
    public List<string> html_attributions ;
    public string photo_reference ;
    public int width ;
}

[System.Serializable]
public class PlusCode
{
    public string compound_code ;
    public string global_code ;
}

[System.Serializable]
public class Result
{
    public Geometry geometry ;
    public string icon ;
    public string icon_background_color ;
    public string icon_mask_base_uri ;
    public string name ;
    public List<Photo> photos ;
    public string place_id ;
    public string reference ;
    public string scope ;
    public List<string> types ;
    public string vicinity ;
    public string business_status ;
    public OpeningHours opening_hours ;
    public PlusCode plus_code ;
    public double? rating ;
    public int? user_ratings_total ;
}

[System.Serializable]
public class NearbyPlacesMapData
{
    public List<object> html_attributions ;
    public string next_page_token ;
    public List<Result> results ;
    public string status ;
}

[System.Serializable]
public class Southwest
{
    public double lat ;
    public double lng ;
}

[System.Serializable]
public class Viewport
{
    public Northeast northeast ;
    public Southwest southwest ;
}


