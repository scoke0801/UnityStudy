using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";

    private string userId = "Zack";

    public TMP_InputField userInputField;
    public TMP_InputField roomInputField;

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private GameObject roomItemPrefab;
    public Transform scrollContent;

    private void Awake()
    {
        // ������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�.
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = version;

      //  PhotonNetwork.NickName = userId;

        //  ���� �������� ������ �ʴ� ���� Ƚ��.
        Debug.Log(PhotonNetwork.SendRate);

        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (PhotonNetwork.IsConnected == false)
        {
            // ���� ���� ����.
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 16):00}");
        userInputField.text = userId;
        PhotonNetwork.NickName = userId;
    }

    public void SetUserId()
    {
        if(string.IsNullOrEmpty(userInputField.text))
        {
            userId = $"USER_{Random.Range(1,16):00}";
        }
        else
        {
            userId = userInputField.text;
        }

        PlayerPrefs.SetString("USER_ID", userId);
        PhotonNetwork.NickName = userId;
    }

    string SetRoomName()
    {
        if(string.IsNullOrEmpty(roomInputField.text))
        {
            roomInputField.text = $"ROOM_{Random.Range(1,101):000}";
        }

        return roomInputField.text;
    }
    // ���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conntected To Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // �κ� ���� �� ȣ��Ǵ� �ݹ��Լ�.
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        // PhotonNetwork.JoinRandomRoom();
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        OnMakeRoomClick();

        // ���� �Ӽ� ����.
        //RoomOptions room = new RoomOptions();
        //room.MaxPlayers = 15;   // �뿡 ������ �� �ִ� �ִ� ������ ��.
        //room.IsOpen = true;
        //room.IsVisible = true;

        //PhotonNetwork.CreateRoom("My Room", room);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }   

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach( var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        //Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int idx = Random.Range(1, points.Length);

        //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        // ������ Ŭ���̾�Ʈ�� ��쿡 �뿡 ������ �� ���� ���� �ε��ϳ�.
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach(var roomInfo in roomList)
        {
            // ���� ������ ���.
            if (roomInfo.RemovedFromList == true)
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom);

                Destroy(tempRoom);

                rooms.Remove(roomInfo.Name);
            }
            else
            {
                // ���� ������ ���.
                if( rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);

                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
                // ���� ����� ���.
                else
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }            
            Debug.Log($"Room={roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})");
        }
    }
    #region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserId();

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        SetUserId();

        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 15;
        roomOption.IsOpen = true;
        roomOption.IsVisible = true;

        PhotonNetwork.CreateRoom(SetRoomName(), roomOption);
    }
    #endregion
}
