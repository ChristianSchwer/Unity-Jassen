using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnState { START, PLAYERTURN1, PLAYERTURN2, PLAYERTURN3, PLAYERTURN4, WON, LOST }

public class GameManager : MonoBehaviour
{
    public GameObject PlayerArea1;
    public GameObject PlayerArea2;
    public GameObject PlayerArea3;
    public GameObject PlayerArea4;
    public GameObject DropZone;
    public GameObject Yard;
    public NextCard NextCard;
    public ShuffleCards ShuffleCards;

    public Text PlayerTurn;
    public Text Trumpf;
    public Text Score1;
    public Text Score2;
    public Text Score3;
    public Text Score4;

    List<GameObject> cards = new List<GameObject>();
    List<GameObject> dropzonecards = new List<GameObject>();
    List<GameObject> trumpfcards = new List<GameObject>();
    List<GameObject> othercards = new List<GameObject>();
    List<GameObject> player1 = new List<GameObject>();
    List<GameObject> player2 = new List<GameObject>();
    List<GameObject> player3 = new List<GameObject>();
    List<GameObject> player4 = new List<GameObject>();

    int yard1;
    int yard2;
    int yard3;
    int yard4;

    string firstcard;
    string trumpf;

    PlayCard playerUnit1;
    PlayCard playerUnit2;
    PlayCard playerUnit3;
    PlayCard playerUnit4;
    PlayCard currentUnit;

    public TurnState state;

    // Start is called before the first frame update
    void Start()
    {
        state = TurnState.START;
        SetupGame();
    }

    void SetupGame()
    {
        cards = ShuffleCards.CardShuffle();
        ResetCards();
        GetTrumpf();
        GiveCards();
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
        trumpf = currentUnit.unitName.Substring(0, 1);
        Trumpf.text = trumpf;
        foreach (GameObject card in cards)
        {
            if (card.gameObject.name.Contains(trumpf + 9))
            {
                currentUnit = card.GetComponent<PlayCard>();
                currentUnit.ChangeSnell(trumpf);
            }
            if (card.gameObject.name.Contains(trumpf + 11))
            {
                currentUnit = card.GetComponent<PlayCard>();
                currentUnit.ChangeBauer(trumpf);
            }
        }
        NextCard.Trumpf(trumpf);
    }

    public void GiveCards()
    {
        for (int i = 0; i <= 35; i++)
        {
            if (i <= 8)
            {
                GameObject playerCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(PlayerArea1.transform, false);
            }
            if (i > 8 && i <= 17)
            {
                GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                enemyCard.transform.SetParent(PlayerArea2.transform, false);
            }
            if (i > 17 && i <= 26)
            {
                GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                enemyCard.transform.SetParent(PlayerArea3.transform, false);
            }
            if (i > 26 && i <= 35)
            {
                GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                enemyCard.transform.SetParent(PlayerArea4.transform, false);
            }
        }
    }

    public void Player1Turn()
    {
        state = TurnState.PLAYERTURN1;
        if (GetChildCount() == 4)
        {
            CalculateRoundWinner("player1");
        }
        else
        {
            NextCard.nextCard("player1");
            PlayerTurn.text = "Player1";
        }
        if (PlayerArea1.transform.childCount == 0 && GetChildCount() == 0)
        {
            CalculateWinner();
        }
    }

    public void Player2Turn()
    {
        state = TurnState.PLAYERTURN2;
        if (GetChildCount() == 4)
        {
            CalculateRoundWinner("player2");
        }
        else
        {
            NextCard.nextCard("player2");
            PlayerTurn.text = "Player2";
        }
        if (PlayerArea2.transform.childCount == 0 && GetChildCount() == 0)
        {
            CalculateWinner();
        }
    }

    public void Player3Turn()
    {
        state = TurnState.PLAYERTURN3;
        if (GetChildCount() == 4)
        {
            CalculateRoundWinner("player3");
        }
        else
        {
            NextCard.nextCard("player3");
            PlayerTurn.text = "Player3";
        }
        if (PlayerArea3.transform.childCount == 0 && GetChildCount() == 0)
        {
            CalculateWinner();
        }
    }

    public void Player4Turn()
    {
        state = TurnState.PLAYERTURN4;
        if (GetChildCount() == 4)
        {
            CalculateRoundWinner("player4");
        }
        else
        {
            NextCard.nextCard("player4");
            PlayerTurn.text = "Player4";
        }
        if (PlayerArea4.transform.childCount == 0 && GetChildCount() == 0)
        {
            CalculateWinner();
        }
    }

    void CalculateRoundWinner(string player)
    {
        //GetChild();       --> in eine eigene Funktion auslagern. 
        if (GetChildCount() == 4)
        {
            GameObject child1 = DropZone.transform.GetChild(0).gameObject;
            GameObject child2 = DropZone.transform.GetChild(1).gameObject;
            GameObject child3 = DropZone.transform.GetChild(2).gameObject;
            GameObject child4 = DropZone.transform.GetChild(3).gameObject;
            dropzonecards.Add(child1);
            dropzonecards.Add(child2);
            dropzonecards.Add(child3);
            dropzonecards.Add(child4);
            firstcard = child1.GetComponent<PlayCard>().unitName.Substring(0, 1);
            foreach (GameObject card in dropzonecards)
            {
                if (card.gameObject.name.Contains(trumpf))
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

    int GetChildCount()
    {
        return DropZone.transform.childCount;
    }

    void Winner(string winner)
    {
        GameObject child1 = DropZone.transform.GetChild(0).gameObject;
        GameObject child2 = DropZone.transform.GetChild(1).gameObject;
        GameObject child3 = DropZone.transform.GetChild(2).gameObject;
        GameObject child4 = DropZone.transform.GetChild(3).gameObject;
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
        while (DropZone.transform.childCount > 0)
        {
            DropZone.transform.GetChild(0).gameObject.transform.SetParent(Yard.transform, false);
        }
        for (int i = 0; i < Yard.transform.childCount; i++)
        {
            Yard.transform.GetChild(i).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.25f);
        if (turn == "player1")
        {
            NextCard.FirstPlayer(1);
            Player1Turn();
        }
        if (turn == "player2")
        {
            NextCard.FirstPlayer(1);
            Player2Turn();
        }
        if (turn == "player3")
        {
            NextCard.FirstPlayer(1);
            Player3Turn();
        }
        if (turn == "player4")
        {
            NextCard.FirstPlayer(1);
            Player4Turn();
        }
    }

    void CalculateWinner()
    {
        yard1 = 0;
        yard2 = 0;
        yard3 = 0;
        yard4 = 0;      //don't do that 
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
        if (yard1 > yard2 && yard1 > yard3 && yard1 > yard4)
        {
            state = TurnState.WON;
        }
        if (yard2 > yard1 && yard2 > yard3 && yard2 > yard4)
        {
            state = TurnState.WON;
        }
        if (yard3 > yard1 && yard3 > yard2 && yard3 > yard4)
        {
            state = TurnState.WON;
        }
        if (yard4 > yard1 && yard4 > yard2 && yard4 > yard3)
        {
            state = TurnState.WON;
        }
        Score1.text = yard1.ToString();
        Score2.text = yard2.ToString();
        Score3.text = yard3.ToString();
        Score4.text = yard4.ToString();
    }
}

