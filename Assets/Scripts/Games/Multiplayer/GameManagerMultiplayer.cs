using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState { START, PLAYERTURN1, PLAYERTURN2, PLAYERTURN3, PLAYERTURN4, WON, LOST }

public class GameManagerMultiplayer : MonoBehaviourPun
{
    #region Public Fields

    public TurnState state;
    public List<GameObject> cards = new List<GameObject>();
    List<GameObject> dropzonecards = new List<GameObject>();
    List<GameObject> trumpfcards = new List<GameObject>();
    List<GameObject> othercards = new List<GameObject>();
    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();
    public static int start;
    public int activePlayer;

    #endregion

    #region Private Fields

    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private NextCardMultiplayer NextCard;
    [SerializeField]
    private GameObject playerHand;
    [SerializeField]
    private GameObject playerHandOverlay;
    [SerializeField]
    private GameObject dropZone;
    [SerializeField]
    private GameObject yard;
    [SerializeField]
    private Text currentPlayer;
    [SerializeField]
    private Text trumpf;

    string trumpfUnit;
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_PLAYER_EVENT = 2;
    private const byte TRUMPF_EVENT = 3;
    private const byte SET_AKTIVE_EVENT = 5;

    #endregion

    PlayCard currentUnit;

    #region MonoBehaviour Callbacks

    void Update()
    {
        //Game starts when Player hit "Start Game"-Button in the PlayerListingsMenu
        if (start == 1)
        {
            state = TurnState.START;
            SetupGame();
            start = 0;
        }
    }

    #endregion

    #region Public Methods

    public void PlayerTurn(int p)
    {
        if (p == 1)
        {
            state = TurnState.PLAYERTURN1;
            activePlayer = 0;
            CurrentPlayer(p);
        }
        if (p == 2)
        {
            state = TurnState.PLAYERTURN2;
            activePlayer = 0;
            CurrentPlayer(p);
        }
        if (p == 3)
        {
            state = TurnState.PLAYERTURN3;
            activePlayer = 0;
            CurrentPlayer(p);
        }
        if (p == 4)
        {
            state = TurnState.PLAYERTURN4;
            activePlayer = 0;
            CurrentPlayer(p);
        }
        if (GetChildCount() == 4)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CalculateRoundWinner(p);
            }
            else
            {
                StartCoroutine(ClearDropZone());
            }
        }
        else
        {
            NextCard.nextCard(p);
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
        ShuffleCards.CardShuffle();
        cards = ShuffleCards.ChangeCardToGameObject();
        ResetCards();
        GetTrumpf();
        ShuffleCards.GiveCards();
        NextCard.GetCards();
    }

    void ResetCards()
    {
        foreach (GameObject card in cards)
        {
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
        return dropZone.transform.childCount;
    }

    void CalculateRoundWinner(int p)
    {
        if (GetChildCount() == 4)
        {
            GameObject child1 = dropZone.transform.GetChild(0).gameObject;
            GameObject child2 = dropZone.transform.GetChild(1).gameObject;
            GameObject child3 = dropZone.transform.GetChild(2).gameObject;
            GameObject child4 = dropZone.transform.GetChild(3).gameObject;
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
        GameObject child1 = dropZone.transform.GetChild(0).gameObject;
        GameObject child2 = dropZone.transform.GetChild(1).gameObject;
        GameObject child3 = dropZone.transform.GetChild(2).gameObject;
        GameObject child4 = dropZone.transform.GetChild(3).gameObject;
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

    IEnumerator Wait(string turn)
    {
        yield return new WaitForSeconds(1.5f);
        //clear all cards frome the dropzone
        dropzonecards.Clear();
        trumpfcards.Clear();
        othercards.Clear();
        while (dropZone.transform.childCount > 0)
        {
            dropZone.transform.GetChild(0).gameObject.transform.SetParent(yard.transform, false);
        }
        for (int i = 0; i < yard.transform.childCount; i++)
        {
            yard.transform.GetChild(i).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.25f);
        if (turn == "player1")
        {
            NextCard.FirstPlayer(1);
            object[] datas = new object[] { true, 1 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (turn == "player2")
        {
            object[] datas = new object[] { true, 2 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (turn == "player3")
        {
            object[] datas = new object[] { true, 3 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
        if (turn == "player4")
        {
            object[] datas = new object[] { true, 4 };
            PhotonNetwork.RaiseEvent(SET_AKTIVE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    void CalculateWinner()
    {
        //yard1 = 0;
        //yard2 = 0;
        //yard3 = 0;
        //yard4 = 0;      //don't do that 
        //foreach (GameObject card in player1)
        //{
        //    currentUnit = card.GetComponent<PlayCard>();
        //    yard1 += currentUnit.unitValue;
        //}
        //foreach (GameObject card in player2)
        //{
        //    currentUnit = card.GetComponent<PlayCard>();
        //    yard2 += currentUnit.unitValue;
        //}
        //foreach (GameObject card in player3)
        //{
        //    currentUnit = card.GetComponent<PlayCard>();
        //    yard3 += currentUnit.unitValue;
        //}
        //foreach (GameObject card in player4)
        //{
        //    currentUnit = card.GetComponent<PlayCard>();
        //    yard4 += currentUnit.unitValue;
        //}
        //if (yard1 > yard2 && yard1 > yard3 && yard1 > yard4)
        //{
        //    state = TurnStateSingleplayer.WON;
        //}
        //if (yard2 > yard1 && yard2 > yard3 && yard2 > yard4)
        //{
        //    state = TurnStateSingleplayer.WON;
        //}
        //if (yard3 > yard1 && yard3 > yard2 && yard3 > yard4)
        //{
        //    state = TurnStateSingleplayer.WON;
        //}
        //if (yard4 > yard1 && yard4 > yard2 && yard4 > yard3)
        //{
        //    state = TurnStateSingleplayer.WON;
        //}
        //Score1.text = yard1.ToString();
        //Score2.text = yard2.ToString();
        //Score3.text = yard3.ToString();
        //Score4.text = yard4.ToString();
    }

    IEnumerator ClearDropZone()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 3; i > -1; i--)
        {
            Destroy(dropZone.transform.GetChild(i).gameObject);
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
        if (obj.Code == CURRENT_PLAYER_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string p = (string)datas[0];
            currentPlayer.text = p;
        }
        if (obj.Code == TRUMPF_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            string t = (string)datas[0];
            trumpf.text = t;
        }
    }
    #endregion

}
