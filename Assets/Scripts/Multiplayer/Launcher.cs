using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [Header("Dependencies")]
    [SerializeField] private TMP_InputField createdRoomName;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    [Header("Settings")] [SerializeField, Scene]
    private string sceneToLoad;

    private void Awake()
    {
        Instance = this;
    }

    /** CONNECTING TO LOBBY = SERVER **/
    private void Start()
    {
        if(!PhotonNetwork.IsConnected) //for when player leaves a room
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("mainMenu");
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    /** Creating/Joining ROOMS  **/
    
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createdRoomName.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(createdRoomName.text);
        MenuManager.Instance.OpenMenu("loadingMenu");
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed with message: " + message;
        MenuManager.Instance.OpenMenu("errorMenu");
    }
    
    public void JoinRoom(RoomInfo info)
    {
        MenuManager.Instance.OpenMenu("loadingMenu");
        PhotonNetwork.JoinRoom(info.Name);
        
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room join failed with message: " + message;
        MenuManager.Instance.OpenMenu("errorMenu");
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("loadingMenu");
        StartGame();
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Debug.Log("Called Room List Update!");
        foreach (Transform t in roomListContent)
        {
            // if (t.TryGetComponent(out RoomListItem roomListItem))
            // {
            //     Debug.Log("Room Name: "+ roomListItem.info.Name+ " - Removed: " + roomListItem.info.IsOpen);
            // }
            Destroy(t.gameObject); 
        }

        foreach (RoomInfo t in roomList.Where(t =>  !t.RemovedFromList ))
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(t);
        }
    }
    
    private void StartGame()
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }
    
    /** other utilities **/
    
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
