using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MultiplayerSystem : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        
        Debug.Log("Connected To Server");
    }
}
