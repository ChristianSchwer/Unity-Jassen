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

    public List<GameObject> cards = new List<GameObject>();
    List<GameObject> dropzonecards = new List<GameObject>();
    List<GameObject> trumpfcards = new List<GameObject>();
    List<GameObject> othercards = new List<GameObject>();
    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();
    public static int start;
    public Player activePlayer;
    public Player startPlayer;
    public int playerNumber;

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
    private Text Player1;
    [SerializeField]
    private Text Player2;
    [SerializeField]
    private Text Player3;
    [SerializeField]
    private Text Player4;
    [SerializeField]
    private Text Round;

    #endregion

    #region Private Fields

    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private NextCardMultiplayer NextCard;
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

    string trumpfUnit;
    private int round;
    private int cardCount;
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_PLAYER_EVENT = 2;
    private const byte TRUMPF_EVENT = 3;
    private const byte SET_AKTIVE_EVENT = 5;
    private const byte SEND_SCORE_EVENT= 13;
    private const byte DEACTIVATE_SCOREBOARD_EVENT= 14;
    private const byte SEND_PLAYERNUMBER_EVENT = 15;
    private const byte SEND_ROUND_EVENT = 16;

    #endregion

    PlayCard currentUnit;

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
            //Set playernickname on the card table for every player
            if (player.IsLocal)
            {
                Player1.text = player.NickName;
                Player2.text = player.GetNext().NickName;
                Player3.text = player.GetNext().GetNext().NickName;
                Player4.text = player.GetNext().GetNext().GetNext().NickName;
            }
            //Set firstplayer
            if (player.IsMasterClient)
            {
                startPlayer = player;
                playerNumber = 1;
                object[] datas = new object[] { playerNumber };
                PhotonNetwork.RaiseEvent(SEND_PLAYERNUMBER_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
            }
        }
    }

    #endregion

    #region Public Methods

    public void PlayerTurn(Player p)
    {
        CurrentPlayer(p.ActorNumber);
        activePlayer = null;
        if (GetChildCount() == 4)
        {
            round++;
            object[] datas = new object[] { round };
            PhotonNetwork.RaiseEvent(SEND_ROUND_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
            if (PhotonNetwork.IsMasterClient)
            {
                CalculateRoundWinner();
            }
            else
            {
                StartCoroutine(ClearDropZone());
            }
        }
        else
        {
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

    #endregion

    #region Private Methods

    void SetupGame()
    {
        //Shuffle cards
        ShuffleCards.CardShuffle();
        //Set round counter
        round = 1;
        object[] datas = new object[] { round };
        PhotonNetwork.RaiseEvent(SEND_ROUND_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        //Make all trumpf icons invisible
        TrumpfE.SetActive(false);
        TrumpfS.SetActive(false);
        TrumpfH.SetActive(false);
        TrumpfB.SetActive(false);
        StartCoroutine(WaitForCards());
    }

    IEnumerator WaitForCards()
    {
        yield return new WaitForSeconds(0.5f);
        cards = ShuffleCards.GetCards();
        ResetCards();
        GetTrumpf();
        object[] datas = new object[] { true, startPlayer };
        PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    void ResetCards()
    {
        foreach (GameObject card in cards)
        {
            currentUnit = card.GetComponent<PlayCard>();
            currentUnit.ResetCard();
            if (card.gameObject.name.Contains("9"))
            {
                currentUnit = card.GetComponent<PlayCard>();
                string currentName = currentUnit.unitName.Substring(0, 1);
                currentUnit.ResetSnell(currentName);
            }
            if (card.gameObject.name.Contains("11"))
            {
                currentUnit = card.GetComponent<PlayCard>();
                string currentName = currentUnit.unitName.Substring(0, 1);
                currentUnit.ResetBauer(currentName);
            }
        }
    }

    void GetTrumpf()
    {
        currentUnit = cards[35].GetComponent<PlayCard>();
        trumpfUnit = currentUnit.unitName.Substring(0, 1);
        Debug.Log(trumpfUnit);
        object[] datas = new object[] { trumpfUnit };
        PhotonNetwork.RaiseEvent(TRUMPF_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);

        foreach (GameObject card in cards)
        {
            if (card.gameObject.name.Contains(trumpfUnit + 9))
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

    int GetChildCount()
    {
        cardCount = 0;
        cardCount += currentCard.transform.childCount;
        cardCount += secondCard.transform.childCount;
        cardCount += thirdCard.transform.childCount;
        cardCount += fourthCard.transform.childCount;
        return cardCount;
    }

    void CalculateRoundWinner()
    {
        if (GetChildCount() == 4)
        {
            GameObject child1 = currentCard.transform.GetChild(0).gameObject;
            GameObject child2 = secondCard.transform.GetChild(0).gameObject;
            GameObject child3 = thirdCard.transform.GetChild(0).gameObject;
            GameObject child4 = fourthCard.transform.GetChild(0).gameObject;
            dropzonecards.Add(child1);
            dropzonecards.Add(child2);
            dropzonecards.Add(child3);
            dropzonecards.Add(child4);
            string firstcard = child1.GetComponent<PlayCard>().unitName.Substring(0, 1);
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
        GameObject child1 = currentCard.transform.GetChild(0).gameObject;
        GameObject child2 = secondCard.transform.GetChild(0).gameObject;
        GameObject child3 = thirdCard.transform.GetChild(0).gameObject;
        GameObject child4 = fourthCard.transform.GetChild(0).gameObject;
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
        StartCoroutine(Wait(winner));
    }

    IEnumerator Wait(string winner)
    {
        yield return new WaitForSeconds(3f);
        //clear all cards frome the dropzone
        dropzonecards.Clear();
        trumpfcards.Clear();
        othercards.Clear();
        currentCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        secondCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        thirdCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        fourthCard.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
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
        object[] data = new object[] { playerNumber };
        PhotonNetwork.RaiseEvent(SEND_PLAYERNUMBER_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
    }

    void CalculateWinner()
    {
        int yard1 = 0;
        int yard2 = 0;
        int yard3 = 0;
        int yard4 = 0;
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
        object[] datas = new object[] { yard1.ToString(), yard2.ToString(), yard3.ToString(), yard4.ToString() };
        PhotonNetwork.RaiseEvent(SEND_SCORE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    IEnumerator ClearDropZone()
    {
        yield return new WaitForSeconds(3f);
        Destroy(currentCard.transform.GetChild(0).gameObject);
        Destroy(secondCard.transform.GetChild(0).gameObject);
        Destroy(thirdCard.transform.GetChild(0).gameObject);
        Destroy(fourthCard.transform.GetChild(0).gameObject);
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
        //Clear Lists
        player1.Clear();
        player2.Clear();
        player3.Clear();
        player4.Clear();
        cards.Clear();
        ShuffleCards.ClearList();
        NextCard.ClearList();
        //Set start player to the next player
        startPlayer = startPlayer.GetNext();
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
        if (obj.Code == CURRENT_PLAYER_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string p = (string)datas[0];
            currentPlayer.text = "Am Zug: " + p;
        }
        if (obj.Code == TRUMPF_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string t = (string)datas[0];
            Debug.Log(t);
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
            Score1.text = startPlayer.NickName + ": " + score1;
            Score2.text = startPlayer.GetNext().NickName + ": " + score2;
            Score3.text = startPlayer.GetNext().GetNext().NickName + ": " + score3;
            Score4.text = startPlayer.GetNext().GetNext().GetNext().NickName + ": " + score4;
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
    }
    #endregion

}
