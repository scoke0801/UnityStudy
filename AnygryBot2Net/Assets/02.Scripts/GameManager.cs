using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public TMP_Text msgList;
    public Button exitBtn;

    private void Awake()
    {
        CreatePlayer();

        SetRoomInfo();

        exitBtn.onClick.AddListener(() => OnExitClick());
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);

        PhotonNetwork.Instantiate("Player",
            points[idx].position,
            points[idx].rotation,
            0);
    }

    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");    
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room";
        msgList.text += msg;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room";
        msgList.text += msg;
    }
}
