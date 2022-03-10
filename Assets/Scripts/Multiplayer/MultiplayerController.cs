using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MultiplayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField newRoomName;
    [SerializeField] private TMP_InputField roomName;
    
    
    public GameObject playerPrefab;
    public GameObject terrainPrefab;
    public PGCTerrain terrainScript;

    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = PhotonView.Get(this);
    }

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
        if (PhotonNetwork.IsMasterClient)
        {
            //make seed here and input it into method below, 
            _photonView.RPC("changeTerrainBuild", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void changeTerrainBuild()//take seed and create same terrain here
    {
        terrainScript.GenerateTerrainData(true);
    }
}
