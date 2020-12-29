using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleCards : MonoBehaviour
{
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

    public List<GameObject> CardShuffle()
    {
        List<GameObject> cards = new List<GameObject>();

        cards.Add(E6);
        cards.Add(E7);
        cards.Add(E8);
        cards.Add(E9);
        cards.Add(E10);
        cards.Add(E11);
        cards.Add(E12);
        cards.Add(E13);
        cards.Add(E14);
        cards.Add(B6);
        cards.Add(B7);
        cards.Add(B8);
        cards.Add(B9);
        cards.Add(B10);
        cards.Add(B11);
        cards.Add(B12);
        cards.Add(B13);
        cards.Add(B14);
        cards.Add(H6);
        cards.Add(H7);
        cards.Add(H8);
        cards.Add(H9);
        cards.Add(H10);
        cards.Add(H11);
        cards.Add(H12);
        cards.Add(H13);
        cards.Add(H14);
        cards.Add(S6);
        cards.Add(S7);
        cards.Add(S8);
        cards.Add(S9);
        cards.Add(S10);
        cards.Add(S11);
        cards.Add(S12);
        cards.Add(S13);
        cards.Add(S14);

        System.Random rnd = new System.Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            GameObject temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
        return cards;
    }
}
