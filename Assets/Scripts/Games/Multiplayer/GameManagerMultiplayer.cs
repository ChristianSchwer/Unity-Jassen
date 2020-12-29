using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMultiplayer : MonoBehaviour
{
    #region Private Fields

    [SerializeField]
    private GameObject playerHand;
    [SerializeField]
    private ShuffleCards ShuffleCards;
    //[SerializeField]
    //private NextCard NextCard;

    #endregion

    #region Public Fields

    public List<GameObject> cards = new List<GameObject>();
    public static int start;

    #endregion

    #region MonoBehaviour Callbacks

    void Update()
    {
        //Game starts when Player hit "Start Game"-Button in the PlayerListingsMenu
        if (start == 1)
        {
            SetupGame();
            start = 0;
        }
    }

    #endregion

    #region Private Methods

    void SetupGame()
    {
        cards = ShuffleCards.CardShuffle();
        ResetCards();
        GetTrumpf();
        GiveCards();
        //NextCard.GetCards();
    }

    void ResetCards()
    {
        //foreach (GameObject card in cards)
        //{
        //    if (card.gameObject.name.Contains("9"))
        //    {
        //        currentUnit = card.GetComponent<PlayCard>();
        //        string currentName = currentUnit.unitName.Substring(0, 1);
        //        currentUnit.ResetSnell(currentName);
        //    }
        //    if (card.gameObject.name.Contains("11"))
        //    {
        //        currentUnit = card.GetComponent<PlayCard>();
        //        string currentName = currentUnit.unitName.Substring(0, 1);
        //        currentUnit.ResetBauer(currentName);
        //    }
        //}
    }

    void GetTrumpf()
    {
        //currentUnit = cards[35].GetComponent<PlayCard>();
        //trumpf = currentUnit.unitName.Substring(0, 1);
        //Trumpf.text = trumpf;
        //foreach (GameObject card in cards)
        //{
        //    if (card.gameObject.name.Contains(trumpf + 9))
        //    {
        //        currentUnit = card.GetComponent<PlayCard>();
        //        currentUnit.ChangeSnell(trumpf);
        //    }
        //    if (card.gameObject.name.Contains(trumpf + 11))
        //    {
        //        currentUnit = card.GetComponent<PlayCard>();
        //        currentUnit.ChangeBauer(trumpf);
        //    }
        //}
        //NextCard.Trumpf(trumpf);
    }

    public void GiveCards()
    {
        for (int i = 0; i <= 35; i++)
        {
            //if (i <= 8)
            //{
                GameObject playerCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
                playerCard.transform.SetParent(playerHand.transform, false);
            //}
            //if (i > 8 && i <= 17)
            //{
            //    GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
            //    enemyCard.transform.SetParent(PlayerArea2.transform, false);
            //}
            //if (i > 17 && i <= 26)
            //{
            //    GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
            //    enemyCard.transform.SetParent(PlayerArea3.transform, false);
            //}
            //if (i > 26 && i <= 35)
            //{
            //    GameObject enemyCard = Instantiate(cards[i], new Vector3(0, 0, 0), Quaternion.identity);
            //    enemyCard.transform.SetParent(PlayerArea4.transform, false);
            //}
        }
    }

    #endregion
}
