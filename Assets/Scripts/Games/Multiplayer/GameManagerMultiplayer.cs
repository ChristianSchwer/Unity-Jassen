using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { START, PLAYERTURN1, PLAYERTURN2, PLAYERTURN3, PLAYERTURN4, WON, LOST }

public class GameManagerMultiplayer : MonoBehaviour
{
    #region Private Fields

    [SerializeField]
    private GameObject playerHand;
    [SerializeField]
    private ShuffleCards ShuffleCards;
    [SerializeField]
    private NextCard NextCard;

    string trumpf;

    #endregion

    #region Public Fields

    public TurnState state;
    public List<GameObject> cards = new List<GameObject>();
    public static int start;

    #endregion

    PlayCard currentUnit;

    #region MonoBehaviour Callbacks

    void Start()
    {
        state = TurnState.START;
    }

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
        ShuffleCards.CardShuffle();
        cards = ShuffleCards.ChangeCardToGameObject();

        //ResetCards();
        //GetTrumpf();
        ShuffleCards.GiveCards();
        //NextCard.GetCards();
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
        //Trumpf.text = trumpf;
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

    #endregion

}
