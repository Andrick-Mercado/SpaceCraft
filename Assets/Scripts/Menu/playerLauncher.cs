using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerLauncher : MonoBehaviourPunCallbacks
{
    public static playerLauncher Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public void LeaveRoom()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    
}
