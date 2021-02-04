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
    #region Public Fields

    public GameManagerMultiplayer gameManagerMultiplayer;
    public GameObject playerHand;
    public GameObject CurrentCard;
    public GameObject SecondCard;
    public GameObject ThirdCard;
    public GameObject FourthCard;
    public GameObject FifthCard;
    public GameObject SixthCard;
    public AudioSource cardLaySound;

    public Player currentPlayer;

    #endregion

    #region Private Fields

    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private GameObject playerHandOverlay;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    string playerString;
    string firstLetter;
    string trumpf;
    int move;

    List<string> player1 = new List<string>();
    List<string> player2 = new List<string>();
    List<string> player3 = new List<string>();
    List<string> player4 = new List<string>();
    List<string> player5 = new List<string>();
    List<string> player6 = new List<string>();
    List<GameObject> cards = new List<GameObject>();

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_CARDS_EVENT = 4;
    private const byte SET_AKTIVE_EVENT = 5;
    private const byte CURRENT_MOVE_EVENT = 6;
    private const byte SEND_TRUMPF_EVENT = 11;
    private const byte SEND_FIRSTCARD_EVENT = 12;
    private const byte SEND_PLAYERNUMBER_EVENT = 15;
    private const byte SEND_PLAYER1HAND_EVENT = 18;
    private const byte SEND_PLAYER2HAND_EVENT = 19;
    private const byte SEND_PLAYER3HAND_EVENT = 20;
    private const byte SEND_PLAYER4HAND_EVENT = 21;
    private const byte SEND_PLAYER5HAND_EVENT = 22;
    private const byte SEND_PLAYER6HAND_EVENT = 23;

    #endregion

    #region MonoBehaviour Callbacks

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
                gameManagerMultiplayer.PlayerTurn(currentPlayer);
            }
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
                                if (playerString == "player5")
                                {
                                    check = CheckList(player5, firstLetter, trumpf);
                                }
                                if (playerString == "player6")
                                {
                                    check = CheckList(player6, firstLetter, trumpf);
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
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    public void nextCard(Player player, int currentNumber)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                if (p.ActorNumber == player.ActorNumber)
                {
                    if(currentNumber == 1)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    if(currentNumber == 2)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    if(currentNumber == 3)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    if(currentNumber == 4)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    if(currentNumber == 5)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    if(currentNumber == 6)
                    {
                        playerString = "player" + currentNumber.ToString();
                    }
                    currentNumber++;
                    if (currentNumber == gameManagerMultiplayer.players + 1)
                    {
                        currentNumber = 1;
                    }
                    object[] datas = new object[] { currentNumber };
                    PhotonNetwork.RaiseEvent(SEND_PLAYERNUMBER_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
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
            player1.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        if (player == "player2")
        {
            player2.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        if (player == "player3")
        {
            player3.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        if (player == "player4")
        {
            player4.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        if (player == "player5")
        {
            player5.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        if (player == "player6")
        {
            player6.Remove(i.gameObject.name.TrimEnd(removeWord));
        }
        playerString = "None";
        Destroy(i.gameObject);
        object[] datas = new object[] { true, currentPlayer.GetNext() };
        PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
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

    public void ClearList()
    {
        player1.Clear();
        player2.Clear();
        player3.Clear();
        player4.Clear();
        player5.Clear();
        player6.Clear();
        cards.Clear();
        playerString = "None";
        firstLetter = "None";
        trumpf = "None";
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
            string _playerString = (string)datas[1];
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
                    cardLaySound.Play();
                    card.gameObject.GetComponent<PlayCard>().SetPlayer(_playerString);
                    GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                    playerCard.layer = 13;
                    foreach (Player player in PhotonNetwork.PlayerList)
                    {
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer)
                        {
                            if (_playerString == "player1")
                            {
                                playerCard.transform.SetParent(CurrentCard.transform, false);
                            }
                            if (_playerString == "player2")
                            {
                                playerCard.transform.SetParent(SecondCard.transform, false);
                            }
                            if (_playerString == "player3")
                            {
                                playerCard.transform.SetParent(ThirdCard.transform, false);
                            }
                            if (_playerString == "player4")
                            {
                                playerCard.transform.SetParent(FourthCard.transform, false);
                            }
                            if (_playerString == "player5")
                            {
                                playerCard.transform.SetParent(FifthCard.transform, false);
                            }
                            if (_playerString == "player6")
                            {
                                playerCard.transform.SetParent(SixthCard.transform, false);
                            }
                        }
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer.GetNext())
                        {
                            if (gameManagerMultiplayer.players == 2)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 3)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 4)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 5)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 6)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SixthCard.transform, false);
                                }
                            }
                            if (_playerString == "player2")
                            {
                                playerCard.transform.SetParent(CurrentCard.transform, false);
                            }
                            if (_playerString == "player3")
                            {
                                playerCard.transform.SetParent(SecondCard.transform, false);
                            }
                            if (_playerString == "player4")
                            {
                                playerCard.transform.SetParent(ThirdCard.transform, false);
                            }
                            if (_playerString == "player5")
                            {
                                playerCard.transform.SetParent(FourthCard.transform, false);
                            }
                            if (_playerString == "player6")
                            {
                                playerCard.transform.SetParent(FifthCard.transform, false);
                            }
                        }
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer.GetNext().GetNext())
                        {
                            if (gameManagerMultiplayer.players == 3)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 4)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 5)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player5")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 6)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(SixthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player5")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player6")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                            }
                        }
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer.GetNext().GetNext().GetNext())
                        {
                            if (gameManagerMultiplayer.players == 4)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 5)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 6)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(SixthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player5")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player6")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                            }

                        }
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer.GetNext().GetNext().GetNext().GetNext())
                        {
                            if (gameManagerMultiplayer.players == 5)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                            }
                            if (gameManagerMultiplayer.players == 6)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(SixthCard.transform, false);
                                }
                                if (_playerString == "player5")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                                if (_playerString == "player6")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                            }
                        }
                        if (player.IsLocal && player == gameManagerMultiplayer.startPlayer.GetNext().GetNext().GetNext().GetNext().GetNext())
                        {
                            if (gameManagerMultiplayer.players == 6)
                            {
                                if (_playerString == "player1")
                                {
                                    playerCard.transform.SetParent(SecondCard.transform, false);
                                }
                                if (_playerString == "player2")
                                {
                                    playerCard.transform.SetParent(ThirdCard.transform, false);
                                }
                                if (_playerString == "player3")
                                {
                                    playerCard.transform.SetParent(FourthCard.transform, false);
                                }
                                if (_playerString == "player4")
                                {
                                    playerCard.transform.SetParent(FifthCard.transform, false);
                                }
                                if (_playerString == "player5")
                                {
                                    playerCard.transform.SetParent(SixthCard.transform, false);
                                }
                                if (_playerString == "player6")
                                {
                                    playerCard.transform.SetParent(CurrentCard.transform, false);
                                }
                            }
                        }
                    }
                }
            }
        }
        if (obj.Code == SET_AKTIVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0];
            Player currentPlayer = (Player)datas[1];
            playerHandOverlay.SetActive(active);
            gameManagerMultiplayer.activePlayer = currentPlayer;
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
            Array p1 = (Array)datas[0];
            foreach (string cardname in p1)
            {
                player1.Add(cardname);
            }
        }
        if (obj.Code == SEND_PLAYER2HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array p2 = (Array)datas[0];
            foreach (string cardname in p2)
            {
                player2.Add(cardname);
            }
        }
        if (obj.Code == SEND_PLAYER3HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array p3 = (Array)datas[0];
            foreach (string cardname in p3)
            {
                player3.Add(cardname);
            }
        }
        if (obj.Code == SEND_PLAYER4HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array p4 = (Array)datas[0];
            foreach (string cardname in p4)
            {
                player4.Add(cardname);
            }
        }
        if (obj.Code == SEND_PLAYER5HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array p5 = (Array)datas[0];
            foreach (string cardname in p5)
            {
                player5.Add(cardname);
            }
        }
        if (obj.Code == SEND_PLAYER6HAND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Array p6 = (Array)datas[0];
            foreach (string cardname in p6)
            {
                player6.Add(cardname);
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
        if (obj.Code == SEND_PLAYERNUMBER_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            int playerNumber = (int)datas[0];
            gameManagerMultiplayer.playerNumber = playerNumber;
        }
    }
}
