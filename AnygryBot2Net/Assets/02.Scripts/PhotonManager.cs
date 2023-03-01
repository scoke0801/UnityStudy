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
        // 마스터 클라이언트의 씬 자동 동기화 옵션.
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = version;

      //  PhotonNetwork.NickName = userId;

        //  포톤 서버와의 데이터 초당 전송 횟수.
        Debug.Log(PhotonNetwork.SendRate);

        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        if (PhotonNetwork.IsConnected == false)
        {
            // 포톤 서버 접속.
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
    // 포톤 서버에 접속 후 호출되는 콜백 함수.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conntected To Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 후 호출되는 콜백함수.
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        // PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        OnMakeRoomClick();

        // 룸의 속성 정의.
        //RoomOptions room = new RoomOptions();
        //room.MaxPlayers = 15;   // 룸에 입장할 수 있는 최대 접속자 수.
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

        // 마스터 클라이언트인 경우에 룸에 입장한 후 전투 씬을 로딩하낟.
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
            // 룸이 삭제된 경우.
            if (roomInfo.RemovedFromList == true)
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom);

                Destroy(tempRoom);

                rooms.Remove(roomInfo.Name);
            }
            else
            {
                // 룸이 생성된 경우.
                if( rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);

                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
                // 룸이 변경된 경우.
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
