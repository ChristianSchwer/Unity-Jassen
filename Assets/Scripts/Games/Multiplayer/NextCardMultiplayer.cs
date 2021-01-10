using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

public class NextCardMultiplayer : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public GameObject DropZone;

    string player;
    string firstLetter;
    string trumpf;
    int move;

    public GameObject playerHand;

    List<string> player1 = new List<string>();
    List<string> player2 = new List<string>();
    List<string> player3 = new List<string>();
    List<string> player4 = new List<string>();
    List<GameObject> cards = new List<GameObject>();

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_CARDS_EVENT = 4;
    private const byte SET_AKTIVE_EVENT = 5;
    private const byte CURRENT_MOVE_EVENT = 6;
    private const byte SEND_PLAYER1HAND_EVENT = 7;
    private const byte SEND_PLAYER2HAND_EVENT = 8;
    private const byte SEND_PLAYER3HAND_EVENT = 9;
    private const byte SEND_PLAYER4HAND_EVENT = 10;
    private const byte SEND_TRUMPF_EVENT = 11;

    //TurnState state;          Enum over all scripts, dosn't work

    public GameManagerMultiplayer gameManagerMultiplayer;

    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private GameObject playerHandOverlay;

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        move = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerMultiplayer.activePlayer == 1)
        {
            gameManagerMultiplayer.PlayerTurn(1);
        }
        if (gameManagerMultiplayer.activePlayer == 2)
        {
            gameManagerMultiplayer.PlayerTurn(2);
        }
        if (gameManagerMultiplayer.activePlayer == 3)
        {
            gameManagerMultiplayer.PlayerTurn(3);
        }
        if (gameManagerMultiplayer.activePlayer == 4)
        {
            gameManagerMultiplayer.PlayerTurn(4);
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;
            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            bool yourTurn = true;

            //Check if PlayerHandOverlay is aktiv
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name.Contains("PlayerHandOverlay"))
                {
                    yourTurn = false;
                }
            }
            foreach (RaycastResult result in results)
            {
                if (yourTurn)
                {
                    if (result.gameObject.layer == 8)
                    {
                        if (!result.gameObject.name.Contains("Background"))
                        {
                            if (move == 1)
                            {
                                if (result.gameObject.name.Contains("E") || result.gameObject.name.Contains("B") || result.gameObject.name.Contains("H") || result.gameObject.name.Contains("S"))
                                {
                                    firstLetter = result.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    move = 2;
                                    object[] movedatas = new object[] { move };
                                    PhotonNetwork.RaiseEvent(CURRENT_MOVE_EVENT, movedatas, raiseEventOptions, SendOptions.SendReliable);
                                    object[] datas = new object[] { result.gameObject.name, player, firstLetter };
                                    PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                    RemoveCard(result, player);
                                }
                            }
                            else
                            {
                                bool check = true;
                                Debug.Log(player);
                                if (player == "player1")
                                {
                                    foreach (string card in player1)
                                    {
                                        Debug.Log(card);
                                    }
                                    check = CheckList(player1, firstLetter);
                                }
                                if (player == "player2")
                                {
                                    foreach (string card in player2)
                                    {
                                        Debug.Log(card);
                                    }
                                    check = CheckList(player2, firstLetter);
                                }
                                if (player == "player3")
                                {
                                    foreach (string card in player3)
                                    {
                                        Debug.Log(card);
                                    }
                                    check = CheckList(player3, firstLetter);
                                }
                                if (player == "player4")
                                {
                                    foreach (string card in player4)
                                    {
                                        Debug.Log(card);
                                    }
                                    check = CheckList(player4, firstLetter);
                                }
                                Debug.Log(check);
                                if (!check)
                                {
                                    if (result.gameObject.name.Contains("E") || result.gameObject.name.Contains("B") || result.gameObject.name.Contains("H") || result.gameObject.name.Contains("S"))
                                    {
                                        object[] datas = new object[] { result.gameObject.name, player, firstLetter };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, player);
                                    }
                                }
                                else
                                {
                                    Debug.Log(result.gameObject.name);
                                    Debug.Log(firstLetter);
                                    Debug.Log(trumpf);
                                    if (result.gameObject.name.Contains(firstLetter) || result.gameObject.name.Contains(trumpf))
                                    {
                                        Debug.Log("inn");
                                        Debug.Log(result.gameObject.name);
                                        object[] datas = new object[] { result.gameObject.name, player, firstLetter };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, player);
                                    }
                                    else
                                    {
                                        Debug.Log("out");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void nextCard(int turn)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                int playerID = int.Parse(p.ActorNumber.ToString().Substring(0,1));
                if (playerID == turn && turn == 1)
                {
                    player = "player1";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == turn && turn == 2)
                {
                    player = "player2";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == turn && turn == 3)
                {
                    player = "player3";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == turn && turn == 4)
                {
                    player = "player4";
                    playerHandOverlay.SetActive(false);
                }
            }
        }
    }

    public void FirstPlayer(int first)
    {
        if (first == 1)
        {
            move = 1;
            object[] movedatas = new object[] { move };
            PhotonNetwork.RaiseEvent(CURRENT_MOVE_EVENT, movedatas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public void Trumpf(string newtrumpf)
    {
        object[] datas = new object[] { newtrumpf };
        PhotonNetwork.RaiseEvent(SEND_TRUMPF_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    public void GetCards()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                string playerHandCards = p.ActorNumber.ToString();
                playerHandCards.Substring(0, 1);
                for (int i = 0; i < playerHand.transform.childCount; i++)
                {
                    GameObject child = playerHand.transform.GetChild(i).gameObject;
                    string playerList = "player" + playerHandCards;
                    if (playerList == "player1")
                    {
                        player1.Add(child.name);
                    }
                    if (playerList == "player2")
                    {
                        player2.Add(child.name);
                        //object[] data = new object[] { player2.ToArray() };
                        //PhotonNetwork.RaiseEvent(SEND_PLAYERHAND_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
                    }
                    if (playerList == "player3")
                    {
                        player3.Add(child.name);
                        //object[] data = new object[] { player3.ToArray() };
                        //PhotonNetwork.RaiseEvent(SEND_PLAYERHAND_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
                    }
                    if (playerList == "player4")
                    {
                        player4.Add(child.name);
                        //object[] data = new object[] { player4.ToArray() };
                        //PhotonNetwork.RaiseEvent(SEND_PLAYERHAND_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
                    }
                }
            }
        }
        object[] datas = new object[] { true, 1 };
        PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RemoveCard(RaycastResult i, string player)
    {
        char[] removeWord = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
        if (player == "player1")
        {
            player = "None";
            player1.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 2 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player2")
        {
            Debug.Log("in");
            player = "None";
            foreach (string card in player2)
            {
                Debug.Log(card);
            }
            Debug.Log(i.gameObject.name.TrimEnd(removeWord));
            player2.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject); 
            object[] datas = new object[] { true, 3 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player3")
        {
            player = "None";
            player3.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 4 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player4")
        {
            player = "None";
            player4.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 1 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    bool CheckList(List<string> cards, string Letter)
    {
        //Check if firstLetter in List
        List<string> letterCards = new List<string>();
        Debug.Log(Letter);
        foreach (string card in cards)
        {
            Debug.Log(card.Substring(0, 1));
            if (card.Substring(0,1) == Letter)
            {
                letterCards.Add(card);
            }
        }
        if (letterCards.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == CURRENT_CARDS_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string cardName = (string)datas[0];
            string unitPlayer = (string)datas[1];
            string firstCard = (string)datas[2];
            char[] removeWord = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
            cardName = cardName.TrimEnd(removeWord);
            if (cardName.Contains("15"))
            {
                cardName = cardName.Substring(0, 1) + "9";
            }
            if (cardName.Contains("16") || cardName.Contains("U"))
            {
                cardName = cardName.Substring(0, 1) + "11";
            }
            if (cardName.Contains("O"))
            {
                cardName = cardName.Substring(0, 1) + "12";
            }
            if (cardName.Contains("K"))
            {
                cardName = cardName.Substring(0, 1) + "13";
            }
            if (cardName.Contains("A"))
            {
                cardName = cardName.Substring(0, 1) + "14";
            }
            cards = ShuffleCards.cards;
            foreach (GameObject card in cards)
            {
                if (card.name == cardName)
                {
                    card.gameObject.GetComponent<PlayCard>().SetPlayer(unitPlayer);
                    GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                    playerCard.transform.SetParent(DropZone.transform, false);
                }
            }
            firstLetter = firstCard;
        }
        if (obj.Code == SET_AKTIVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0];
            int nextPlayer = (int)datas[1];
            playerHandOverlay.SetActive(active);
            gameManagerMultiplayer.activePlayer = nextPlayer;
        }
        if (obj.Code == CURRENT_MOVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            int playerMove = (int)datas[0];
            move = playerMove;
        }
        if (obj.Code == SEND_PLAYER1HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
                player1.Add(s);
            }
        }
        if (obj.Code == SEND_PLAYER2HAND_EVENT)
        {
            Debug.Log("send");
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            Debug.Log(array);
            foreach (string s in array)
            {
                Debug.Log(s);
                player2.Add(s);
            }
        }
        if (obj.Code == SEND_PLAYER3HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
                player3.Add(s);
            }
        }
        if (obj.Code == SEND_PLAYER4HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
                player4.Add(s);
            }
        }
        if (obj.Code == SEND_TRUMPF_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string newtrumpf = (string)datas[0];
            trumpf = newtrumpf;
        }
    }
}
