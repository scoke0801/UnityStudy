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
        // ������ Ŭ���̾�Ʈ�� �� �ڵ� ����ȭ �ɼ�.
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = version;

        PhotonNetwork.NickName = userId;

        //  ���� �������� ������ �ʴ� ���� Ƚ��.
        Debug.Log(PhotonNetwork.SendRate);

        // ���� ���� ����.
        PhotonNetwork.ConnectUsingSettings();
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
        PhotonNetwork.JoinRandomRoom();
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�.
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        // ���� �Ӽ� ����.
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 15;   // �뿡 ������ �� �ִ� �ִ� ������ ��.
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
