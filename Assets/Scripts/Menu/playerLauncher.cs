using NaughtyAttributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerLauncher : MonoBehaviourPunCallbacks
{
    public static playerLauncher Instance;

    [Header("Settings")] [SerializeField, Scene]
    private string sceneToLoad;

    private PhotonView _view;
    
    private void Awake()
    {
        Instance = this;
        _view = GetComponent<PhotonView>();
    }
    
    public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _view.RPC(nameof(LeaveRoomPlayers), RpcTarget.AllBuffered);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Destroy(RoomManager.Instance.gameObject);
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        //PhotonNetwork.LoadLevel(0);
        SceneManager.LoadScene(sceneToLoad);
    }

    [PunRPC]
    private void LeaveRoomPlayers()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }
}
