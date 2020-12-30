﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ShuffleCards : MonoBehaviourPun
{
    #region Public Fields

    public GameObject E6;
    public GameObject E7;
    public GameObject E8;
    public GameObject E9;
    public GameObject E10;
    public GameObject E11;
    public GameObject E12;
    public GameObject E13;
    public GameObject E14;
    public GameObject B6;
    public GameObject B7;
    public GameObject B8;
    public GameObject B9;
    public GameObject B10;
    public GameObject B11;
    public GameObject B12;
    public GameObject B13;
    public GameObject B14;
    public GameObject H6;
    public GameObject H7;
    public GameObject H8;
    public GameObject H9;
    public GameObject H10;
    public GameObject H11;
    public GameObject H12;
    public GameObject H13;
    public GameObject H14;
    public GameObject S6;
    public GameObject S7;
    public GameObject S8;
    public GameObject S9;
    public GameObject S10;
    public GameObject S11;
    public GameObject S12;
    public GameObject S13;
    public GameObject S14;

    #endregion

    #region Private Fields

    [SerializeField]
    private GameObject playerHand;
    List<string> cardsName = new List<string>();
    List<GameObject> cards = new List<GameObject>();
    private const byte CARDS_SHUFFLE_EVENT = 0;

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
            object[] datas = (object[])obj.CustomData;
            Array array = (Array)datas[0];
            foreach (string s in array)
            {
                cardsName.Add(s);
            }
            AddCards();
            for (int i = 0; i <= 35; i++)
            {
                GameObject playerCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
            }
        }
    }

    #endregion

    #region Public Methods

    public void CardShuffle()
    {
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

        PhotonNetwork.RaiseEvent(CARDS_SHUFFLE_EVENT, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

    public List<GameObject> ChangeCardToGameObject()
    {
        AddCards();
        return cards;
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
                cards.Add(E6);
            }
            if (cardName == "E7")
            {
                cards.Add(E7);
            }
            if (cardName == "E8")
            {
                cards.Add(E8);
            }
            if (cardName == "E9")
            {
                cards.Add(E9);
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
                cards.Add(B6);
            }
            if (cardName == "B7")
            {
                cards.Add(B7);
            }
            if (cardName == "B8")
            {
                cards.Add(B8);
            }
            if (cardName == "B9")
            {
                cards.Add(B9);
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
                cards.Add(H6);
            }
            if (cardName == "H7")
            {
                cards.Add(H7);
            }
            if (cardName == "H8")
            {
                cards.Add(H8);
            }
            if (cardName == "H9")
            {
                cards.Add(H9);
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
                cards.Add(S6);
            }
            if (cardName == "S7")
            {
                cards.Add(S7);
            }
            if (cardName == "S8")
            {
                cards.Add(S8);
            }
            if (cardName == "S9")
            {
                cards.Add(S9);
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

    #endregion
}