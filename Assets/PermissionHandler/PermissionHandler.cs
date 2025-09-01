using UnityEngine;
using UnityEngine.Android;

public class PermissionHandler : MonoBehaviour
{
    void Start()
    {
       
         if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
             Permission.RequestUserPermission(Permission.Camera);

         if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
             Permission.RequestUserPermission(Permission.Microphone);

         if (!Permission.HasUserAuthorizedPermission("android.permission.READ_CONTACTS"))
             Permission.RequestUserPermission("android.permission.READ_CONTACTS");

         if (!Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_IMAGES"))
                Permission.RequestUserPermission("android.permission.READ_MEDIA_IMAGES");
        
    }
}
