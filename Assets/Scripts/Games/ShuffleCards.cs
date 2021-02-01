using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class ShuffleCards : MonoBehaviourPun
{
    #region Public Fields

    public GameObject E06;
    public GameObject E07;
    public GameObject E08;
    public GameObject E09;
    public GameObject E10;
    public GameObject E11;
    public GameObject E12;
    public GameObject E13;
    public GameObject E14;
    public GameObject B06;
    public GameObject B07;
    public GameObject B08;
    public GameObject B09;
    public GameObject B10;
    public GameObject B11;
    public GameObject B12;
    public GameObject B13;
    public GameObject B14;
    public GameObject H06;
    public GameObject H07;
    public GameObject H08;
    public GameObject H09;
    public GameObject H10;
    public GameObject H11;
    public GameObject H12;
    public GameObject H13;
    public GameObject H14;
    public GameObject S06;
    public GameObject S07;
    public GameObject S08;
    public GameObject S09;
    public GameObject S10;
    public GameObject S11;
    public GameObject S12;
    public GameObject S13;
    public GameObject S14;

    #endregion

    #region Private Fields

    [SerializeField]
    private GameObject playerHand;
    [SerializeField]
    private GameManagerMultiplayer gameManagerMultiplayer;

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    private const byte CARDS_SHUFFLE_EVENT = 1;
    private const byte SEND_PLAYERHAND_EVENT = 7;

    List<string> cardsName = new List<string>();
    public List<GameObject> cards = new List<GameObject>();
    List<string> player1 = new List<string>();
    List<string> player2 = new List<string>();
    List<string> player3 = new List<string>();
    List<string> player4 = new List<string>();    
    List<GameObject> playerUnsorted1 = new List<GameObject>();
    List<GameObject> playerUnsorted2 = new List<GameObject>();
    List<GameObject> playerUnsorted3 = new List<GameObject>();
    List<GameObject> playerUnsorted4 = new List<GameObject>();

    PlayCard currentUnit;


    #endregion

    #region Public Methods

    public void CardShuffle()
    {
        cardsName.Clear();
        AddCardsName();
        System.Random rnd = new System.Random();
        for (int i = cardsName.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            string temp = cardsName[i];
            cardsName[i] = cardsName[j];
            cardsName[j] = temp;
        }
        object[] datas = new object[] { cardsName.ToArray() };
        PhotonNetwork.RaiseEvent(CARDS_SHUFFLE_EVENT, datas, raiseEventOptions, SendOptions.SendReliable);
    }

    public List<GameObject> GetCards()
    {
        return cards;
    }

    public void GiveCards()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                GiveCardsToPlayer(p);
            }
        }
    }

    #endregion

    #region Private Methods

    private void AddCardsName()
    {

        cardsName.Add("E6");
        cardsName.Add("E7");
        cardsName.Add("E8");
        cardsName.Add("E9");
        cardsName.Add("E10");
        cardsName.Add("E11");
        cardsName.Add("E12");
        cardsName.Add("E13");
        cardsName.Add("E14");
        cardsName.Add("B6");
        cardsName.Add("B7");
        cardsName.Add("B8");
        cardsName.Add("B9");
        cardsName.Add("B10");
        cardsName.Add("B11");
        cardsName.Add("B12");
        cardsName.Add("B13");
        cardsName.Add("B14");
        cardsName.Add("H6");
        cardsName.Add("H7");
        cardsName.Add("H8");
        cardsName.Add("H9");
        cardsName.Add("H10");
        cardsName.Add("H11");
        cardsName.Add("H12");
        cardsName.Add("H13");
        cardsName.Add("H14");
        cardsName.Add("S6");
        cardsName.Add("S7");
        cardsName.Add("S8");
        cardsName.Add("S9");
        cardsName.Add("S10");
        cardsName.Add("S11");
        cardsName.Add("S12");
        cardsName.Add("S13");
        cardsName.Add("S14");
    }

    private void AddCards()
    {
        foreach (string cardName in cardsName)
        {
            if (cardName == "E6")
            {
                cards.Add(E06);
            }
            if (cardName == "E7")
            {
                cards.Add(E07);
            }
            if (cardName == "E8")
            {
                cards.Add(E08);
            }
            if (cardName == "E9")
            {
                cards.Add(E09);
            }
            if (cardName == "E10")
            {
                cards.Add(E10);
            }
            if (cardName == "E11")
            {
                cards.Add(E11);
            }
            if (cardName == "E12")
            {
                cards.Add(E12);
            }
            if (cardName == "E13")
            {
                cards.Add(E13);
            }
            if (cardName == "E14")
            {
                cards.Add(E14);
            }
            if (cardName == "B6")
            {
                cards.Add(B06);
            }
            if (cardName == "B7")
            {
                cards.Add(B07);
            }
            if (cardName == "B8")
            {
                cards.Add(B08);
            }
            if (cardName == "B9")
            {
                cards.Add(B09);
            }
            if (cardName == "B10")
            {
                cards.Add(B10);
            }
            if (cardName == "B11")
            {
                cards.Add(B11);
            }
            if (cardName == "B12")
            {
                cards.Add(B12);
            }
            if (cardName == "B13")
            {
                cards.Add(B13);
            }
            if (cardName == "B14")
            {
                cards.Add(B14);
            }
            if (cardName == "H6")
            {
                cards.Add(H06);
            }
            if (cardName == "H7")
            {
                cards.Add(H07);
            }
            if (cardName == "H8")
            {
                cards.Add(H08);
            }
            if (cardName == "H9")
            {
                cards.Add(H09);
            }
            if (cardName == "H10")
            {
                cards.Add(H10);
            }
            if (cardName == "H11")
            {
                cards.Add(H11);
            }
            if (cardName == "H12")
            {
                cards.Add(H12);
            }
            if (cardName == "H13")
            {
                cards.Add(H13);
            }
            if (cardName == "H14")
            {
                cards.Add(H14);
            }
            if (cardName == "S6")
            {
                cards.Add(S06);
            }
            if (cardName == "S7")
            {
                cards.Add(S07);
            }
            if (cardName == "S8")
            {
                cards.Add(S08);
            }
            if (cardName == "S9")
            {
                cards.Add(S09);
            }
            if (cardName == "S10")
            {
                cards.Add(S10);
            }
            if (cardName == "S11")
            {
                cards.Add(S11);
            }
            if (cardName == "S12")
            {
                cards.Add(S12);
            }
            if (cardName == "S13")
            {
                cards.Add(S13);
            }
            if (cardName == "S14")
            {
                cards.Add(S14);
            }
        }
    }

    private void GiveCardsToPlayer(Player p)
    {
        if (gameManagerMultiplayer.startPlayer.ActorNumber == p.ActorNumber)
        {
            for (int i = 0; i <= 8; i++)
            {
                playerUnsorted1.Add(cards[i]);
            }
            playerUnsorted1.Sort(CompareList);
            foreach (GameObject card in playerUnsorted1)
            {
                GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
                player1.Add(card.name);
            }
        }
        if (gameManagerMultiplayer.startPlayer.GetNext().ActorNumber == p.ActorNumber)
        {
            for (int i = 9; i <= 17; i++)
            {
                playerUnsorted2.Add(cards[i]);
            }
            playerUnsorted2.Sort(CompareList);
            foreach (GameObject card in playerUnsorted2)
            {
                GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
                player2.Add(card.name);
            }
        }
        if (gameManagerMultiplayer.startPlayer.GetNext().GetNext().ActorNumber == p.ActorNumber)
        {
            for (int i = 18; i <= 26; i++)
            {
                playerUnsorted3.Add(cards[i]);
            }
            playerUnsorted3.Sort(CompareList);
            foreach (GameObject card in playerUnsorted3)
            {
                GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
                player3.Add(card.name);
            }
        }
        if (gameManagerMultiplayer.startPlayer.GetNext().GetNext().GetNext().ActorNumber == p.ActorNumber)
        {
            for (int i = 27; i <= 35; i++)
            {
                playerUnsorted4.Add(cards[i]);
            }
            playerUnsorted4.Sort(CompareList);
            foreach (GameObject card in playerUnsorted4)
            {
                GameObject playerCard = Instantiate(card, new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
                player4.Add(card.name);
            }
        }
        object[] data = new object[] { player1.ToArray(), player2.ToArray(), player3.ToArray(), player4.ToArray() };
        PhotonNetwork.RaiseEvent(SEND_PLAYERHAND_EVENT, data, raiseEventOptions, SendOptions.SendReliable);
    }

    private static int CompareList(GameObject card1, GameObject card2)
    {
        return card1.name.CompareTo(card2.name);
    }

    public void ResetCards(List<GameObject> cards)
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

    public void Test()
    {
        foreach (GameObject card in cards)
        {
            if (card.name.Contains("9"))
            {
                currentUnit = card.GetComponent<PlayCard>();
                Debug.Log(card);
                Debug.Log(currentUnit);
                Debug.Log(currentUnit.unitName);
                Debug.Log(currentUnit.unitPlayer);
                Debug.Log(currentUnit.unitStrength);
                Debug.Log(currentUnit.unitValue);
            }
        }
    }

    public void ClearList()
    {
        player1.Clear();
        player2.Clear();
        player3.Clear();
        player4.Clear();
        playerUnsorted1.Clear();
        playerUnsorted2.Clear();
        playerUnsorted3.Clear();
        playerUnsorted4.Clear();
        cards.Clear();
        cardsName.Clear();
}

    #endregion

    #region MonoBehaviour Callbacks

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
        if (obj.Code == CARDS_SHUFFLE_EVENT)
        {
            cardsName.Clear();
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
                cardsName.Add(s);
            }
            AddCards();
            ResetCards(cards);
            gameManagerMultiplayer.GetTrumpf(cards);
            GiveCards();
            //Test();
        }
    }

    #endregion
}
