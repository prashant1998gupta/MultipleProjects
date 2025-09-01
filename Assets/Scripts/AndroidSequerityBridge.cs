using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AndroidSequerityBridge : MonoBehaviour
{
    private AndroidJavaObject activity;
    private AndroidJavaClass bridge;

    public bool isBreakSequerity = false;
    private bool isHighRisk = false;
    private bool IsTimerComplete = false;
    private string securityIssue = "";

    private float latitude;
    private float longitude;

    private float currentTimer = 10f;

    private readonly string bridgeClas = "com.aheaditec.talsec.demoapp.TalsecBridge";
    bool isMainUrl = true; // Replace with actual condition to check if the URL is main or not
    public static AndroidSequerityBridge instance { get; set; }

    //public DontDestroyManager dontDestroyManager = null;

    private void Awake()
    {
        if (instance == null) instance = this;
        //if (dontDestroyManager == null) FindObjectOfType<DontDestroyManager>();

        InitSecurity();
    }

    internal void InitSecurity()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CheckSecurityOnStart();   
        GetIPAndLocation();
#endif
    }
    public void CheckSecurityOnClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CheckSecurityOnStart();   
#endif
    }
    private void CheckSecurityOnStart()
    {
        isHighRisk = false;
        isBreakSequerity = false;
        //if(dontDestroyManager.securityPopup != null) dontDestroyManager.securityPopup.SetActive(false);
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        bridge = new AndroidJavaClass(bridgeClas);

        Debug.Log("Emulator: " + bridge.CallStatic<bool>("isEmulator"));   // Checked
        Debug.Log("App Repackaged: " + bridge.CallStatic<bool>("isAppRepackaged", activity));  // Checked
        Debug.Log("Rooted: " + bridge.CallStatic<bool>("isRooted", activity));
        Debug.Log("Check Tampering: " + bridge.CallStatic<bool>("checkForTampering", activity));   // Checked
        Debug.Log("Check Installer: " + bridge.CallStatic<bool>("checkInstaller", activity));
        Debug.Log("Hooking: " + bridge.CallStatic<bool>("detectHooking", activity));
        Debug.Log("Device Binding: " + bridge.CallStatic<bool>("isDeviceBound", activity));   // Checked
        Debug.Log("Malware: " + bridge.CallStatic<bool>("checkForMalware", activity));
        Debug.Log("isHardwareBackedKeyStoreAvailable: " + bridge.CallStatic<bool>("isHardwareBackedKeyStoreAvailable"));
        Debug.Log("VPN Active: " + bridge.CallStatic<bool>("isVPNActive", activity));    // Checked
        Debug.Log("Debugger Attached: " + bridge.CallStatic<bool>("isDebuggerAttached"));
        Debug.Log("Developer Mode: " + bridge.CallStatic<bool>("isDeveloperModeEnabled", activity));    // Checked
        Debug.Log("ADB Enabled: " + bridge.CallStatic<bool>("isAdbEnabled", activity));


        Debug.Log("isDynamicCodeLoaded: " + bridge.CallStatic<bool>("isDynamicCodeLoaded"));
        Debug.Log("isPtraceAttached: " + bridge.CallStatic<bool>("isPtraceAttached"));
        Debug.Log("isOverlayEnabled Enabled: " + bridge.CallStatic<bool>("isOverlayEnabled",activity));
        Debug.Log("isAccessibilityEnabled Enabled: " + bridge.CallStatic<bool>("isAccessibilityEnabled",activity));
        Debug.Log("detectHiddenRoot Enabled: " + bridge.CallStatic<bool>("detectHiddenRoot"));
        Debug.Log("isDNSHijackedOrBehindCDN Enabled: " + bridge.CallStatic<bool>("isDNSHijackedOrBehindCDN"));
        Debug.Log("isTimeSpoofed Enabled: " + bridge.CallStatic<bool>("isTimeSpoofed"));
        Debug.Log("isMockLocationEnabled Enabled: " + bridge.CallStatic<bool>("isMockLocationEnabled", activity));
        Debug.Log("isAppCloned Enabled: " + bridge.CallStatic<bool>("isAppCloned", activity));

        if (bridge.CallStatic<bool>("isEmulator"))
        {
            SetSecurityIssue("Emulator", true);
        }
        else if (!bridge.CallStatic<bool>("isAppRepackaged", activity))
        {
            SetSecurityIssue("AppRepackaged", true);
        }
        else if (bridge.CallStatic<bool>("isRooted", activity))
        {
            SetSecurityIssue("Rooted", true);
        }
        else if (bridge.CallStatic<bool>("checkForTampering", activity))
        {
            SetSecurityIssue("Tampering", true);
        }
        else if (bridge.CallStatic<bool>("checkInstaller", activity))
        {
            SetSecurityIssue("Installer", true);
        }
        else if (!bridge.CallStatic<bool>("isDeviceBound", activity))
        {
            SetSecurityIssue("DeviceBound", true);
        }
        else if (bridge.CallStatic<bool>("checkForMalware", activity))
        {
            string result = bridge.CallStatic<string>("checkForMalwaress", activity);
            if (!string.IsNullOrEmpty(result))
            {
                Debug.Log("Malware Detected: " + result);
            }
            else
            {
                Debug.Log("No malware detected.");
            }
            SetSecurityIssue(string.Concat("Malware found : ", result), true);
        }
        else if (!bridge.CallStatic<bool>("isHardwareBackedKeyStoreAvailable"))
        {
            SetSecurityIssue("KeyStore", true);
        }
        else if (bridge.CallStatic<bool>("isVPNActive", activity))
        {
            SetSecurityIssue("VPN", true);
        }
        else if (bridge.CallStatic<bool>("isDebuggerAttached"))
        {
            SetSecurityIssue("Debugger", true);
        }
        else if (bridge.CallStatic<bool>("detectHooking", activity))
        {
            SetSecurityIssue("Hooking", true);
        }
        else if (bridge.CallStatic<bool>("isDynamicCodeLoaded"))
        {
            SetSecurityIssue("Dynamic code detected", false);
        }
        else if (bridge.CallStatic<bool>("isPtraceAttached"))
        {
            SetSecurityIssue("isPtraceAttached", false);
        }
        else if (bridge.CallStatic<bool>("isOverlayEnabled", activity))
        {
            SetSecurityIssue("OverlayEnabled", false);
        }
        else if (bridge.CallStatic<bool>("detectHiddenRoot"))
        {
            SetSecurityIssue("detectHiddenRoot", false);
        }
        else if (bridge.CallStatic<bool>("isMockLocationEnabled", activity))
        {
            SetSecurityIssue("isMockLocationEnabled", false);
        }
        else if (bridge.CallStatic<bool>("isAppCloned", activity))
        {
            SetSecurityIssue("isAppCloned", false);
        }
        else if (bridge.CallStatic<bool>("isDeveloperModeEnabled", activity))
        {
            if (bridge.CallStatic<bool>("isAdbEnabled", activity))
            {
                //dontDestroyManager.securityPopup.SetActive(true);
                SetSecurityIssue("DebuggingEnabled", true);
                StartCoroutine(CloseSecurityPopup());
            }
            else if (IsWirelessDebuggingEnabled())
            {
                //dontDestroyManager.securityPopup.SetActive(true);
                SetSecurityIssue("DebuggingEnabled", true);
                StartCoroutine(CloseSecurityPopup());
            }
        }
        //else if (bridge.CallStatic<bool>("isAccessibilityEnabled", activity))
        //{
        //    SetSecurityIssue("isAccessibilityEnabled", false);
        //}
        //else if (bridge.CallStatic<bool>("isDNSHijackedOrBehindCDN"))
        //{
        //    SetSecurityIssue("isDNSHijackedOrBehindCDN", false);
        //}
        //else if (bridge.CallStatic<bool>("isTimeSpoofed"))
        //{
        //    SetSecurityIssue("isTimeSpoofed", false);
        //}

        Debug.Log("getDeviceId: " + bridge.CallStatic<string>("getDeviceId", activity));
        Debug.Log("Security issue found: " + securityIssue);
        //  bridge.CallStatic("preventScreenshots", activity);

        

        if (isBreakSequerity && isMainUrl /*&& !dontDestroyManager.securityPopup.activeSelf*/)
        {
            string riskType = string.Empty;
            if (isHighRisk) riskType = "High";
            else riskType = "Low";
            StartCoroutine(SendSecurityAlert(securityIssue, riskType));
        }
    }
    private void SetSecurityIssue(string issue, bool highRisk)
    {
        securityIssue = issue;
        isHighRisk = highRisk;
        isBreakSequerity = true;
    }
    private IEnumerator CloseSecurityPopup()
    {
        currentTimer -= 1f;
        string t = currentTimer.ToString();
        yield return new WaitForSecondsRealtime(1);
        //dontDestroyManager.securityPopup_timerText.text =  t.ToString();
        if (currentTimer <= 0 && !IsTimerComplete)
        {
            IsTimerComplete = true;
            string riskType = string.Empty;
            if (isHighRisk) riskType = "High";
            else riskType = "Low";
            StartCoroutine(SendSecurityAlert(securityIssue, riskType));
            //dontDestroyManager.securityPopup.SetActive(false);
        }
        else
        {
            StartCoroutine(CloseSecurityPopup());
        }
    }
    private IEnumerator SendSecurityAlert(string issue, string riskType)
    {
        string url = "https://testgdbe.metaspacechain.com/dashboard/api/v1/player/security-threat";
        string osInfo = SystemInfo.operatingSystem;
        string shortOs = osInfo.Split('/')[0].Trim();

      /*  if(string.IsNullOrEmpty(Prashant.Utils.UserId))
        {
            Prashant.Utils.UserId = "Hacker_Attacker";
        }*/

       /* string ectraNotes = GetSimDetails();
        if (string.IsNullOrEmpty(ectraNotes))
        {
            ectraNotes = "No SIM details available";
        }*/

        string localIP = GetLocalIPAddress();
        if (string.IsNullOrEmpty(localIP) || localIP == "Unavailable" || localIP == "Error retrieving IP")
        {
            localIP = "Local IP not available";
        }

        string networkType = GetNetworkType();
        if (string.IsNullOrEmpty(networkType))
        {
            networkType = "Network type not available";
        }

        SecurityPayload payload = new SecurityPayload
        {
            firebase_id = "Prashant.Utils.UserId",
            issue = issue,
            device_id = SystemInfo.deviceUniqueIdentifier,
            device_model = SystemInfo.deviceModel,
            os_version = shortOs,
            app_version = Application.version,
            timestamp_utc = System.DateTime.UtcNow.ToString("o"),
            ip_address = localIP,
            user_id = "Prashant.Utils.UserId",
            network_type = networkType,
            threat_level = riskType,
            //extra_notes = ectraNotes,
          //  lat = "12.345678",
         //   long = "98.765432",
        };

        string jsonData = JsonUtility.ToJson(payload);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to send security alert: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Security alert sent successfully.");
        }

        if (isMainUrl)
        {
            Debug.Log("[UAT Mode] Application Quit.");
            Application.Quit(); // Enforce quit in production
        }
        else
        {
            Debug.Log("[UAT Mode] Application.Quit skipped.");
        }
    }
    private string GetLocalIPAddress()
    {
        string localIP = "Unavailable";
        try
        {
            foreach (var host in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (host.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    localIP = host.ToString();
                    break;
                }
            }
        }
        catch
        {
            localIP = "Error retrieving IP";
        }
        return localIP;
    }
    private string GetNetworkType()
    {
        string networkType = string.Empty;
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                {
                    networkType = "No Internet";
                    Debug.Log("No Internet Connection");
                    break;
                }
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                {
                    networkType = "Mobile Data";
                    Debug.Log("Connected via Mobile Data");
                    break;
                }
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                {
                    networkType = "Wi-Fi";
                    Debug.Log("Connected via Wi-Fi");
                }
                break;
        }
        return networkType;
    }
    public string GetIPAndLocation()
    {
        // Call Kotlin static method
        using (AndroidJavaClass networkUtilClass = new AndroidJavaClass("com.aheaditec.talsec.demoapp.TalsecBridge"))
        {
            string result = networkUtilClass.CallStatic<string>("fetchIPAndLocationDetails");
            Debug.Log(result);
            return result;
        }
    }
    public string GetSimDetails()
    {
        // Call Kotlin static method
        using (AndroidJavaClass networkUtilClass = new AndroidJavaClass("com.aheaditec.talsec.demoapp.TalsecBridge"))
        {
            try
            {
                string result = networkUtilClass.CallStatic<string>("fetchAllSIMDetails");
                Debug.Log(result);
                return result;
            }
            catch (System.Exception)
            {
                return "No SIM details available";
            }

        }
    }
    private IEnumerator StartLocationService()
    {
        // Check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled");
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in time
        if (maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log("Latitude: " + latitude + ", Longitude: " + longitude);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
    public bool IsWirelessDebuggingEnabled()
    {
        bool isWirelessDebugging = false;

        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass javaClass = new AndroidJavaClass(bridgeClas))
                {
                    // Call the static method isWirelessDebuggingEnabled()
                    isWirelessDebugging = javaClass.CallStatic<bool>("isWirelessDebuggingEnabled");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error checking wireless debugging: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("This feature is only available on Android.");
        }

        return isWirelessDebugging;
    }
    [System.Serializable]
    public class SecurityPayload
    {
        public string firebase_id;
        public string issue;
        public string device_id;
        public string device_model;
        public string os_version;
        public string app_version;
        public string timestamp_utc;
        public string ip_address;
        public string user_id;
        public string network_type;
        public string threat_level;
        public string extra_notes;
        public string lat;
        //public string long;
    }
}
