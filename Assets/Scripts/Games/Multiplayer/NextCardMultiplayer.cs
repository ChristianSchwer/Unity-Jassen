using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

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

    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();
    List<GameObject> cards = new List<GameObject>();

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_CARDS_EVENT = 4;
    private const byte SET_AKTIVE_EVENT = 5;

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
                                    object[] datas = new object[] { result.gameObject.name };
                                    PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                    firstLetter = result.gameObject.GetComponent<PlayCard>().unitName.Substring(0, 1);
                                    move = 2;
                                    RemoveCard(result, player);
                                }
                            }
                            else
                            {
                                bool check = CheckList(player1, firstLetter);
                                if (check == false)
                                {
                                    if (result.gameObject.name.Contains("E") || result.gameObject.name.Contains("B") || result.gameObject.name.Contains("H") || result.gameObject.name.Contains("S"))
                                    {
                                        object[] datas = new object[] { result.gameObject.name };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, player);
                                    }
                                }
                                else
                                {
                                    if (result.gameObject.name.Contains(firstLetter) || result.gameObject.name.Contains(trumpf))
                                    {
                                        object[] datas = new object[] { result.gameObject.name };
                                        PhotonNetwork.RaiseEvent(CURRENT_CARDS_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
                                        RemoveCard(result, player);
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
        }
    }

    public void Trumpf(string newtrumpf)
    {
        trumpf = newtrumpf;
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
                        player1.Add(child);
                    }
                    if (playerList == "player2")
                    {
                        player2.Add(child);
                    }
                    if (playerList == "player3")
                    {
                        player3.Add(child);
                    }
                    if (playerList == "player4")
                    {
                        player4.Add(child);
                    }
                }
            }
        }
        object[] datas = new object[] { true, 1 };
        PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    private void RemoveCard(RaycastResult i, string player)
    {
        if (player == "player1")
        {
            player = "None";
            player1.Remove(i.gameObject);
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 2 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player2")
        {
            player = "None";
            player2.Remove(i.gameObject);
            Destroy(i.gameObject); 
            object[] datas = new object[] { true, 3 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player3")
        {
            player = "None";
            player3.Remove(i.gameObject);
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 4 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (player == "player4")
        {
            player = "None";
            player4.Remove(i.gameObject);
            Destroy(i.gameObject);
            object[] datas = new object[] { true, 1 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    bool CheckList(List<GameObject> cards, string Letter)
    {
        //Check if firstLetter in List
        List<GameObject> letterCards = new List<GameObject>();
        foreach (GameObject card in cards)
        {
            if (card.GetComponent<PlayCard>().unitName.Substring(0, 1) == Letter)
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
                    GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                    playerCard.transform.SetParent(DropZone.transform, false);
                }
            }                
        }
        if (obj.Code == SET_AKTIVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0];
            int nextPlayer = (int)datas[1];
            playerHandOverlay.SetActive(active);
            gameManagerMultiplayer.activePlayer = nextPlayer;
        }
    }
}
