using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    #region Private Fields

    [SerializeField]
    private Text _roomName;
    [SerializeField]
    private InputField _numberOfPlayer;

    private RoomsCanvases _roomsCanvases;

    #endregion

    #region Public Fields

    int maxPlayersPerRoom = 4;

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        //maxPlayersPerRoom = Convert.ToInt32(_numberOfPlayer.text);
    }

    #endregion

    #region Public Methods

    public void OnClick_CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        Debug.Log("NetworkManager: CreateRoom()");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = Convert.ToByte(maxPlayersPerRoom);
        PhotonNetwork.CreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager: OnCreatedRoom() Room successfully created.", this);
        _roomsCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("NetworkManager: OnCreatedRoomFailed() Room creation failed: " + message, this);
    }

    #endregion
}
