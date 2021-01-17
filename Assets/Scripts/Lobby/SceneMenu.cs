using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMenu : MonoBehaviourPunCallbacks
{
    #region Private Fields

    private RoomsCanvases _roomsCanvases;
    [SerializeField]
    private GameObject backOptions;
    [SerializeField]
    private NetworkManager NetworkManager;

    #endregion

    #region Public Methods

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        _roomsCanvases.CurrentRoomCanvas.Hide();
    }

    public void OnClick_BackButton()
    {
        if (backOptions.activeSelf == true)
        {
            backOptions.SetActive(false);
        }
        else
        {
            backOptions.SetActive(true);
        }
    }

    public void OnClick_Singleplayer()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void OnClick_LobbyScene()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClick_HomeScene()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }

    public void OnClick_QuitGame()
    {
        Application.Quit();
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager: OnConnectedToMaster() was called by PUN.");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("NetworkManager: OnJoinedLobby() mit Lobby verbunden.");
        PhotonNetwork.LoadLevel(2);
    }

    #endregion
}
