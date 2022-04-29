using NaughtyAttributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerLauncher : MonoBehaviourPunCallbacks
{
    public static playerLauncher Instance;

    [Header("Settings")] [SerializeField, Scene]
    private string sceneToLoad;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public void LeaveRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //PhotonNetwork.LoadLevel(0);
        SceneManager.LoadScene(sceneToLoad);
    }
}
