using UnityEngine;

public class Authorizer : MonoBehaviour
{
    public GameObject suspendedObject;
    
    void Start()
    {
        suspendedObject = new GameObject();
        #if UNITY_WEBPLAYER || UNITY_FLASH 
            yield Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization)
        #endif
            suspendedObject.SetActive(true);
    }
}
