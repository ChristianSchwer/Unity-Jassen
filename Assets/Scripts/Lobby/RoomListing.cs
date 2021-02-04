using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text roomMaxPlayers;
    [SerializeField]
    private Text roomName;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        roomMaxPlayers.text = RoomInfo.PlayerCount + "/" + RoomInfo.MaxPlayers;
        roomName.text = RoomInfo.Name;
    }

    public void RemoveFromList()
    {
        Destroy(this.gameObject);
    }

    public void AddToList(RoomInfo info)
    {
        SetRoomInfo(info);
    }

    public void OnClick_Button()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
    }
}
