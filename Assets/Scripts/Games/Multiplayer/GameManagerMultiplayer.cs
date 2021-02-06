using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerMultiplayer : MonoBehaviourPun
{
    #region Public Fields

    public static int start;
    public int playerNumber;
    public int players;
    public Player activePlayer;
    public Player startPlayer;
    public Player lastPlayer;
    public AudioSource youShouldLaySound;

    #endregion

    #region Private Textfields

    [SerializeField]
    private Text currentPlayer;
    [SerializeField]
    private Text Score1;
    [SerializeField]
    private Text Score2;
    [SerializeField]
    private Text Score3;
    [SerializeField]
    private Text Score4;
    [SerializeField]
    private Text Score5;
    [SerializeField]
    private Text Score6;
    [SerializeField]
    private Text Player1;
    [SerializeField]
    private Text Player2;
    [SerializeField]
    private Text Player3;
    [SerializeField]
    private Text Player4;
    [SerializeField]
    private Text Player5;
    [SerializeField]
    private Text Player6;
    [SerializeField]
    private Text Round;

    #endregion

    #region Private GameObjects

    [SerializeField]
    private GameObject playerHand;
    [SerializeField]
    private GameObject currentCard;
    [SerializeField]
    private GameObject secondCard;
    [SerializeField]
    private GameObject thirdCard;
    [SerializeField]
    private GameObject fourthCard;
    [SerializeField]
    private GameObject fifthCard;
    [SerializeField]
    private GameObject sixthCard;
    [SerializeField]
    private GameObject yard;
    [SerializeField]
    private GameObject scoreBoard;
    [SerializeField]
    private GameObject TrumpfE;
    [SerializeField]
    private GameObject TrumpfS;
    [SerializeField]
    private GameObject TrumpfH;
    [SerializeField]
    private GameObject TrumpfB;

    #endregion

    #region Private Fields

    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private NextCardMultiplayer NextCard;

    List<GameObject> dropzonecards = new List<GameObject>();
    List<GameObject> trumpfcards = new List<GameObject>();
    List<GameObject> othercards = new List<GameObject>();
    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();
    List<GameObject> player5 = new List<GameObject>();
    List<GameObject> player6 = new List<GameObject>();

    string trumpfUnit;
    private int round;
    private int cardCount;

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_PLAYER_EVENT = 2;
    private const byte TRUMPF_EVENT = 3;
    private const byte SET_AKTIVE_EVENT = 5;
    private const byte SET_PLAYERCOUNT_EVENT = 7;
    private const byte RESET_VARIABELS_EVENT = 9;
    private const byte SET_STARTPLAYER_EVENT = 10;
    private const byte SEND_SCORE_EVENT= 13;
    private const byte DEACTIVATE_SCOREBOARD_EVENT= 14;
    private const byte SEND_PLAYERNUMBER_EVENT = 15;
    private const byte SEND_ROUND_EVENT = 16;
    private const byte SET_TRUMPF_AKTIVE_EVENT = 17;

    PlayCard currentUnit;

    #endregion

    #region MonoBehaviour Callbacks

    void Update()
    {
        //Game starts when Player hit "Start Game"-Button in the PlayerListingsMenu
        if (start == 1)
        {
            //Deactivate the scoreboard when activ
            object[] datas = new object[] { false };
            PhotonNetwork.RaiseEvent(DEACTIVATE_SCOREBOARD_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
            start = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                SetupGame();
            }
        }
    }

    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //Get player count from current room
            if (player.IsMasterClient)
            {
                players = PhotonNetwork.CurrentRoom.PlayerCount;
            }
            //Set playernickname on the card table for every player
            if (player.IsLocal)
            {
                if (players == 2)
                {
                    Player1.text = player.NickName;
                    Player2.text = player.GetNext().NickName;
                    Player3.text = "";
                    Player4.text = "";
                    Player5.text = "";
                    Player6.text = "";
                }
                if (players == 3)
                {
                    Player1.text = player.NickName;
                    Player2.text = player.GetNext().NickName;
                    Player3.text = player.GetNext().GetNext().NickName;
                    Player4.text = "";
                    Player5.text = "";
                    Player6.text = "";
                }
                if (players == 4)
                {
                    Player1.text = player.NickName;
                    Player2.text = player.GetNext().NickName;
                    Player3.text = player.GetNext().GetNext().NickName;
                    Player4.text = player.GetNext().GetNext().GetNext().NickName;
                    Player5.text = "";
                    Player6.text = "";
                }
                if (players == 5)
                {
                    Player1.text = player.NickName;
                    Player2.text = player.GetNext().NickName;
                    Player5.text = player.GetNext().GetNext().NickName;
                    Player3.text = player.GetNext().GetNext().GetNext().NickName;
                    Player4.text = player.GetNext().GetNext().GetNext().GetNext().NickName;
                    Player6.text = "";
                }
                if (players == 6)
                {
                    Player1.text = player.NickName;
                    Player2.text = player.GetNext().NickName;
                    Player5.text = player.GetNext().GetNext().NickName;
                    Player3.text = player.GetNext().GetNext().GetNext().NickName;
                    Player4.text = player.GetNext().GetNext().GetNext().GetNext().NickName;
                    Player6.text = player.GetNext().GetNext().GetNext().GetNext().GetNext().NickName;
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void OnClick_YouShouldLay()
    {
        if (NextCard.currentPlayer.IsLocal)
        {
            youShouldLaySound.Play();
        }
    }

    public void PlayerTurn(Player p, int playerCount)
    {
        players = playerCount;
        activePlayer = null;
        if (GetChildCount() == players)
        {
            round++;
            object[] datas = new object[] { round };
            PhotonNetwork.RaiseEvent(SEND_ROUND_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
            if (PhotonNetwork.IsMasterClient)
            {
                lastPlayer = p;
                CalculateRoundWinner();
            }
            else
            {
                StartCoroutine(ClearDropZone());
            }
        }
        else
        {
            CurrentPlayer(p.ActorNumber);
            NextCard.nextCard(p, playerNumber);
        }
        if (playerHand.transform.childCount == 0 && GetChildCount() == 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CalculateWinner();
            }
        }
    }

    public void CurrentPlayer(int i)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == i)
            {
                object[] datas = new object[] { p.NickName.ToString() };
                PhotonNetwork.RaiseEvent(CURRENT_PLAYER_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
            }
        }
    }

    public void GetTrumpf(List<GameObject> cards)
    {
        //Set trumpf
        currentUnit = cards[35].GetComponent<PlayCard>();
        trumpfUnit = currentUnit.unitName.Substring(0, 1);
        //Send trumpf to all players;
        object[] datas = new object[] { trumpfUnit };
        PhotonNetwork.RaiseEvent(TRUMPF_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        foreach (GameObject card in cards)
        {
            if (card.gameObject.name.Contains(trumpfUnit + "09"))
            {
                currentUnit = card.GetComponent<PlayCard>();
                currentUnit.ChangeSnell(trumpfUnit);
            }
            if (card.gameObject.name.Contains(trumpfUnit + 11))
            {
                currentUnit = card.GetComponent<PlayCard>();
                currentUnit.ChangeBauer(trumpfUnit);
            }
        }
        NextCard.Trumpf(trumpfUnit);
    }

    #endregion

    #region Private Methods

    void SetupGame()
    {
        //Set number of players
        object[] numberOfPlayers = new object[] { players };
        PhotonNetwork.RaiseEvent(SET_PLAYERCOUNT_EVENT, numberOfPlayers, raiseEventOptions, SendOptions.SendReliable);
        //Reset Variables
        object[] resetData = new object[] { true };
        PhotonNetwork.RaiseEvent(RESET_VARIABELS_EVENT, resetData, raiseEventOptions, SendOptions.SendReliable);
        //Set start player
        if (startPlayer == null)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.IsMasterClient)
                {
                    startPlayer = player;
                }
            }
        }
        object[] startPlayerData = new object[] { startPlayer };
        PhotonNetwork.RaiseEvent(SET_STARTPLAYER_EVENT, startPlayerData, raiseEventOptions, SendOptions.SendReliable);
        //Set player number
        playerNumber = 1;
        object[] data = new object[] { playerNumber };
        PhotonNetwork.RaiseEvent(SEND_PLAYERNUMBER_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
        //Shuffle cards
        ShuffleCards.CardShuffle(players);
        //Set round counter
        round = 1;
        object[] datas = new object[] { round };
        PhotonNetwork.RaiseEvent(SEND_ROUND_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        //Make all trumpf icons invisible
        object[] trumpfData = new object[] { false };
        PhotonNetwork.RaiseEvent(SET_TRUMPF_AKTIVE_EVENT, trumpfData, raiseEventOptions, SendOptions.SendReliable);
        //Send current player
        object[] startData = new object[] { true, startPlayer };
        PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, startData, raiseEventOptions, SendOptions.SendReliable);
    }

    int GetChildCount()
    {
        cardCount = 0;
        if (players == 2)
        {
            cardCount += currentCard.transform.childCount;
            cardCount += secondCard.transform.childCount;
        }
        if (players == 3)
        {
            cardCount += currentCard.transform.childCount;
            cardCount += secondCard.transform.childCount;
            cardCount += thirdCard.transform.childCount;
        }
        if (players == 4)
        {
            cardCount += currentCard.transform.childCount;
            cardCount += secondCard.transform.childCount;
            cardCount += thirdCard.transform.childCount;
            cardCount += fourthCard.transform.childCount;
        }
        if (players == 5)
        {
            cardCount += currentCard.transform.childCount;
            cardCount += secondCard.transform.childCount;
            cardCount += thirdCard.transform.childCount;
            cardCount += fourthCard.transform.childCount;
            cardCount += fifthCard.transform.childCount;
        }
        if (players == 6)
        {
            cardCount += currentCard.transform.childCount;
            cardCount += secondCard.transform.childCount;
            cardCount += thirdCard.transform.childCount;
            cardCount += fourthCard.transform.childCount;
            cardCount += fifthCard.transform.childCount;
            cardCount += sixthCard.transform.childCount;
        }
        return cardCount;
    }

    private ArrayList GetChilds()
    {
        ArrayList childs = new ArrayList();
        if (players == 2)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            childs.Add(child1);
            childs.Add(child2);
            return childs;
        }
        if (players == 3)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            GameObject child3 = thirdCard.transform.GetChild(0).gameObject;
            childs.Add(child1);
            childs.Add(child2);
            childs.Add(child3);
            return childs;
        }
        if (players == 4)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            GameObject child3 = thirdCard.transform.GetChild(0).gameObject;
            GameObject child4 = fourthCard.transform.GetChild(0).gameObject;
            childs.Add(child1);
            childs.Add(child2);
            childs.Add(child3);
            childs.Add(child4);
            return childs;
        }
        if (players == 5)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            GameObject child3 = fifthCard.transform.GetChild(0).gameObject;
            GameObject child4 = thirdCard.transform.GetChild(0).gameObject;
            GameObject child5 = fourthCard.transform.GetChild(0).gameObject;
            childs.Add(child1);
            childs.Add(child2);
            childs.Add(child3);
            childs.Add(child4);
            childs.Add(child5);
            return childs;
        }
        if (players == 6)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            GameObject child3 = fifthCard.transform.GetChild(0).gameObject;
            GameObject child4 = thirdCard.transform.GetChild(0).gameObject;
            GameObject child5 = fourthCard.transform.GetChild(0).gameObject;
            GameObject child6 = sixthCard.transform.GetChild(0).gameObject;
            childs.Add(child1);
            childs.Add(child2);
            childs.Add(child3);
            childs.Add(child4);
            childs.Add(child5);
            childs.Add(child6);
            return childs;
        }
        return null;
    }

    void CalculateRoundWinner()
    {
        if (GetChildCount() == players)
        {
            GameObject child1 = null;
            GameObject child2 = null;
            GameObject child3 = null;
            GameObject child4 = null;
            GameObject child5 = null;
            GameObject child6 = null;
            ArrayList childs = new ArrayList();
            //Get current GameObjects from the desk and store them in a List
            childs = GetChilds();
            for (int i = 0; i < players; i++)
            {
                if (i == 0)
                {
                    child1 = childs[i] as GameObject;
                    dropzonecards.Add(child1);
                }
                if (i == 1)
                {
                    child2 = childs[i] as GameObject;
                    dropzonecards.Add(child2);
                }
                if (i == 2)
                {
                    child3 = childs[i] as GameObject;
                    dropzonecards.Add(child3);
                }
                if (i == 3)
                {
                    child4 = childs[i] as GameObject;
                    dropzonecards.Add(child4);
                }
                if (i == 4)
                {
                    child5 = childs[i] as GameObject;
                    dropzonecards.Add(child5);
                }
                if (i == 5)
                {
                    child6 = childs[i] as GameObject;
                    dropzonecards.Add(child6);
                }
            }
            //Check which is the first card
            string firstcard = null;
            if (lastPlayer.ActorNumber == 1)
            {
                firstcard = child1.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            if (lastPlayer.ActorNumber == 2)
            {
                firstcard = child2.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            if (lastPlayer.ActorNumber == 3)
            {
                firstcard = child3.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            if (lastPlayer.ActorNumber == 4)
            {
                firstcard = child4.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            if (lastPlayer.ActorNumber == 5)
            {
                firstcard = child5.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            if (lastPlayer.ActorNumber == 6)
            {
                firstcard = child6.GetComponent<PlayCard>().unitName.Substring(0, 1);
            }
            //Calculate winner
            foreach (GameObject card in dropzonecards)
            {
                if (card.gameObject.name.Contains(trumpfUnit))
                {
                    trumpfcards.Add(card);
                }
            }
            if (trumpfcards.Count == 1)
            {
                foreach (GameObject card in trumpfcards)
                {
                    string winner = card.GetComponent<PlayCard>().unitPlayer;
                    Winner(winner);
                }
            }
            if (trumpfcards.Count > 1)
            {
                int highestCard = 0;
                foreach (GameObject card in trumpfcards)
                {
                    if (card.GetComponent<PlayCard>().unitStrength > highestCard)
                    {
                        highestCard = card.GetComponent<PlayCard>().unitStrength;
                    }
                }
                foreach (GameObject card in trumpfcards)
                {
                    if (card.GetComponent<PlayCard>().unitStrength == highestCard)
                    {
                        string winner = card.GetComponent<PlayCard>().unitPlayer;
                        Winner(winner);
                    }
                }
            }
            if (trumpfcards.Count == 0)
            {
                foreach (GameObject card in dropzonecards)
                {
                    if (card.gameObject.name.Contains(firstcard))
                    {
                        othercards.Add(card);
                    }
                }
                if (othercards.Count == 1)
                {
                    foreach (GameObject card in othercards)
                    {
                        string winner = card.GetComponent<PlayCard>().unitPlayer;
                        Winner(winner);
                    }
                }
                if (othercards.Count > 1)
                {
                    int highestCard = 0;
                    foreach (GameObject card in othercards)
                    {
                        if (card.GetComponent<PlayCard>().unitStrength > highestCard)
                        {
                            highestCard = card.GetComponent<PlayCard>().unitStrength;
                        }
                    }
                    foreach (GameObject card in othercards)
                    {
                        if (card.GetComponent<PlayCard>().unitStrength == highestCard)
                        {
                            string winner = card.GetComponent<PlayCard>().unitPlayer;
                            Winner(winner);
                        }
                    }
                }
            }
        }
    }

    void Winner(string winner)
    {
        GameObject child1 = null;
        GameObject child2 = null;
        GameObject child3 = null;
        GameObject child4 = null;
        GameObject child5 = null;
        GameObject child6 = null;
        ArrayList childs = new ArrayList();
        //Get current GameObjects from the desk and store them in a List
        childs = GetChilds();
        for (int i = 0; i < players; i++)
        {
            if (i == 0)
            {
                child1 = childs[i] as GameObject;
            }
            if (i == 1)
            {
                child2 = childs[i] as GameObject;
            }
            if (i == 2)
            {
                child3 = childs[i] as GameObject;
            }
            if (i == 3)
            {
                child4 = childs[i] as GameObject;
            }
            if (i == 4)
            {
                child5 = childs[i] as GameObject;
            }
            if (i == 5)
            {
                child6 = childs[i] as GameObject;
            }
        }
        if (players == 2)
        {
            if (winner == "player1")
            {
                player1.Add(child1);
                player1.Add(child2);
            }
            if (winner == "player2")
            {
                player2.Add(child1);
                player2.Add(child2);
            }
        }
        if (players == 3)
        {
            if (winner == "player1")
            {
                player1.Add(child1);
                player1.Add(child2);
                player1.Add(child3);
            }
            if (winner == "player2")
            {
                player2.Add(child1);
                player2.Add(child2);
                player2.Add(child3);
            }
            if (winner == "player3")
            {
                player3.Add(child1);
                player3.Add(child2);
                player3.Add(child3);
            }
        }
        if (players == 4)
        {
            if (winner == "player1")
            {
                player1.Add(child1);
                player1.Add(child2);
                player1.Add(child3);
                player1.Add(child4);
            }
            if (winner == "player2")
            {
                player2.Add(child1);
                player2.Add(child2);
                player2.Add(child3);
                player2.Add(child4);
            }
            if (winner == "player3")
            {
                player3.Add(child1);
                player3.Add(child2);
                player3.Add(child3);
                player3.Add(child4);
            }
            if (winner == "player4")
            {
                player4.Add(child1);
                player4.Add(child2);
                player4.Add(child3);
                player4.Add(child4);
            }
        }
        if (players == 5)
        {
            if (winner == "player1")
            {
                player1.Add(child1);
                player1.Add(child2);
                player1.Add(child3);
                player1.Add(child4);
                player1.Add(child5);
            }
            if (winner == "player2")
            {
                player2.Add(child1);
                player2.Add(child2);
                player2.Add(child3);
                player2.Add(child4);
                player2.Add(child5);
            }
            if (winner == "player3")
            {
                player3.Add(child1);
                player3.Add(child2);
                player3.Add(child3);
                player3.Add(child4);
                player3.Add(child5);
            }
            if (winner == "player4")
            {
                player4.Add(child1);
                player4.Add(child2);
                player4.Add(child3);
                player4.Add(child4);
                player4.Add(child5);
            }
            if (winner == "player5")
            {
                player5.Add(child1);
                player5.Add(child2);
                player5.Add(child3);
                player5.Add(child4);
                player5.Add(child5);
            }
        }
        if (players == 6)
        {
            if (winner == "player1")
            {
                player1.Add(child1);
                player1.Add(child2);
                player1.Add(child3);
                player1.Add(child4);
                player1.Add(child5);
                player1.Add(child6);
            }
            if (winner == "player2")
            {
                player2.Add(child1);
                player2.Add(child2);
                player2.Add(child3);
                player2.Add(child4);
                player2.Add(child5);
                player2.Add(child6);
            }
            if (winner == "player3")
            {
                player3.Add(child1);
                player3.Add(child2);
                player3.Add(child3);
                player3.Add(child4);
                player3.Add(child5);
                player3.Add(child6);
            }
            if (winner == "player4")
            {
                player4.Add(child1);
                player4.Add(child2);
                player4.Add(child3);
                player4.Add(child4);
                player4.Add(child5);
                player4.Add(child6);
            }
            if (winner == "player5")
            {
                player5.Add(child1);
                player5.Add(child2);
                player5.Add(child3);
                player5.Add(child4);
                player5.Add(child5);
                player5.Add(child6);
            }
            if (winner == "player6")
            {
                player6.Add(child1);
                player6.Add(child2);
                player6.Add(child3);
                player6.Add(child4);
                player6.Add(child5);
                player6.Add(child6);
            }
        }
        StartCoroutine(Wait(winner));
    }

    IEnumerator Wait(string winner)
    {
        yield return new WaitForSeconds(3f);
        //clear all cards frome the dropzone
        dropzonecards.Clear();
        trumpfcards.Clear();
        othercards.Clear();
        if (players == 2)
        {
            currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        if (players == 3)
        {
            currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            thirdCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        if (players == 4)
        {
            currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            thirdCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            fourthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        if (players == 5)
        {
            currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            fifthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            thirdCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            fourthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        if (players == 6)
        {
            currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            fifthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            thirdCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            fourthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
            sixthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        for (int i = 0; i < yard.transform.childCount; i++)
        {
            yard.transform.GetChild(i).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.25f);
        if (winner == "player1")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 1;
            object[] datas = new object[] { true, startPlayer };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (winner == "player2")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 2;
            object[] datas = new object[] { true, startPlayer.GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (winner == "player3")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 3;
            object[] datas = new object[] { true, startPlayer.GetNext().GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (winner == "player4")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 4;
            object[] datas = new object[] { true, startPlayer.GetNext().GetNext().GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (winner == "player5")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 5;
            object[] datas = new object[] { true, startPlayer.GetNext().GetNext().GetNext().GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (winner == "player6")
        {
            NextCard.FirstPlayer(1);
            playerNumber = 6;
            object[] datas = new object[] { true, startPlayer.GetNext().GetNext().GetNext().GetNext().GetNext() };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        object[] data = new object[] { playerNumber };
        PhotonNetwork.RaiseEvent(SEND_PLAYERNUMBER_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
    }

    void CalculateWinner()
    {
        int yard1 = 0;
        int yard2 = 0;
        int yard3 = 0;
        int yard4 = 0;
        int yard5 = 0;
        int yard6 = 0;
        foreach (GameObject card in player1)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard1 += currentUnit.unitValue;
        }
        foreach (GameObject card in player2)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard2 += currentUnit.unitValue;
        }
        foreach (GameObject card in player3)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard3 += currentUnit.unitValue;
        }
        foreach (GameObject card in player4)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard4 += currentUnit.unitValue;
        }
        foreach (GameObject card in player5)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard5 += currentUnit.unitValue;
        }
        foreach (GameObject card in player6)
        {
            currentUnit = card.GetComponent<PlayCard>();
            yard6 += currentUnit.unitValue;
        }
        object[] datas = new object[] { yard1.ToString(), yard2.ToString(), yard3.ToString(), yard4.ToString(), yard5.ToString(), yard6.ToString()};
        PhotonNetwork.RaiseEvent(SEND_SCORE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    IEnumerator ClearDropZone()
    {
        yield return new WaitForSeconds(3f);
        if (players == 2)
        {
            Destroy(currentCard.transform.GetChild(0).gameObject);
            Destroy(secondCard.transform.GetChild(0).gameObject);
        }
        if (players == 3)
        {
            Destroy(currentCard.transform.GetChild(0).gameObject);
            Destroy(secondCard.transform.GetChild(0).gameObject);
            Destroy(thirdCard.transform.GetChild(0).gameObject);
        }
        if (players == 4)
        {
            Destroy(currentCard.transform.GetChild(0).gameObject);
            Destroy(secondCard.transform.GetChild(0).gameObject);
            Destroy(thirdCard.transform.GetChild(0).gameObject);
            Destroy(fourthCard.transform.GetChild(0).gameObject);
        }
        if (players == 5)
        {
            Destroy(currentCard.transform.GetChild(0).gameObject);
            Destroy(secondCard.transform.GetChild(0).gameObject);
            Destroy(thirdCard.transform.GetChild(0).gameObject);
            Destroy(fourthCard.transform.GetChild(0).gameObject);
            Destroy(fifthCard.transform.GetChild(0).gameObject);
        }
        if (players == 6)
        {
            Destroy(currentCard.transform.GetChild(0).gameObject);
            Destroy(secondCard.transform.GetChild(0).gameObject);
            Destroy(thirdCard.transform.GetChild(0).gameObject);
            Destroy(fourthCard.transform.GetChild(0).gameObject);
            Destroy(fifthCard.transform.GetChild(0).gameObject);
            Destroy(sixthCard.transform.GetChild(0).gameObject);
        }
    }

    void EndGame()
    {
        //Set scoreboard active
        scoreBoard.SetActive(true);
        //Enable "new game button" only for master client 
        if (!PhotonNetwork.IsMasterClient)
        {
            if (scoreBoard.GetComponentInChildren<Button>() == true)
            {
                Button newGameButton = scoreBoard.GetComponentInChildren<Button>();
                newGameButton.gameObject.SetActive(false);
            }
        }
        //Set start player to the next player
        startPlayer = startPlayer.GetNext();
        //Clear Variables
        ClearVariables();
    }

    void ClearVariables()
    {
        player1.Clear();
        player2.Clear();
        player3.Clear();
        player4.Clear();
        player5.Clear();
        player6.Clear();
        ShuffleCards.ClearList();
        NextCard.ClearList();
        activePlayer = null;
        playerNumber = 1;
        trumpfUnit = null;
        round = 0;
        cardCount = 0;
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
        if (obj.Code == RESET_VARIABELS_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool reset = (bool)datas[0];
            if (reset)
            {
                ClearVariables();
            }
        }
        if (obj.Code == SET_STARTPLAYER_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            Player sPlayer = (Player)datas[0];
            startPlayer = sPlayer;
        }
        if (obj.Code == SET_PLAYERCOUNT_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            int playerCount = (int)datas[0];
            players = playerCount;
        }
        if (obj.Code == CURRENT_PLAYER_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string p = (string)datas[0];
            Player1.color = Color.black;
            Player2.color = Color.black;
            Player3.color = Color.black;
            Player4.color = Color.black;
            Player5.color = Color.black;
            Player6.color = Color.black;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if(player.IsLocal)
                {
                    if (players == 2)
                    {
                        if (Player1.text == p)
                        {
                            Player1.color = Color.red;
                        }
                        if (Player2.text == p)
                        {
                            Player2.color = Color.red;
                        }
                    }
                    if (players == 3)
                    {
                        if (Player1.text == p)
                        {
                            Player1.color = Color.red;
                        }
                        if (Player2.text == p)
                        {
                            Player2.color = Color.red;
                        }
                        if (Player3.text == p)
                        {
                            Player3.color = Color.red;
                        }
                    }
                    if (players == 4)
                    {
                        if (Player1.text == p)
                        {
                            Player1.color = Color.red;
                        }
                        if (Player2.text == p)
                        {
                            Player2.color = Color.red;
                        }
                        if (Player3.text == p)
                        {
                            Player3.color = Color.red;
                        }
                        if (Player4.text == p)
                        {
                            Player4.color = Color.red;
                        }
                    }
                    if (players == 5)
                    {
                        if (Player1.text == p)
                        {
                            Player1.color = Color.red;
                        }
                        if (Player2.text == p)
                        {
                            Player2.color = Color.red;
                        }
                        if (Player3.text == p)
                        {
                            Player3.color = Color.red;
                        }
                        if (Player4.text == p)
                        {
                            Player4.color = Color.red;
                        }
                        if (Player5.text == p)
                        {
                            Player5.color = Color.red;
                        }
                    }
                    if (players == 6)
                    {
                        if (Player1.text == p)
                        {
                            Player1.color = Color.red;
                        }
                        if (Player2.text == p)
                        {
                            Player2.color = Color.red;
                        }
                        if (Player3.text == p)
                        {
                            Player3.color = Color.red;
                        }
                        if (Player4.text == p)
                        {
                            Player4.color = Color.red;
                        }
                        if (Player5.text == p)
                        {
                            Player5.color = Color.red;
                        }
                        if (Player6.text == p)
                        {
                            Player6.color = Color.red;
                        }
                    }
                }
            }
            currentPlayer.text = "Am Zug: " + p;
        }
        if (obj.Code == TRUMPF_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string t = (string)datas[0];
            if (t == "E")
            {
                TrumpfE.SetActive(true);
            }
            if (t == "S")
            {
                TrumpfS.SetActive(true);
            }
            if (t == "H")
            {
                TrumpfH.SetActive(true);
            }
            if (t == "B")
            {
                TrumpfB.SetActive(true);
            }
        }
        if (obj.Code == SEND_SCORE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string score1 = (string)datas[0];
            string score2 = (string)datas[1];
            string score3 = (string)datas[2];
            string score4 = (string)datas[3];
            string score5 = (string)datas[4];
            string score6 = (string)datas[5];
            if (players == 2)
            {
                Score1.text = startPlayer.NickName + ": " + score1;
                Score2.text = startPlayer.GetNext().NickName + ": " + score2;
                Score3.text = "";
                Score4.text = "";
                Score5.text = "";
                Score6.text = "";
            }
            if (players == 3)
            {
                Score1.text = startPlayer.NickName + ": " + score1;
                Score2.text = startPlayer.GetNext().NickName + ": " + score2;
                Score3.text = startPlayer.GetNext().GetNext().NickName + ": " + score3;
                Score4.text = "";
                Score5.text = "";
                Score6.text = "";
            }
            if (players == 4)
            {
                Score1.text = startPlayer.NickName + ": " + score1;
                Score2.text = startPlayer.GetNext().NickName + ": " + score2;
                Score3.text = startPlayer.GetNext().GetNext().NickName + ": " + score3;
                Score4.text = startPlayer.GetNext().GetNext().GetNext().NickName + ": " + score4;
                Score5.text = "";
                Score6.text = "";
            }
            if (players == 5)
            {
                Score1.text = startPlayer.NickName + ": " + score1;
                Score2.text = startPlayer.GetNext().NickName + ": " + score2;
                Score3.text = startPlayer.GetNext().GetNext().NickName + ": " + score3;
                Score4.text = startPlayer.GetNext().GetNext().GetNext().NickName + ": " + score4;
                Score5.text = startPlayer.GetNext().GetNext().GetNext().GetNext().NickName + ": " + score5;
                Score6.text = "";
            }
            if (players == 6)
            {
                Score1.text = startPlayer.NickName + ": " + score1;
                Score2.text = startPlayer.GetNext().NickName + ": " + score2;
                Score3.text = startPlayer.GetNext().GetNext().NickName + ": " + score3;
                Score4.text = startPlayer.GetNext().GetNext().GetNext().NickName + ": " + score4;
                Score5.text = startPlayer.GetNext().GetNext().GetNext().GetNext().NickName + ": " + score5;
                Score6.text = startPlayer.GetNext().GetNext().GetNext().GetNext().GetNext().NickName + ": " + score6;
            }
            EndGame();
        }
        if (obj.Code == DEACTIVATE_SCOREBOARD_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0];
            scoreBoard.SetActive(active);
        }
        if (obj.Code == SEND_ROUND_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            int r = (int)datas[0];
            round = r;
            Round.text = "Runde: " + round.ToString();
        }
        if (obj.Code == SET_TRUMPF_AKTIVE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            bool active = (bool)datas[0]; 
            TrumpfE.SetActive(active);
            TrumpfS.SetActive(active);
            TrumpfH.SetActive(active);
            TrumpfB.SetActive(active);
        }
    }
    #endregion

}
