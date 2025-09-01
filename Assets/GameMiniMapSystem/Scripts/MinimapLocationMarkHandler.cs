
using UnityEngine;

public class MinimapLocationMarkHandler : MonoBehaviour
{

    public float markSizeOnFullMap;
    public float markSizeOnMiniMap;


    private void Start()
    {
        if (GameMapManager.instance != null)
        {
            GameMapManager.instance._minimapLocationMarkHandlers.Add(this);
            GameMapManager.instance.MissionMark();
        }
    }
    private void OnEnable()
    {
        Start();
    }

    /*private void OnDisable()
    {
        GameMapHandler.instance._minimapLocationMarkHandlers[x+1]
    }*/
}
