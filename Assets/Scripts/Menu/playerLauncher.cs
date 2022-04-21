using Photon.Pun;
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
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //PhotonNetwork.LoadLevel(0);
        SceneManager.LoadScene(0);
    }
}
