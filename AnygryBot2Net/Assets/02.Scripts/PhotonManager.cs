using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";

    private string userId = "Zack";

    private void Awake()
    {
        // 마스터 클라이언트의 씬 자동 동기화 옵션.
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = version;

        PhotonNetwork.NickName = userId;

        //  포톤 서버와의 데이터 초당 전송 횟수.
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속.
        PhotonNetwork.ConnectUsingSettings();
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
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        // 룸의 속성 정의.
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 15;   // 룸에 입장할 수 있는 최대 접속자 수.
        room.IsOpen = true;
        room.IsVisible = true;

        PhotonNetwork.CreateRoom("My Room", room);
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

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
    }
}
