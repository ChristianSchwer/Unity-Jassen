﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Private Fields

    [SerializeField]
    private string gameVersion = "3";
    [SerializeField]
    private GameObject controlPanel;
    [SerializeField]
    private GameObject progressLabel;
    private const byte CARDS_SHUFFLE_EVENT = 1;

    #endregion

    #region Public Fields

    public InputField nickName;

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        PhotonNetwork.NickName = nickName.text;
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("NetworkManager: Connect() wird ausgeführt.");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("NetworkManager: OnDisconnected() Reason " + cause.ToString());
    }

    #endregion
}
