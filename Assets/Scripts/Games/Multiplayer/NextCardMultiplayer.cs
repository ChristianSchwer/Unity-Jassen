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

    string playerString;
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
    private const byte SEND_FIRSTCARD_EVENT = 12;

    Player currentPlayer;

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
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (gameManagerMultiplayer.activePlayer == player)
            {
                currentPlayer = gameManagerMultiplayer.activePlayer;
                gameManagerMultiplayer.PlayerTurn(player);
            }
            //if (gameManagerMultiplayer.activePlayer == 2)
            //{
            //    gameManagerMultiplayer.PlayerTurn(2);
            //}
            //if (gameManagerMultiplayer.activePlayer == 3)
            //{
            //    gameManagerMultiplayer.PlayerTurn(3);
            //}
            //if (gameManagerMultiplayer.activePlayer == 4)
            //{
            //    gameManagerMultiplayer.PlayerTurn(4);
            //}
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
                                    object[] firstcarddatas = new object[] { firstLetter };
                                    PhotonNetwork.RaiseEvent(SEND_FIRSTCARD_EVENT, firstcarddatas, raiseEventOptions, SendOptions.SendReliable);
                                    object[] movedatas = new object[] { move };
                                    PhotonNetwork.RaiseEvent(CURRENT_MOVE_EVENT, movedatas, raiseEventOptions, SendOptions.SendReliable);
                                    object[] datas = new object[] { result.gameObject.name, playerString };
                                    PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                    RemoveCard(result, playerString);
                                }
                            }
                            else
                            {
                                bool check = true;
                                if (playerString == "player1")
                                {
                                    check = CheckList(player1, firstLetter, trumpf);
                                }
                                if (playerString == "player2")
                                {
                                    check = CheckList(player2, firstLetter, trumpf);
                                }
                                if (playerString == "player3")
                                {
                                    check = CheckList(player3, firstLetter, trumpf);
                                }
                                if (playerString == "player4")
                                {
                                    check = CheckList(player4, firstLetter, trumpf);
                                }
                                if (!check)
                                {
                                    if (result.gameObject.name.Contains("E") || result.gameObject.name.Contains("B") || result.gameObject.name.Contains("H") || result.gameObject.name.Contains("S"))
                                    {
                                        object[] datas = new object[] { result.gameObject.name, playerString };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, playerString);
                                    }
                                }
                                else
                                {
                                    if (result.gameObject.name.Contains(firstLetter) || result.gameObject.name.Contains(trumpf))
                                    {
                                        object[] datas = new object[] { result.gameObject.name, playerString };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, playerString);
                                    }
                                    //else
                                    //{
                                    //    Debug.Log("out");
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void nextCard(Player player)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                int playerID = int.Parse(p.ActorNumber.ToString().Substring(0,1));
                if (playerID == player.ActorNumber && player.ActorNumber == 1)
                {
                    playerString = "player1";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == player.ActorNumber && player.ActorNumber == 2)
                {
                    playerString = "player2";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == player.ActorNumber && player.ActorNumber == 3)
                {
                    playerString = "player3";
                    playerHandOverlay.SetActive(false);
                }
                if (playerID == player.ActorNumber && player.ActorNumber == 4)
                {
                    playerString = "player4";
                    playerHandOverlay.SetActive(false);
                }
            }
        }
    }

    public void FirstPlayer(int first)
    {
        if (first == 1)
        {
            object[] movedatas = new object[] { first };
            PhotonNetwork.RaiseEvent(CURRENT_MOVE_EVENT, movedatas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public void Trumpf(string newtrumpf)
    {
        object[] datas = new object[] { newtrumpf };
        PhotonNetwork.RaiseEvent(SEND_TRUMPF_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RemoveCard(RaycastResult i, string player)
    {
        char[] removeWord = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
        if (player == "player1")
        {
            player = "None";
            player1.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, currentPlayer.GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player2")
        {
            player = "None";
            player2.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, currentPlayer.GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player3")
        {
            player = "None";
            player3.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, currentPlayer.GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player4")
        {
            player = "None";
            player4.Remove(i.gameObject.name.TrimEnd(removeWord));
            Destroy(i.gameObject);
            object[] datas = new object[] { true, currentPlayer.GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    bool CheckList(List<string> cards, string Letter, string currentTrumpf)
    {
        //Check if firstLetter in List
        List<string> letterCards = new List<string>();
        foreach (string card in cards)
        {
            if (card.Substring(0,1) == Letter)
            {
                letterCards.Add(card);
            }
        }
        if (firstLetter == currentTrumpf)
        {
            if (letterCards.Count == 1)
            {
                foreach (string card in letterCards)
                {
                    if (card.Contains("11"))
                    {
                        return false;
                    }
                }
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
                    playerCard.layer = 13;
                    playerCard.transform.SetParent(DropZone.transform, false);
                }
            }
        }
        if (obj.Code == SET_AKTIVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0];
            Player nextPlayer = (Player)datas[1];
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
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
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
        if (obj.Code == SEND_FIRSTCARD_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string firstCard = (string)datas[0];
            firstLetter = firstCard;
        }
    }
}
