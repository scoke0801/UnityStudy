using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private RoomInfo _roomInfo;
    private TMP_Text roomInfoText;
    private PhotonManager photonManager;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";

            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    private void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    void OnEnterRoom(string roomName)
    {
        photonManager.SetUserId();

        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 15;
        roomOption.IsOpen = true;
        roomOption.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOption, TypedLobby.Default);
    }
}

