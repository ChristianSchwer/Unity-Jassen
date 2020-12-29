using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Private Fields

    [SerializeField]
    private string gameVersion = "1";
    [SerializeField]
    private GameObject controlPanel;
    [SerializeField]
    private GameObject progressLabel;

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



    //void Spawn()
    //  {
    //    foreach (var player in PhotonNetwork.playerList)
    //    {
    //        print(player);
    //      /*//if (i == 0)
    //      //{
    //      desk.transform.Find("PlayerName1").GetComponent<Text> ().text = player.NickName;
    //      //}
    //      //if (i == 1)
    //      //{
    //      //  desk.transform.Find("PlayerName2").GetComponent<Text> ().text = player.NickName;
    //      //}
    //      //if (i == 2)
    //      //{
    //      //  desk.transform.Find("PlayerName3").GetComponent<Text> ().text = player.NickName;
    //      //}
    //      //if (i == 3)
    //      //{
    //      //  desk.transform.Find("PlayerName4").GetComponent<Text> ().text = player.NickName;
    //      //}
    //      //i++;*/
    //    }
    //  }


}
