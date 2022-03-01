using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class MultiplayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField newRoomName;
    [SerializeField] private TMP_InputField roomName;
    
    
    public GameObject playerPrefab;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(newRoomName.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Lobby");
        Vector3 randomPosition = new Vector3(Random.Range(-5, 5), 2, Random.Range(-5, 5));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
}
}
