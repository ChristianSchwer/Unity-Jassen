using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public SwitchScene SwitchScene;
    string gameVersion = "1";

    // Start is called before the first frame update
    void Start()
    {
        //desk = GameObject.Find("Main_Canvas").transform.Find("Desk").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public void Connect()
    {
        Debug.Log("Connect wird ausgeführt.");
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    void OnConnectedToMaster()
    {
        Debug.Log("Mit Master verbunden. Szene für Lobby laden.");
        PhotonNetwork.JoinLobby();
    }

    void OnJoinedLobby()
    {
        Debug.Log("Wir sind mit der Lobby verbunden.");
        SwitchScene.NextScene("Multiplayer");
        PhotonNetwork.JoinRandomRoom();
    }
    
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("asdf");
        PhotonNetwork.CreateRoom(null);
        Debug.Log("New Room");
    }

    void OnJoinedRoom()
    {
        print("ok");
        Spawn();
    }

    void Spawn()
      {
        foreach (var player in PhotonNetwork.playerList)
        {
            print(player);
          /*//if (i == 0)
          //{
          desk.transform.Find("PlayerName1").GetComponent<Text> ().text = player.NickName;
          //}
          //if (i == 1)
          //{
          //  desk.transform.Find("PlayerName2").GetComponent<Text> ().text = player.NickName;
          //}
          //if (i == 2)
          //{
          //  desk.transform.Find("PlayerName3").GetComponent<Text> ().text = player.NickName;
          //}
          //if (i == 3)
          //{
          //  desk.transform.Find("PlayerName4").GetComponent<Text> ().text = player.NickName;
          //}
          //i++;*/
        }
      }

    public void SaveNickname()
    {
        string nickName = GameObject.Find("NicknameText").GetComponent<Text>().text.ToString();
        PhotonNetwork.player.NickName = nickName;
    }
}
