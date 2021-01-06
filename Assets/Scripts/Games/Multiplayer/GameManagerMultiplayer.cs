using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState { START, PLAYERTURN1, PLAYERTURN2, PLAYERTURN3, PLAYERTURN4, WON, LOST }

public class GameManagerMultiplayer : MonoBehaviourPun
{
    #region Public Fields

    public TurnState state;
    public List<GameObject> cards = new List<GameObject>();
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
    private Text currentPlayer;
    [SerializeField]
    private Text trumpf;

    string trumpfUnit;
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CURRENT_PLAYER_EVENT = 2;
    private const byte TRUMPF_EVENT = 3;

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
            CalculateRoundWinner(p);
        }
        else
        {
            NextCard.nextCard(p);
        }
        if (playerHand.transform.childCount == 0 && GetChildCount() == 0)
        {
            CalculateWinner();
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

    }

    void CalculateWinner()
    {

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
