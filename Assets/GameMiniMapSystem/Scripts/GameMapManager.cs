using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameMapManager : MonoBehaviour
{
    public static GameMapManager instance { get; set; }
    [Header("Map Component Ref. Settings")]
    public Transform gamePlayer;
    public Transform destinationLocationMarker;
    public Camera mapCamera;
    public GameObject fullMapPanel;
    public GameObject playerMiniMapLocationMarker;

    [Header("UI Elements")]
    public Button miniMapButton;
    public Button fullMapButton;
    public Slider mapZoomSlider;
    public Image mapImage;
    [SerializeField]
    Image[] InSideBuildingMaps;
    public List<MinimapLocationMarkHandler> _minimapLocationMarkHandlers = new List<MinimapLocationMarkHandler>();
    public LineRenderer navigationLineRenderer;

    [Header("Camera Value Settings")]
    [SerializeField]
    float maxCamSize;
    [SerializeField]
    float minCamSize;
    public bool isZoomDisabled;
    [SerializeField]
    bool isUseIfOnstartBuildingMapNum;
    [SerializeField]
    float OfsetYPos;
    
    
    public Vector3 lastTouchPos;
    [SerializeField]
    private bool isFullMapActive;
    int currentBuildingMapNum;
    float minXClamp, maxXClamp, minYClamp, maxYClamp;

    [Header("GPS Settings")]
    [SerializeReference]
    private bool gpsActive;
    [SerializeField]
    float _maxWidthLineRenderer;
    [SerializeField]
    float _minWidthLineRenderer;
    public float gpsYPos;
    private NavMeshPath navPath;
    
    public NavMeshAgent agent;
    public LayerMask AccessibleArea;

    [Header("Map Event")]
    public UnityEvent onFullMapEnable;
    public UnityEvent onMiniMapEnable;


    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        isFullMapActive = false;
        if (instance == null) instance = this;
        // Initialize UI Buttons
        miniMapButton.onClick.AddListener(SwitchToFullMap);
        fullMapButton.onClick.AddListener(SwitchToMiniMap);

        if(isUseIfOnstartBuildingMapNum == false)
        {
            // Initialize Zooming
            if (!isZoomDisabled)
            {
                mapZoomSlider.minValue = minCamSize;
                mapZoomSlider.maxValue = maxCamSize;
                mapZoomSlider.value = minCamSize;
                mapCamera.orthographicSize = minCamSize;
                CalculateMapBounds();
                // Initialize GPS Navigation
                navPath = new NavMeshPath();
                navigationLineRenderer.positionCount = 0;
            }
            else
            {
                Debug.LogWarning("Zoom is disabled.");

            }
        }
        if (onFullMapEnable == null)
        {
            onFullMapEnable = new UnityEvent();
        }
        if (onMiniMapEnable == null)
        {
            onMiniMapEnable = new UnityEvent();
        }
    }

    private void Update()
    {
        if (isFullMapActive)
        {
            if (!isZoomDisabled) HandleTouchPan();
        }
       

        if (gpsActive && gamePlayer != null && destinationLocationMarker != null)
        {
            if (NavMesh.CalculatePath(gamePlayer.position, destinationLocationMarker.position, agent.areaMask, navPath))
            {
                DrawGPSPath();
            }
        }
        
    }
    private void LateUpdate()
    {
        if (!isFullMapActive)
        {
            if (mapCamera != null)
            {
                mapCamera.transform.position = new Vector3(gamePlayer.position.x, gpsYPos + OfsetYPos, gamePlayer.position.z);
            }
        }
        
    }


    private void HandleTouchPan()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("input Chack");
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > Screen.width / 2)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("input Chack 2");
                    lastTouchPos = mapCamera.ScreenToWorldPoint(touch.position);
                }
                Vector3 difference = lastTouchPos - mapCamera.ScreenToWorldPoint(touch.position);
                mapCamera.transform.position += difference;
                mapCamera.transform.position = ClampCamera(mapCamera.transform.position);
            }
        }
    }


    private Vector3 ClampCamera(Vector3 targetPos)
    {
        float camHeight = mapCamera.orthographicSize;
        float minX = minXClamp + camHeight;
        float maxX = maxXClamp - camHeight;
        float minZ = minYClamp + camHeight;
        float maxZ = maxYClamp - camHeight;
        return new Vector3(Mathf.Clamp(targetPos.x, minX, maxX), 650f, Mathf.Clamp(targetPos.z, minZ, maxZ));
    }


    public void SwitchToFullMap()
    {
        isFullMapActive = true;
        miniMapButton.gameObject.SetActive(false);
        fullMapPanel.SetActive(true);
        mapCamera.enabled = true;
        
        if (!isZoomDisabled)
        {
            navigationLineRenderer.startWidth = _maxWidthLineRenderer;
            navigationLineRenderer.endWidth = _maxWidthLineRenderer;
            mapCamera.orthographicSize = minCamSize;
            mapZoomSlider.value = minCamSize;
            mapCamera.transform.position = new Vector3(gamePlayer.position.x, gpsYPos + OfsetYPos, gamePlayer.position.z);

        }
        foreach (MinimapLocationMarkHandler mark in _minimapLocationMarkHandlers)
        {
            mark.transform.localScale = new Vector3(mark.markSizeOnFullMap, mark.markSizeOnFullMap, mark.markSizeOnFullMap);
        }
        onFullMapEnable.Invoke();
        //if(JoystickSprint.instance != null) JoystickSprint.instance.CancelSprint();
    }

    public void SwitchToMiniMap()
    {
        miniMapButton.gameObject.SetActive(true);
        fullMapPanel.SetActive(false);
        //fullMapCamera.enabled = false;
        isFullMapActive = false;
        if (!isZoomDisabled)
        {
            navigationLineRenderer.startWidth = _minWidthLineRenderer;
            navigationLineRenderer.endWidth = _minWidthLineRenderer;
        }
        foreach (MinimapLocationMarkHandler mark in _minimapLocationMarkHandlers)
        {
            mark.transform.localScale = new Vector3(mark.markSizeOnMiniMap, mark.markSizeOnMiniMap, mark.markSizeOnMiniMap);
        }
        onMiniMapEnable.Invoke();
    }

    public void EnableGPS(bool isUse, Transform targetPos, Transform playerPos )
    {
        gamePlayer = playerPos;
        destinationLocationMarker = targetPos;
        gpsActive = isUse;
        navigationLineRenderer.gameObject.SetActive(true);
    }

    public void EnableGPSIfSameTarget(Transform targetPos)
    {
        destinationLocationMarker = targetPos;
        navigationLineRenderer.gameObject.SetActive(true);
        gpsActive = true;
    }

    public void DisableGPS()
    {
        //player = null;
        destinationLocationMarker = null;
        gpsActive = false;
        navigationLineRenderer.positionCount = 0;
        navigationLineRenderer.gameObject.SetActive(false);
    }

    private void DrawGPSPath()
    {
        navigationLineRenderer.positionCount = navPath.corners.Length;
        for (int i = 0; i < navPath.corners.Length; i++)
        {
            navigationLineRenderer.SetPosition(i, new Vector3(navPath.corners[i].x, gpsYPos, navPath.corners[i].z));
        }
    }

    private void CalculateMapBounds()
    {
        if (mapImage != null)
        {
            RectTransform rectTransform = mapImage.rectTransform;
            minXClamp = mapImage.transform.position.x - rectTransform.rect.width / 2;
            maxXClamp = mapImage.transform.position.x + rectTransform.rect.width / 2;
            minYClamp = mapImage.transform.position.z - rectTransform.rect.height / 2;
            maxYClamp = mapImage.transform.position.z + rectTransform.rect.height / 2;
        }
    }

    public void MinimapTarget(Transform newTarget)
    {
        gpsActive = true;
        destinationLocationMarker = newTarget;
        navigationLineRenderer.gameObject.SetActive(true);
    }

    public void MinimapNotUse()
    {
        gpsActive = false;
        destinationLocationMarker = null;
        navigationLineRenderer.gameObject.SetActive(false);
    }

    public void mapZoomChangeCheck()
    {
        mapCamera.orthographicSize = mapZoomSlider.value;
    }

    public void minimapUseinSide(int mapNum)
    {
        isZoomDisabled = true;
        MiniMapAccessibleArea();
        foreach (Image _map in InSideBuildingMaps)
        {
            _map.gameObject.SetActive(false);
        }
        InSideBuildingMaps[mapNum].gameObject.SetActive(true);
        mapCamera.transform.position = new Vector3(InSideBuildingMaps[mapNum].transform.position.x, gpsYPos + OfsetYPos, InSideBuildingMaps[mapNum].transform.position.z);
        mapCamera.transform.rotation = InSideBuildingMaps[mapNum].transform.rotation;
        mapCamera.orthographicSize = InSideBuildingMaps[mapNum].rectTransform.rect.height /2;
        mapZoomSlider.interactable = false;
    }

    public void miniMapNormal()
    {
        isZoomDisabled = false;
        mapZoomSlider.interactable = true;
        InSideBuildingMaps[currentBuildingMapNum].gameObject.SetActive(false);
    }

    public void MissionMark()
    {
        foreach(MinimapLocationMarkHandler mark in _minimapLocationMarkHandlers)
        {
            mark.transform.localScale = new Vector3(mark.markSizeOnMiniMap, mark.markSizeOnMiniMap, mark.markSizeOnMiniMap);
        }
    }

    public void MiniMapNotAccessibleArea()
    {
        mapCamera.cullingMask = 0;
    }
    public void MiniMapAccessibleArea()
    {
        mapCamera.cullingMask = AccessibleArea;
        
    }
}
